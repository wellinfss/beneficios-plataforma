namespace BeneficiosPlataforma.Application.Tenants.Queries;

using Common;
using MediatR;
using Microsoft.Extensions.Logging;
using BeneficiosPlataforma.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

public record GetTenantBySlugQuery(string Slug)
    : IRequest<TenantDto?>;

public class GetTenantBySlugQueryHandler(
    AppDbContext dbContext,
    ILogger<GetTenantBySlugQueryHandler> logger)
    : IRequestHandler<GetTenantBySlugQuery, TenantDto?>
{
    public async Task<TenantDto?> Handle(
        GetTenantBySlugQuery request,
        CancellationToken cancellationToken)
    {
        var tenant = await dbContext.Tenants
            .Where(t => t.Slug == request.Slug.ToLowerInvariant())
            .FirstOrDefaultAsync(cancellationToken);

        if (tenant == null)
        {
            logger.LogWarning("Tenant not found: {Slug}", request.Slug);
            return null;
        }

        logger.LogInformation("Tenant found: {Slug}", request.Slug);
        return new TenantDto
        {
            Id = tenant.Id,
            Name = tenant.Name,
            Slug = tenant.Slug,
            Status = tenant.Status
        };
    }
}
