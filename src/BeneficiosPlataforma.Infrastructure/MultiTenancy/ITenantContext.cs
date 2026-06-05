namespace BeneficiosPlataforma.Infrastructure.MultiTenancy;

public interface ITenantContext
{
    Guid TenantId { get; }
    string? TenantSlug { get; }
    void SetTenant(Guid id, string slug);
}
