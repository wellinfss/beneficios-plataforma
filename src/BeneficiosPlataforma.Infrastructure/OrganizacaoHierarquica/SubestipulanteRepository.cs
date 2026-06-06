namespace BeneficiosPlataforma.Infrastructure.OrganizacaoHierarquica;

using BeneficiosPlataforma.Domain.OrganizacaoHierarquica;
using BeneficiosPlataforma.Domain.ValueObjects;
using BeneficiosPlataforma.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

public class SubestipulanteRepository : ISubestipulanteRepository
{
    private readonly AppDbContext _dbContext;

    public SubestipulanteRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Subestipulante?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Subestipulantes
            .FirstOrDefaultAsync(s => s.Id == id && !s.IsDeleted, cancellationToken);
    }

    public async Task AddAsync(Subestipulante entity, CancellationToken cancellationToken = default)
    {
        await _dbContext.Subestipulantes.AddAsync(entity, cancellationToken);
    }

    public void Update(Subestipulante entity)
    {
        _dbContext.Subestipulantes.Update(entity);
    }

    public void Delete(Subestipulante entity)
    {
        entity.SoftDelete();
        _dbContext.Subestipulantes.Update(entity);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<PagedResult<Subestipulante>> GetAllPagedAsync(
        Guid? estipulanteId,
        string? nome,
        string? status,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Subestipulantes.Where(s => !s.IsDeleted);

        if (estipulanteId.HasValue)
            query = query.Where(s => s.EstipulanteId == estipulanteId);

        if (!string.IsNullOrWhiteSpace(nome))
            query = query.Where(s => EF.Functions.ILike(s.RazaoSocial, $"%{nome}%") ||
                                     EF.Functions.ILike(s.NomeFantasia ?? "", $"%{nome}%"));

        if (!string.IsNullOrWhiteSpace(status))
            query = query.Where(s => s.Status == status);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(s => s.RazaoSocial)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<Subestipulante>(items, totalCount, page, pageSize);
    }

    public async Task<IEnumerable<Subestipulante>> GetByEstipulanteAsync(
        Guid estipulanteId,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Subestipulantes
            .Where(s => !s.IsDeleted && s.EstipulanteId == estipulanteId)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsByCnpjAsync(
        Cnpj cnpj,
        Guid? excludeId = null,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Subestipulantes
            .Where(s => !s.IsDeleted && s.Cnpj == cnpj);

        if (excludeId.HasValue)
            query = query.Where(s => s.Id != excludeId);

        return await query.AnyAsync(cancellationToken);
    }
}
