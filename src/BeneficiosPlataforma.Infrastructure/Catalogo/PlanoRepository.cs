namespace BeneficiosPlataforma.Infrastructure.Catalogo;

using Domain.Catalogo;
using Persistence;
using Microsoft.EntityFrameworkCore;

public class PlanoRepository : Repository<Plano>, IPlanoRepository
{
    private readonly AppDbContext _context;

    public PlanoRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<PagedResult<Plano>> GetAllPagedAsync(
        string? nome,
        Guid? operadoraId,
        string? tipoBeneficio,
        string? status,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Planos.AsQueryable();

        if (!string.IsNullOrWhiteSpace(nome))
            query = query.Where(p => p.Nome.Contains(nome));

        if (operadoraId.HasValue || !string.IsNullOrWhiteSpace(tipoBeneficio))
        {
            query = query.Join(
                _context.Produtos,
                plano => plano.ProdutoId,
                produto => produto.Id,
                (plano, produto) => new { plano, produto }
            ).Where(x =>
                (!operadoraId.HasValue || x.produto.OperadoraId == operadoraId.Value) &&
                (string.IsNullOrWhiteSpace(tipoBeneficio) || x.produto.TipoBeneficio == tipoBeneficio)
            ).Select(x => x.plano);
        }

        if (!string.IsNullOrWhiteSpace(status))
            query = query.Where(p => p.Status == status);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(p => p.Nome)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<Plano>(items, totalCount, page, pageSize);
    }

    public async Task<IEnumerable<Plano>> GetByProdutoAsync(
        Guid produtoId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Planos
            .Where(p => p.ProdutoId == produtoId)
            .ToListAsync(cancellationToken);
    }
}
