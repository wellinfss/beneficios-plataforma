namespace BeneficiosPlataforma.Application.Auth.Commands;

using Common;
using MediatR;
using Microsoft.Extensions.Logging;
using BeneficiosPlataforma.Infrastructure.Persistence;
using BeneficiosPlataforma.Infrastructure.Auth;
using BeneficiosPlataforma.Infrastructure.MultiTenancy;
using Microsoft.EntityFrameworkCore;

public record LoginCommand(string Email, string Password, string TenantSlug)
    : IRequest<AuthResponseDto>;

public class LoginCommandHandler(
    AppDbContext dbContext,
    ITenantContext tenantContext,
    IJwtTokenService jwtTokenService,
    IRefreshTokenStore refreshTokenStore,
    IPasswordHasher passwordHasher,
    ILogger<LoginCommandHandler> logger)
    : IRequestHandler<LoginCommand, AuthResponseDto>
{
    public async Task<AuthResponseDto> Handle(
        LoginCommand request,
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

        var user = await dbContext.Users
            .Where(u => u.Email == request.Email)
            .FirstOrDefaultAsync(cancellationToken);

        if (user == null || user.Status != "ACTIVE")
        {
            logger.LogWarning("User not found or inactive: {Email}", request.Email);
            throw new InvalidOperationException("Invalid credentials");
        }

        if (!passwordHasher.Verify(request.Password, user.PasswordHash))
        {
            logger.LogWarning("Invalid password for user: {Email}", request.Email);
            throw new InvalidOperationException("Invalid credentials");
        }

        var accessToken = jwtTokenService.GenerateAccessToken(user.Id, user.Email, tenant.Id);
        var refreshToken = jwtTokenService.GenerateRefreshToken();

        await refreshTokenStore.StoreAsync(
            refreshToken,
            user.Id,
            tenant.Id,
            TimeSpan.FromDays(7),
            cancellationToken);

        logger.LogInformation("User {Email} logged in for tenant {TenantSlug}",
            request.Email, request.TenantSlug);

        return new AuthResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresIn = 900
        };
    }
}
