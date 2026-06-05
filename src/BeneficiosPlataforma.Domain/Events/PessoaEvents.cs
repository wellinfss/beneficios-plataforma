namespace BeneficiosPlataforma.Domain.Events;

public record PessoaCriadaEvent(
    Guid TenantId,
    Guid PessoaId,
    string Nome,
    string Cpf,
    string Email) : DomainEvent
{
    public override string EventType { get; init; } = "mdm.pessoas.criada";
}

public record PessoaAtualizadaEvent(
    Guid TenantId,
    Guid PessoaId,
    string Nome,
    string Cpf,
    string Email) : DomainEvent
{
    public override string EventType { get; init; } = "mdm.pessoas.atualizada";
}
