namespace BeneficiosPlataforma.Application.Auth.Commands;

using MediatR;
using Microsoft.Extensions.Logging;
using BeneficiosPlataforma.Infrastructure.Persistence;
using BeneficiosPlataforma.Infrastructure.Cache;
using BeneficiosPlataforma.Infrastructure.MultiTenancy;
using BeneficiosPlataforma.Application.Messaging;
using Microsoft.EntityFrameworkCore;

public record ForgotPasswordCommand(string Email, string TenantSlug)
    : IRequest<Unit>;

public class ForgotPasswordCommandHandler(
    AppDbContext dbContext,
    ITenantContext tenantContext,
    ICacheService cacheService,
    ILogger<ForgotPasswordCommandHandler> logger,
    INotificationService notificationService)
    : IRequestHandler<ForgotPasswordCommand, Unit>
{
    public async Task<Unit> Handle(
        ForgotPasswordCommand request,
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

        tenantContext.SetTenant(tenant.Id, tenant.Slug);

        var user = await dbContext.Users
            .Where(u => u.Email == request.Email && u.TenantId == tenant.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (user == null)
        {
            logger.LogWarning("User not found for password reset: {Email}", request.Email);
            throw new InvalidOperationException("User not found");
        }

        var resetToken = Guid.NewGuid().ToString("N");
        var cacheKey = $"password_reset:{resetToken}";

        await cacheService.SetAsync(cacheKey, user.Id.ToString(), TimeSpan.FromHours(1), cancellationToken);

        var resetUrl = $"https://app.beneficiosos.local/reset-password?token={resetToken}&tenantSlug={tenant.Slug}";
        await notificationService.SendPasswordResetEmailAsync(user.Email, user.Name, resetUrl, cancellationToken);

        logger.LogInformation("Password reset email sent to user: {Email}", request.Email);

        return Unit.Value;
    }
}
