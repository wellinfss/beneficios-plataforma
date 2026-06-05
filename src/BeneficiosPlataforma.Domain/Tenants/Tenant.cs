namespace BeneficiosPlataforma.Domain.Tenants;

using Common;

public class Tenant : BaseEntity
{
    public string Name { get; private set; } = null!;
    public string Slug { get; private set; } = null!;
    public string Status { get; private set; } = "ACTIVE";

    private Tenant() { }

    public Tenant(string name, string slug)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Tenant name cannot be empty.");
        if (string.IsNullOrWhiteSpace(slug))
            throw new DomainException("Tenant slug cannot be empty.");

        Name = name;
        Slug = slug.ToLowerInvariant();
        Status = "ACTIVE";
    }

    public void Deactivate()
    {
        Status = "INACTIVE";
        UpdateTimestamp();
    }

    public void Activate()
    {
        Status = "ACTIVE";
        UpdateTimestamp();
    }
}
