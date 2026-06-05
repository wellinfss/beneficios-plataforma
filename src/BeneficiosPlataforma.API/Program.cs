using BeneficiosPlataforma.API.Middleware;
using BeneficiosPlataforma.Application.Auth.Commands;
using BeneficiosPlataforma.Application.Common;
using BeneficiosPlataforma.Application.Messaging;
using BeneficiosPlataforma.Domain.Tenants;
using BeneficiosPlataforma.Infrastructure.Auth;
using BeneficiosPlataforma.Infrastructure.Cache;
using BeneficiosPlataforma.Infrastructure.Messaging;
using BeneficiosPlataforma.Infrastructure.MultiTenancy;
using BeneficiosPlataforma.Infrastructure.Persistence;
using BeneficiosPlataforma.Infrastructure.Persistence.Interceptors;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Logging
builder.Services.AddLogging();

// Database
var connectionString = configuration.GetConnectionString("DefaultConnection")
    ?? "Host=localhost;Database=beneficiostdb;Username=postgres;Password=postgres;Port=5432";

builder.Services.AddScoped<ITenantContext, TenantContext>();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddScoped<AuditSaveChangesInterceptor>();

builder.Services.AddDbContext<AppDbContext>((serviceProvider, options) =>
{
    options.UseNpgsql(connectionString);
    var auditInterceptor = serviceProvider.GetRequiredService<AuditSaveChangesInterceptor>();
    options.AddInterceptors(auditInterceptor);
});

// Redis & Caching
var redisConnection = configuration.GetConnectionString("Redis")
    ?? "localhost:6379";

var redis = ConnectionMultiplexer.Connect(redisConnection);
builder.Services.AddSingleton(redis);
builder.Services.AddScoped<ICacheService, RedisCacheService>();

// Authentication - JWT
var jwtSettings = configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"]
    ?? throw new InvalidOperationException("JWT Secret Key is not configured");
var issuer = jwtSettings["Issuer"] ?? "beneficios-plataforma";
var audience = jwtSettings["Audience"] ?? "beneficios-api";
var accessTokenExpiryMinutes = int.Parse(jwtSettings["AccessTokenExpiryMinutes"] ?? "15");

builder.Services.AddScoped<IJwtTokenService>(sp =>
    new JwtTokenService(secretKey, issuer, audience, accessTokenExpiryMinutes));

builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IRefreshTokenStore>(sp =>
    new RefreshTokenStore(redis));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
            ValidateIssuer = true,
            ValidIssuer = issuer,
            ValidateAudience = true,
            ValidAudience = audience,
            ValidateLifetime = true
        };
    });

builder.Services.AddAuthorization(options =>
{
    var permissions = new[]
    {
        "beneficiarios:read",
        "beneficiarios:write",
        "contratos:read",
        "contratos:write",
        "operadoras:read",
        "operadoras:write",
        "auditoria:read"
    };

    foreach (var permission in permissions)
    {
        options.AddPolicy(permission, policy =>
            policy.Requirements.Add(new BeneficiosPlataforma.Infrastructure.Auth.PermissionRequirement(permission)));
    }
});

builder.Services.AddScoped<Microsoft.AspNetCore.Authorization.IAuthorizationHandler, BeneficiosPlataforma.Infrastructure.Auth.PermissionAuthorizationHandler>();

// MassTransit & RabbitMQ
var rabbitMqConnection = configuration.GetConnectionString("RabbitMQ")
    ?? "amqp://guest:guest@localhost:5672";

builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(new Uri(rabbitMqConnection));
        cfg.ConfigureEndpoints(context);
    });
});

// MediatR
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssemblies(
        typeof(Program).Assembly,
        typeof(BeneficiosPlataforma.Application.Auth.Commands.LoginCommand).Assembly);

    cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
    cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
});

builder.Services.AddValidatorsFromAssembly(typeof(LoginCommand).Assembly);

