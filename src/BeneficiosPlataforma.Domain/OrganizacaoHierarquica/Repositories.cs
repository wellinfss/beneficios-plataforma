namespace BeneficiosPlataforma.Domain.OrganizacaoHierarquica;

using Interfaces;
using ValueObjects;

public interface IGrupoEconomicoRepository : IRepository<GrupoEconomico>
{
    Task<PagedResult<GrupoEconomico>> GetAllPagedAsync(
        string? nome,
        string? status,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsByCnpjRaizAsync(
        CnpjRaiz cnpjRaiz,
        Guid? excludeId = null,
        CancellationToken cancellationToken = default);
}

public interface IEstipulanteRepository : IRepository<Estipulante>
{
    Task<PagedResult<Estipulante>> GetAllPagedAsync(
        string? nome,
        string? cnpj,
        Guid? grupoEconomicoId,
        string? status,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsByCnpjAsync(
        Cnpj cnpj,
        Guid? excludeId = null,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<Estipulante>> GetByGrupoEconomicoAsync(
        Guid grupoEconomicoId,
        CancellationToken cancellationToken = default);
}

public interface ISubestipulanteRepository : IRepository<Subestipulante>
{
    Task<PagedResult<Subestipulante>> GetAllPagedAsync(
        Guid? estipulanteId,
        string? nome,
        string? status,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<Subestipulante>> GetByEstipulanteAsync(
        Guid estipulanteId,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsByCnpjAsync(
        Cnpj cnpj,
        Guid? excludeId = null,
        CancellationToken cancellationToken = default);
}

public class PagedResult<T>
{
    public IReadOnlyList<T> Items { get; }
    public int TotalCount { get; }
    public int PageNumber { get; }
    public int PageSize { get; }

    public PagedResult(IReadOnlyList<T> items, int totalCount, int pageNumber, int pageSize)
    {
        Items = items;
        TotalCount = totalCount;
        PageNumber = pageNumber;
        PageSize = pageSize;
    }

    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    public bool HasNextPage => PageNumber < TotalPages;
    public bool HasPreviousPage => PageNumber > 1;
}
