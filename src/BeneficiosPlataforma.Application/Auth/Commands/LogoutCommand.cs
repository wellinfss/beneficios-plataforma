namespace BeneficiosPlataforma.Application.Auth.Commands;

using MediatR;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

public record LogoutCommand(Guid UserId)
    : IRequest<Unit>;

public class LogoutCommandHandler(
    IConnectionMultiplexer redis,
    ILogger<LogoutCommandHandler> logger)
    : IRequestHandler<LogoutCommand, Unit>
{
    public async Task<Unit> Handle(
        LogoutCommand request,
        CancellationToken cancellationToken)
    {
        var db = redis.GetDatabase();
        var logoutKey = $"user_logout:{request.UserId}";
        await db.StringSetAsync(logoutKey, DateTime.UtcNow.ToString("O"), TimeSpan.FromDays(7));

        logger.LogInformation("User {UserId} logged out", request.UserId);
        return Unit.Value;
    }
}