// Infrastructure Services
builder.Services.AddScoped<IOutboxRepository, OutboxRepository>();
builder.Services.AddScoped<IDomainEventPublisher, DomainEventPublisher>();
builder.Services.AddScoped<IEventBus, EventBus>();
builder.Services.AddScoped<TenantMiddleware>();
builder.Services.AddScoped<BeneficiosPlataforma.Application.Messaging.INotificationService, BeneficiosPlataforma.Infrastructure.Messaging.EmailNotificationService>();

// Hosted Services
builder.Services.AddHostedService<OutboxDispatcherWorker>();

// HttpContext Accessor
builder.Services.AddHttpContextAccessor();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<TenantMiddleware>();

app.MapControllers();

// Database Migration
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();

    await SeedDefaultDataAsync(db);
}

app.Run();

static async Task SeedDefaultDataAsync(AppDbContext context)
{
    var defaultTenant = await context.Tenants
        .FirstOrDefaultAsync(t => t.Slug == "default");

    if (defaultTenant == null)
    {
        defaultTenant = new Tenant("Plataforma Admin", "default")
        {
            Id = Guid.Parse("00000000-0000-0000-0000-000000000001")
        };

        context.Tenants.Add(defaultTenant);
        await context.SaveChangesAsync();
    }

    // Seed base permissions (tenant-independent)
    var permissionsData = new[]
    {
        ("beneficiarios:read", "Read Beneficiaries"),
        ("beneficiarios:write", "Write Beneficiaries"),
        ("contratos:read", "Read Contracts"),
        ("contratos:write", "Write Contracts"),
        ("operadoras:read", "Read Operators"),
        ("operadoras:write", "Write Operators"),
        ("auditoria:read", "Read Audit")
    };

    var permissionIds = new Dictionary<string, Guid>();
    foreach (var (code, name) in permissionsData)
    {
        var permission = await context.Permissions
            .FirstOrDefaultAsync(p => p.Code == code);

        if (permission == null)
        {
            permission = new BeneficiosPlataforma.Infrastructure.Persistence.Permission
            {
                Id = Guid.NewGuid(),
                Code = code,
                Name = name
            };

            context.Permissions.Add(permission);
            await context.SaveChangesAsync();
        }

        permissionIds[code] = permission.Id;
    }

    // Seed default roles for the default tenant (with IgnoreQueryFilters for tenant scope)
    var roleNames = new[] { "ADMIN", "OPERADOR", "CONSULTOR", "READONLY" };

    foreach (var roleName in roleNames)
    {
        var existingRole = await context.Roles
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(r => r.Name == roleName && r.TenantId == defaultTenant.Id);

        if (existingRole != null)
            continue;

        var role = new BeneficiosPlataforma.Infrastructure.Persistence.Role
        {
            Id = Guid.NewGuid(),
            TenantId = defaultTenant.Id,
            Name = roleName
        };

        context.Roles.Add(role);
        await context.SaveChangesAsync();

        // Assign permissions based on role
        var rolePermissions = roleName switch
        {
            "ADMIN" => new[] { "beneficiarios:read", "beneficiarios:write", "contratos:read", "contratos:write", "operadoras:read", "operadoras:write", "auditoria:read" },
            "OPERADOR" => new[] { "beneficiarios:read", "beneficiarios:write", "contratos:read", "contratos:write" },
            "CONSULTOR" => new[] { "beneficiarios:read", "contratos:read" },
            "READONLY" => new[] { "beneficiarios:read", "contratos:read", "operadoras:read", "auditoria:read" },
            _ => Array.Empty<string>()
        };

        foreach (var permCode in rolePermissions)
        {
            if (permissionIds.TryGetValue(permCode, out var permId))
            {
                var existingRolePermission = await context.RolePermissions
                    .FirstOrDefaultAsync(rp => rp.RoleId == role.Id && rp.PermissionId == permId);

                if (existingRolePermission == null)
                {
                    context.RolePermissions.Add(new BeneficiosPlataforma.Infrastructure.Persistence.RolePermission
                    {
                        RoleId = role.Id,
                        PermissionId = permId
                    });
                }
            }
        }

        await context.SaveChangesAsync();
    }
}
