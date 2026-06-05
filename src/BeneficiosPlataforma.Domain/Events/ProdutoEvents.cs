namespace BeneficiosPlataforma.Domain.Events;

public record ProdutoCriadoEvent(
    Guid TenantId,
    Guid ProdutoId,
    string Nome,
    string OperadoraId) : DomainEvent
{
    public override string EventType { get; init; } = "mdm.produtos.criado";
}

public record ProdutoAtualizadoEvent(
    Guid TenantId,
    Guid ProdutoId,
    string Nome) : DomainEvent
{
    public override string EventType { get; init; } = "mdm.produtos.atualizado";
}
