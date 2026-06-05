namespace BeneficiosPlataforma.Domain.Common;

public interface IMasterDataEntity : ITenantEntity
{
    Guid GlobalId { get; }
    string MdmStatus { get; }
    DateTime? LastSyncAt { get; }
}
