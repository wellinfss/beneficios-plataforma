namespace BeneficiosPlataforma.Domain.Events;

public record GrupoEconomicoCriadoEvent(
    Guid TenantId,
    Guid GrupoEconomicoId,
    string Nome,
    string CnpjRaiz) : DomainEvent
{
    public override string EventType { get; init; } = "organizacao.grupo-economico.criado";
}

public record GrupoEconomicoAtualizadoEvent(
    Guid TenantId,
    Guid GrupoEconomicoId,
    string Nome,
    string CnpjRaiz) : DomainEvent
{
    public override string EventType { get; init; } = "organizacao.grupo-economico.atualizado";
}

public record EstipulanteCriadoEvent(
    Guid TenantId,
    Guid EstipulanteId,
    string RazaoSocial,
    string Cnpj,
    Guid? GrupoEconomicoId) : DomainEvent
{
    public override string EventType { get; init; } = "organizacao.estipulante.criado";
}

public record EstipulanteAtualizadoEvent(
    Guid TenantId,
    Guid EstipulanteId,
    string RazaoSocial,
    string Cnpj,
    Guid? GrupoEconomicoId) : DomainEvent
{
    public override string EventType { get; init; } = "organizacao.estipulante.atualizado";
}

public record SubestipulanteCriadoEvent(
    Guid TenantId,
    Guid SubestipulanteId,
    string RazaoSocial,
    string Cnpj,
    Guid EstipulanteId) : DomainEvent
{
    public override string EventType { get; init; } = "organizacao.subestipulante.criado";
}

public record SubestipulanteAtualizadoEvent(
    Guid TenantId,
    Guid SubestipulanteId,
    string RazaoSocial,
    string Cnpj,
    Guid EstipulanteId) : DomainEvent
{
    public override string EventType { get; init; } = "organizacao.subestipulante.atualizado";
}
