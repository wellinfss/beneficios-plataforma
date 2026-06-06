namespace BeneficiosPlataforma.Infrastructure.Catalogo;

using Domain.Catalogo;
using Domain.ValueObjects;
using Persistence;
using Microsoft.EntityFrameworkCore;

public class OperadoraRepository : Repository<Operadora>, IOperadoraRepository
{
    private readonly AppDbContext _context;

    public OperadoraRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<PagedResult<Operadora>> GetAllPagedAsync(
        string? razaoSocial,
        string? tipo,
        string? status,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Operadoras.AsQueryable();

        if (!string.IsNullOrWhiteSpace(razaoSocial))
            query = query.Where(o => o.RazaoSocial.Contains(razaoSocial));

        if (!string.IsNullOrWhiteSpace(tipo))
            query = query.Where(o => o.Tipo == tipo);

        if (!string.IsNullOrWhiteSpace(status))
            query = query.Where(o => o.Status == status);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(o => o.RazaoSocial)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<Operadora>(items, totalCount, page, pageSize);
    }

    public async Task<bool> ExistsByCnpjAsync(
        Cnpj cnpj,
        Guid? excludeId = null,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Operadoras
            .Where(o => o.Cnpj.Value == cnpj.Value);

        if (excludeId.HasValue)
            query = query.Where(o => o.Id != excludeId.Value);

        return await query.AnyAsync(cancellationToken);
    }
}
