namespace BeneficiosPlataforma.Infrastructure.OrganizacaoHierarquica;

using BeneficiosPlataforma.Domain.OrganizacaoHierarquica;
using BeneficiosPlataforma.Domain.ValueObjects;
using BeneficiosPlataforma.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

public class GrupoEconomicoRepository : IGrupoEconomicoRepository
{
    private readonly AppDbContext _dbContext;

    public GrupoEconomicoRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<GrupoEconomico?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.GruposEconomicos
            .FirstOrDefaultAsync(g => g.Id == id && !g.IsDeleted, cancellationToken);
    }

    public async Task AddAsync(GrupoEconomico entity, CancellationToken cancellationToken = default)
    {
        await _dbContext.GruposEconomicos.AddAsync(entity, cancellationToken);
    }

    public void Update(GrupoEconomico entity)
    {
        _dbContext.GruposEconomicos.Update(entity);
    }

    public void Delete(GrupoEconomico entity)
    {
        entity.SoftDelete();
        _dbContext.GruposEconomicos.Update(entity);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<PagedResult<GrupoEconomico>> GetAllPagedAsync(
        string? nome,
        string? status,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.GruposEconomicos.Where(g => !g.IsDeleted);

        if (!string.IsNullOrWhiteSpace(nome))
            query = query.Where(g => EF.Functions.ILike(g.Nome, $"%{nome}%"));

        if (!string.IsNullOrWhiteSpace(status))
            query = query.Where(g => g.Status == status);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(g => g.Nome)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<GrupoEconomico>(items, totalCount, page, pageSize);
    }

    public async Task<bool> ExistsByCnpjRaizAsync(
        CnpjRaiz cnpjRaiz,
        Guid? excludeId = null,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.GruposEconomicos
            .Where(g => !g.IsDeleted && g.CnpjRaiz == cnpjRaiz);

        if (excludeId.HasValue)
            query = query.Where(g => g.Id != excludeId);

        return await query.AnyAsync(cancellationToken);
    }
}
