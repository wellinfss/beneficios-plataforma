namespace BeneficiosPlataforma.Domain.Catalogo;

using Interfaces;
using ValueObjects;
using Application.Common;

public interface IOperadoraRepository : IRepository<Operadora>
{
    Task<PagedResult<Operadora>> GetAllPagedAsync(
        string? razaoSocial,
        string? tipo,
        string? status,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsByCnpjAsync(
        Cnpj cnpj,
        Guid? excludeId = null,
        CancellationToken cancellationToken = default);
}

public interface IProdutoRepository : IRepository<Produto>
{
    Task<PagedResult<Produto>> GetAllPagedAsync(
        string? nome,
        Guid? operadoraId,
        string? tipoBeneficio,
        string? status,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<Produto>> GetByOperadoraAsync(
        Guid operadoraId,
        CancellationToken cancellationToken = default);
}

public interface IPlanoRepository : IRepository<Plano>
{
    Task<PagedResult<Plano>> GetAllPagedAsync(
        string? nome,
        Guid? operadoraId,
        string? tipoBeneficio,
        string? status,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<Plano>> GetByProdutoAsync(
        Guid produtoId,
        CancellationToken cancellationToken = default);
}
