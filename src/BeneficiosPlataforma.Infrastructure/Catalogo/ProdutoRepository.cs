namespace BeneficiosPlataforma.Infrastructure.Catalogo;

using Domain.Catalogo;
using Persistence;
using Microsoft.EntityFrameworkCore;

public class ProdutoRepository : Repository<Produto>, IProdutoRepository
{
    private readonly AppDbContext _context;

    public ProdutoRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<PagedResult<Produto>> GetAllPagedAsync(
        string? nome,
        Guid? operadoraId,
        string? tipoBeneficio,
        string? status,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Produtos.AsQueryable();

        if (!string.IsNullOrWhiteSpace(nome))
            query = query.Where(p => p.Nome.Contains(nome));

        if (operadoraId.HasValue)
            query = query.Where(p => p.OperadoraId == operadoraId.Value);

        if (!string.IsNullOrWhiteSpace(tipoBeneficio))
            query = query.Where(p => p.TipoBeneficio == tipoBeneficio);

        if (!string.IsNullOrWhiteSpace(status))
            query = query.Where(p => p.Status == status);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(p => p.Nome)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<Produto>(items, totalCount, page, pageSize);
    }

    public async Task<IEnumerable<Produto>> GetByOperadoraAsync(
        Guid operadoraId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Produtos
            .Where(p => p.OperadoraId == operadoraId)
            .ToListAsync(cancellationToken);
    }
}
