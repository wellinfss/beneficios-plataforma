namespace BeneficiosPlataforma.Application.Auth.Commands;

using Common;
using MediatR;
using Microsoft.Extensions.Logging;
using BeneficiosPlataforma.Infrastructure.Persistence;
using BeneficiosPlataforma.Infrastructure.Auth;
using BeneficiosPlataforma.Infrastructure.MultiTenancy;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

public record RefreshTokenCommand(string RefreshToken)
    : IRequest<AuthResponseDto>;

public class RefreshTokenCommandHandler(
    AppDbContext dbContext,
    ITenantContext tenantContext,
    IJwtTokenService jwtTokenService,
    IRefreshTokenStore refreshTokenStore,
    IConnectionMultiplexer redis,
    ILogger<RefreshTokenCommandHandler> logger)
    : IRequestHandler<RefreshTokenCommand, AuthResponseDto>
{
    public async Task<AuthResponseDto> Handle(
        RefreshTokenCommand request,
        CancellationToken cancellationToken)
    {
        var tokenData = await refreshTokenStore.GetAsync(request.RefreshToken, cancellationToken);
        if (tokenData == null)
        {
            logger.LogWarning("Invalid or expired refresh token");
            throw new InvalidOperationException("Invalid refresh token");
        }

        var (userId, tenantId) = tokenData.Value;

        var db = redis.GetDatabase();
        var logoutKey = $"user_logout:{userId}";
        var logoutTimestamp = await db.StringGetAsync(logoutKey);
        if (logoutTimestamp.HasValue && DateTime.TryParse(logoutTimestamp.ToString(), out var logoutTime))
        {
            logger.LogWarning("User {UserId} has been logged out", userId);
            throw new InvalidOperationException("Invalid refresh token");
        }

        tenantContext.SetTenant(tenantId, string.Empty);

        var user = await dbContext.Users
            .Where(u => u.Id == userId)
            .FirstOrDefaultAsync(cancellationToken);

        if (user == null || user.Status != "ACTIVE")
        {
            logger.LogWarning("User not found or inactive: {UserId}", userId);
            throw new InvalidOperationException("Invalid refresh token");
        }

        var tenant = await dbContext.Tenants
            .Where(t => t.Id == tenantId)
            .FirstOrDefaultAsync(cancellationToken);

        if (tenant == null || tenant.Status != "ACTIVE")
        {
            logger.LogWarning("Tenant not found or inactive: {TenantId}", tenantId);
            throw new InvalidOperationException("Invalid refresh token");
        }

        var roles = await GetUserRolesAsync(user.Id, cancellationToken);
        var permissions = await GetUserPermissionsAsync(user.Id, cancellationToken);

        var accessToken = jwtTokenService.GenerateAccessToken(user.Id, user.Email, tenant.Id, roles, permissions);

        logger.LogInformation("Token refreshed for user {UserId}", userId);

        return new AuthResponseDto
        {
            AccessToken = accessToken,
            ExpiresIn = 900
        };
    }

    private async Task<string[]> GetUserRolesAsync(Guid userId, CancellationToken cancellationToken)
    {
        return await dbContext.UserRoles
            .Where(ur => ur.UserId == userId)
            .Join(dbContext.Roles,
                ur => ur.RoleId,
                role => role.Id,
                (ur, role) => role.Name)
            .ToArrayAsync(cancellationToken);
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
