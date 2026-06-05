namespace BeneficiosPlataforma.Domain.Events;

public record EmpresaCriadaEvent(
    Guid TenantId,
    Guid EmpresaId,
    string RazaoSocial,
    string Cnpj) : DomainEvent
{
    public override string EventType { get; init; } = "mdm.empresas.criada";
}

public record EmpresaAtualizadaEvent(
    Guid TenantId,
    Guid EmpresaId,
    string RazaoSocial,
    string Cnpj) : DomainEvent
{
    public override string EventType { get; init; } = "mdm.empresas.atualizada";
}
