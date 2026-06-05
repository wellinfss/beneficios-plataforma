using BeneficiosPlataforma.API.Middleware;
using BeneficiosPlataforma.Application.Common;
using BeneficiosPlataforma.Application.Messaging;
using BeneficiosPlataforma.Domain.Tenants;
using BeneficiosPlataforma.Infrastructure.Auth;
using BeneficiosPlataforma.Infrastructure.Cache;
using BeneficiosPlataforma.Infrastructure.Messaging;
using BeneficiosPlataforma.Infrastructure.MultiTenancy;
using BeneficiosPlataforma.Infrastructure.Persistence;
using BeneficiosPlataforma.Infrastructure.Persistence.Interceptors;
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

builder.Services.AddAuthorization();

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

builder.Services.AddFluentValidation();

// Infrastructure Services
builder.Services.AddScoped<IOutboxRepository, OutboxRepository>();
builder.Services.AddScoped<IDomainEventPublisher, DomainEventPublisher>();
builder.Services.AddScoped<IEventBus, EventBus>();
builder.Services.AddScoped<TenantMiddleware>();

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

    // Seed default roles and permissions
    var adminRole = await context.Roles
        .FirstOrDefaultAsync(r => r.Name == "ADMIN");

    if (adminRole == null)
    {
        adminRole = new BeneficiosPlataforma.Infrastructure.Persistence.Role
        {
            Id = Guid.NewGuid(),
            TenantId = defaultTenant.Id,
            Name = "ADMIN"
        };

        context.Roles.Add(adminRole);
        await context.SaveChangesAsync();
    }

    // Seed base permissions
    var permissions = new[]
    {
        ("beneficiarios:read", "Read Beneficiaries"),
        ("beneficiarios:write", "Write Beneficiaries"),
        ("contratos:read", "Read Contracts"),
        ("contratos:write", "Write Contracts"),
        ("operadoras:read", "Read Operators"),
        ("operadoras:write", "Write Operators"),
        ("auditoria:read", "Read Audit")
    };

    foreach (var (code, name) in permissions)
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
        }
    }

    await context.SaveChangesAsync();
}
