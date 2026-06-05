namespace BeneficiosPlataforma.Infrastructure.Auth;

using StackExchange.Redis;

public interface IRefreshTokenStore
{
    Task StoreAsync(string token, Guid userId, Guid tenantId, TimeSpan expiry, CancellationToken cancellationToken = default);
    Task<(Guid UserId, Guid TenantId)?> GetAsync(string token, CancellationToken cancellationToken = default);
    Task RevokeAsync(string token, CancellationToken cancellationToken = default);
}

public class RefreshTokenStore : IRefreshTokenStore
{
    private readonly IDatabase _db;

    public RefreshTokenStore(IConnectionMultiplexer redis)
    {
        _db = redis.GetDatabase();
    }

    public async Task StoreAsync(string token, Guid userId, Guid tenantId, TimeSpan expiry, CancellationToken cancellationToken = default)
    {
        var key = $"refresh_token:{token}";
        var value = $"{userId}:{tenantId}";
        await _db.StringSetAsync(key, value, expiry);
    }

    public async Task<(Guid UserId, Guid TenantId)?> GetAsync(string token, CancellationToken cancellationToken = default)
    {
        var key = $"refresh_token:{token}";
        var value = await _db.StringGetAsync(key);

        if (!value.HasValue)
            return null;

        var parts = value.ToString().Split(':');
        if (parts.Length != 2 || !Guid.TryParse(parts[0], out var userId) || !Guid.TryParse(parts[1], out var tenantId))
            return null;

        return (userId, tenantId);
    }

    public async Task RevokeAsync(string token, CancellationToken cancellationToken = default)
    {
        var key = $"refresh_token:{token}";
        await _db.KeyDeleteAsync(key);
    }
}
