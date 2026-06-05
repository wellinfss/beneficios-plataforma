namespace BeneficiosPlataforma.Application.Auth.Commands;

using MediatR;
using Microsoft.Extensions.Logging;
using BeneficiosPlataforma.Infrastructure.Persistence;
using BeneficiosPlataforma.Infrastructure.Cache;
using BeneficiosPlataforma.Infrastructure.Auth;
using Microsoft.EntityFrameworkCore;

public record ResetPasswordCommand(string Token, string NewPassword)
    : IRequest<Unit>;

public class ResetPasswordCommandHandler(
    AppDbContext dbContext,
    ICacheService cacheService,
    IPasswordHasher passwordHasher,
    ILogger<ResetPasswordCommandHandler> logger)
    : IRequestHandler<ResetPasswordCommand, Unit>
{
    public async Task<Unit> Handle(
        ResetPasswordCommand request,
        CancellationToken cancellationToken)
    {
        var cacheKey = $"password_reset:{request.Token}";
        var cachedUserId = await cacheService.GetAsync(cacheKey, cancellationToken);

        if (cachedUserId == null)
        {
            logger.LogWarning("Invalid or expired password reset token");
            throw new InvalidOperationException("Invalid or expired password reset token");
        }

        if (!Guid.TryParse(cachedUserId, out var userId))
        {
            logger.LogWarning("Invalid user ID in password reset token");
            throw new InvalidOperationException("Invalid password reset token");
        }

        var user = await dbContext.Users
            .Where(u => u.Id == userId)
            .FirstOrDefaultAsync(cancellationToken);

        if (user == null)
        {
            logger.LogWarning("User not found for password reset");
            throw new InvalidOperationException("User not found");
        }

        var newPasswordHash = passwordHasher.Hash(request.NewPassword);
        user.PasswordHash = newPasswordHash;

        dbContext.Users.Update(user);
        await dbContext.SaveChangesAsync(cancellationToken);

        await cacheService.RemoveAsync(cacheKey, cancellationToken);

        logger.LogInformation("Password reset successful for user: {UserId}", userId);

        return Unit.Value;
    }
}
