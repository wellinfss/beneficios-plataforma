namespace BeneficiosPlataforma.API.Middleware;

using Application.Common;
using Application.Tenants.Queries;
using Infrastructure.Cache;
using Infrastructure.MultiTenancy;
using MediatR;

public class TenantMiddleware : IMiddleware
{
    private readonly ITenantContext _tenantContext;
    private readonly ICacheService _cacheService;
    private readonly IMediator _mediator;
    private readonly ILogger<TenantMiddleware> _logger;

    public TenantMiddleware(
        ITenantContext tenantContext,
        ICacheService cacheService,
        IMediator mediator,
        ILogger<TenantMiddleware> logger)
    {
        _tenantContext = tenantContext;
        _cacheService = cacheService;
        _mediator = mediator;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var tenantSlug = ExtractTenantSlug(context);

        if (string.IsNullOrEmpty(tenantSlug))
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsJsonAsync(new { error = "Tenant not found" });
            return;
        }

        var cacheKey = $"tenant:{tenantSlug}";
        var cachedTenant = await _cacheService.GetAsync<TenantDto>(cacheKey);

        TenantDto? tenant = null;

        if (cachedTenant != null)
        {
            tenant = cachedTenant;
        }
        else
        {
            tenant = await _mediator.Send(new GetTenantBySlugQuery(tenantSlug));

            if (tenant != null)
            {
                await _cacheService.SetAsync(cacheKey, tenant, TimeSpan.FromMinutes(5));
            }
        }

        if (tenant == null || tenant.Status != "ACTIVE")
        {
            context.Response.StatusCode = StatusCodes.Status404NotFound;
            await context.Response.WriteAsJsonAsync(new { error = "Tenant not found or inactive" });
            return;
        }

        _tenantContext.SetTenant(tenant.Id, tenant.Slug);
        _logger.LogInformation("Tenant resolved: {TenantSlug}", tenantSlug);

        await next(context);
    }

    private string? ExtractTenantSlug(HttpContext context)
    {
        if (context.Request.Headers.TryGetValue("X-Tenant-Id", out var tenantIdHeader))
            return tenantIdHeader.ToString();

        var host = context.Request.Host.Host;
        var parts = host.Split('.');

        if (parts.Length > 1 && parts[0] != "localhost" && parts[0] != "127")
            return parts[0];

        return "default";
    }
}
