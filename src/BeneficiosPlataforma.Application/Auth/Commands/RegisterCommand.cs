namespace BeneficiosPlataforma.Application.Auth.Commands;

using Common;
using MediatR;
using Microsoft.Extensions.Logging;
using BeneficiosPlataforma.Infrastructure.Persistence;
using BeneficiosPlataforma.Infrastructure.Auth;
using BeneficiosPlataforma.Infrastructure.MultiTenancy;
using Microsoft.EntityFrameworkCore;

public record RegisterCommand(string Name, string Email, string Password, string TenantSlug)
    : IRequest<AuthResponseDto>;

public class RegisterCommandHandler(
    AppDbContext dbContext,
    ITenantContext tenantContext,
    IJwtTokenService jwtTokenService,
    IRefreshTokenStore refreshTokenStore,
    IPasswordHasher passwordHasher,
    ILogger<RegisterCommandHandler> logger)
    : IRequestHandler<RegisterCommand, AuthResponseDto>
{
    public async Task<AuthResponseDto> Handle(
        RegisterCommand request,
        CancellationToken cancellationToken)
    {
        var tenant = await dbContext.Tenants
            .Where(t => t.Slug == request.TenantSlug.ToLowerInvariant())
            .FirstOrDefaultAsync(cancellationToken);

        if (tenant == null)
        {
            logger.LogWarning("Tenant not found: {TenantSlug}", request.TenantSlug);
            throw new InvalidOperationException("Tenant not found");
        }

        if (tenant.Status != "ACTIVE")
        {
            logger.LogWarning("Tenant is not active: {TenantId}", tenant.Id);
            throw new InvalidOperationException("Tenant is not active");
        }

        tenantContext.SetTenant(tenant.Id, tenant.Slug);

        var existingUser = await dbContext.Users
            .Where(u => u.Email == request.Email)
            .FirstOrDefaultAsync(cancellationToken);

        if (existingUser != null)
        {
            logger.LogWarning("Email already exists: {Email}", request.Email);
            throw new InvalidOperationException("Email already registered");
        }

        var passwordHash = passwordHasher.Hash(request.Password);

        var user = new User
        {
            Id = Guid.NewGuid(),
            TenantId = tenant.Id,
            Email = request.Email,
            Name = request.Name,
            PasswordHash = passwordHash,
            Status = "ACTIVE"
        };

        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync(cancellationToken);

        var operatorRole = await dbContext.Roles
            .FirstOrDefaultAsync(r => r.Name == "OPERADOR" && r.TenantId == tenant.Id, cancellationToken);

        if (operatorRole == null)
        {
            logger.LogError("OPERADOR role not found for tenant {TenantId}. Ensure roles are seeded.", tenant.Id);
            throw new InvalidOperationException("OPERADOR role not configured for tenant");
        }

        var userRole = new UserRole
        {
            UserId = user.Id,
            RoleId = operatorRole.Id
        };

        dbContext.UserRoles.Add(userRole);
        await dbContext.SaveChangesAsync(cancellationToken);

        var roles = new[] { "OPERADOR" };
        var permissions = await GetUserPermissionsAsync(user.Id, cancellationToken);

        var accessToken = jwtTokenService.GenerateAccessToken(user.Id, user.Email, tenant.Id, roles, permissions);
        var refreshToken = jwtTokenService.GenerateRefreshToken();

        await refreshTokenStore.StoreAsync(
            refreshToken,
            user.Id,
            tenant.Id,
            TimeSpan.FromDays(7),
            cancellationToken);

        logger.LogInformation("User {Email} registered for tenant {TenantSlug}",
            request.Email, request.TenantSlug);

        return new AuthResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresIn = 900
        };
    }

    private async Task<string[]> GetUserPermissionsAsync(Guid userId, CancellationToken cancellationToken)
    {
        return await dbContext.UserRoles
            .Where(ur => ur.UserId == userId)
            .Join(dbContext.RolePermissions,
                ur => ur.RoleId,
                rp => rp.RoleId,
                (ur, rp) => rp.PermissionId)
            .Join(dbContext.Permissions,
                permId => permId,
                perm => perm.Id,
                (permId, perm) => perm.Code)
            .Distinct()
            .ToArrayAsync(cancellationToken);
    }
}
