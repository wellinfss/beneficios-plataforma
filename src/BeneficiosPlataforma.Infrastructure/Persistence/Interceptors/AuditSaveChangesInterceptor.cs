namespace BeneficiosPlataforma.Infrastructure.Persistence.Interceptors;

using Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using MultiTenancy;
using System.Text.Json;

public class AuditSaveChangesInterceptor : SaveChangesInterceptor
{
    private readonly ITenantContext _tenantContext;
    private readonly ICurrentUserService _currentUserService;

    public AuditSaveChangesInterceptor(
        ITenantContext tenantContext,
        ICurrentUserService currentUserService)
    {
        _tenantContext = tenantContext;
        _currentUserService = currentUserService;
    }

    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        var context = eventData.Context;
        if (context == null)
            return await base.SavingChangesAsync(eventData, result, cancellationToken);

        var entries = context.ChangeTracker
            .Entries<BaseEntity>()
            .Where(e => e.State is EntityState.Added or EntityState.Modified or EntityState.Deleted)
            .ToList();

        var auditLogs = new List<AuditLog>();

        foreach (var entry in entries)
        {
            var auditLog = new AuditLog
            {
                Id = Guid.NewGuid(),
                TenantId = _tenantContext.TenantId,
                UserId = _currentUserService.GetCurrentUserId(),
                EntityName = entry.Entity.GetType().Name,
                EntityId = entry.Entity.Id,
                Action = entry.State.ToString(),
                CreatedAt = DateTime.UtcNow
            };

            if (entry.State != EntityState.Added)
            {
                var originalValues = entry.OriginalValues;
                var oldValueDict = new Dictionary<string, object?>();

                foreach (var property in entry.Properties)
                {
                    oldValueDict[property.Metadata.Name] = originalValues[property.Metadata.Name];
                }

                auditLog.OldValue = JsonSerializer.Serialize(oldValueDict);
            }

            var currentValues = entry.CurrentValues;
            var newValueDict = new Dictionary<string, object?>();

            foreach (var property in entry.Properties)
            {
                if (entry.State != EntityState.Deleted)
                {
                    newValueDict[property.Metadata.Name] = currentValues[property.Metadata.Name];
                }
            }

            if (entry.State != EntityState.Deleted)
                auditLog.NewValue = JsonSerializer.Serialize(newValueDict);

            auditLogs.Add(auditLog);
        }

        if (auditLogs.Count > 0 && context is AppDbContext dbContext)
        {
            dbContext.AuditLogs.AddRange(auditLogs);
        }

        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}

public interface ICurrentUserService
{
    Guid? GetCurrentUserId();
}

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid? GetCurrentUserId()
    {
        var userIdClaim = _httpContextAccessor.HttpContext?.User
            .FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        if (Guid.TryParse(userIdClaim, out var userId))
            return userId;

        return null;
    }
}
