namespace BeneficiosPlataforma.Application.OrganizacaoHierarquica.Queries;

using Common;
using Domain.OrganizacaoHierarquica;
using MediatR;
using Microsoft.Extensions.Logging;

public record ListarGruposEconomicosQuery(
    string? Nome,
    string? Status,
    int Page,
    int PageSize) : IRequest<PagedResult<GrupoEconomicoDto>>;

public class ListarGruposEconomicosQueryHandler(
    IGrupoEconomicoRepository repository,
    ILogger<ListarGruposEconomicosQueryHandler> logger)
    : IRequestHandler<ListarGruposEconomicosQuery, PagedResult<GrupoEconomicoDto>>
{
    public async Task<PagedResult<GrupoEconomicoDto>> Handle(
        ListarGruposEconomicosQuery request,
        CancellationToken cancellationToken)
    {
        var result = await repository.GetAllPagedAsync(
            request.Nome,
            request.Status,
            request.Page,
            request.PageSize,
            cancellationToken);

        var dtos = result.Items
            .Select(g => new GrupoEconomicoDto
            {
                Id = g.Id,
                Nome = g.Nome,
                CnpjRaiz = g.CnpjRaiz.Value,
                Responsavel = g.Responsavel,
                Status = g.Status,
                CreatedAt = g.CreatedAt,
                UpdatedAt = g.UpdatedAt
            })
            .ToList();

        logger.LogInformation("Grupos econômicos listados: {Count} de {Total}", dtos.Count, result.TotalCount);

        return new PagedResult<GrupoEconomicoDto>(dtos, result.TotalCount, result.PageNumber, result.PageSize);
    }
}

public record ObterGrupoEconomicoQuery(Guid Id) : IRequest<Result<GrupoEconomicoDto>>;

public class ObterGrupoEconomicoQueryHandler(
    IGrupoEconomicoRepository repository,
    ILogger<ObterGrupoEconomicoQueryHandler> logger)
    : IRequestHandler<ObterGrupoEconomicoQuery, Result<GrupoEconomicoDto>>
{
    public async Task<Result<GrupoEconomicoDto>> Handle(
        ObterGrupoEconomicoQuery request,
        CancellationToken cancellationToken)
    {
        var grupo = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (grupo == null)
            return Result<GrupoEconomicoDto>.Failure("Grupo econômico não encontrado.");

        var dto = new GrupoEconomicoDto
        {
            Id = grupo.Id,
            Nome = grupo.Nome,
            CnpjRaiz = grupo.CnpjRaiz.Value,
            Responsavel = grupo.Responsavel,
            Status = grupo.Status,
            CreatedAt = grupo.CreatedAt,
            UpdatedAt = grupo.UpdatedAt
        };

        logger.LogInformation("Grupo econômico obtido: {GrupoId}", grupo.Id);

        return Result<GrupoEconomicoDto>.Success(dto);
    }
}
