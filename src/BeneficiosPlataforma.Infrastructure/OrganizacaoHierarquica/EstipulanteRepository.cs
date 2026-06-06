namespace BeneficiosPlataforma.Infrastructure.OrganizacaoHierarquica;

using BeneficiosPlataforma.Domain.OrganizacaoHierarquica;
using BeneficiosPlataforma.Domain.ValueObjects;
using BeneficiosPlataforma.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

public class EstipulanteRepository : IEstipulanteRepository
{
    private readonly AppDbContext _dbContext;

    public EstipulanteRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Estipulante?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Estipulantes
            .FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted, cancellationToken);
    }

    public async Task AddAsync(Estipulante entity, CancellationToken cancellationToken = default)
    {
        await _dbContext.Estipulantes.AddAsync(entity, cancellationToken);
    }

    public void Update(Estipulante entity)
    {
        _dbContext.Estipulantes.Update(entity);
    }

    public void Delete(Estipulante entity)
    {
        entity.SoftDelete();
        _dbContext.Estipulantes.Update(entity);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<PagedResult<Estipulante>> GetAllPagedAsync(
        string? nome,
        string? cnpj,
        Guid? grupoEconomicoId,
        string? status,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Estipulantes.Where(e => !e.IsDeleted);

        if (!string.IsNullOrWhiteSpace(nome))
            query = query.Where(e => EF.Functions.ILike(e.RazaoSocial, $"%{nome}%") ||
                                     EF.Functions.ILike(e.NomeFantasia ?? "", $"%{nome}%"));

        if (!string.IsNullOrWhiteSpace(cnpj))
            query = query.Where(e => e.Cnpj.Value.Contains(cnpj.Replace(".", "").Replace("-", "").Replace("/", "")));

        if (grupoEconomicoId.HasValue)
            query = query.Where(e => e.GrupoEconomicoId == grupoEconomicoId);

        if (!string.IsNullOrWhiteSpace(status))
            query = query.Where(e => e.Status == status);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(e => e.RazaoSocial)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<Estipulante>(items, totalCount, page, pageSize);
    }

    public async Task<bool> ExistsByCnpjAsync(
        Cnpj cnpj,
        Guid? excludeId = null,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Estipulantes
            .Where(e => !e.IsDeleted && e.Cnpj == cnpj);

        if (excludeId.HasValue)
            query = query.Where(e => e.Id != excludeId);

        return await query.AnyAsync(cancellationToken);
    }

    public async Task<IEnumerable<Estipulante>> GetByGrupoEconomicoAsync(
        Guid grupoEconomicoId,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Estipulantes
            .Where(e => !e.IsDeleted && e.GrupoEconomicoId == grupoEconomicoId)
            .ToListAsync(cancellationToken);
    }
}
