namespace BeneficiosPlataforma.Infrastructure.MultiTenancy;

public class TenantContext : ITenantContext
{
    private Guid _tenantId = Guid.Empty;
    private string? _tenantSlug;

    public Guid TenantId => _tenantId;
    public string? TenantSlug => _tenantSlug;

    public void SetTenant(Guid id, string slug)
    {
        _tenantId = id;
        _tenantSlug = slug;
    }
}
