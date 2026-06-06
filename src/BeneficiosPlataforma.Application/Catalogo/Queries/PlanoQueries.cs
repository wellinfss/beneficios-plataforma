namespace BeneficiosPlataforma.Application.Catalogo.Queries;

using Common;
using Domain.Catalogo;
using MediatR;
using Microsoft.Extensions.Logging;

public record ListarPlanosQuery(
    string? Nome,
    Guid? OperadoraId,
    string? TipoBeneficio,
    string? Status,
    int Page,
    int PageSize) : IRequest<PagedResult<PlanoDto>>;

public class ListarPlanosQueryHandler(
    IPlanoRepository repository,
    IProdutoRepository produtoRepository,
    IOperadoraRepository operadoraRepository,
    ILogger<ListarPlanosQueryHandler> logger)
    : IRequestHandler<ListarPlanosQuery, PagedResult<PlanoDto>>
{
    public async Task<PagedResult<PlanoDto>> Handle(
        ListarPlanosQuery request,
        CancellationToken cancellationToken)
    {
        var result = await repository.GetAllPagedAsync(
            request.Nome,
            request.OperadoraId,
            request.TipoBeneficio,
            request.Status,
            request.Page,
            request.PageSize,
            cancellationToken);

        var dtos = new List<PlanoDto>();
        foreach (var plano in result.Items)
        {
            var produto = await produtoRepository.GetByIdAsync(plano.ProdutoId, cancellationToken);
            var operadora = produto != null
                ? await operadoraRepository.GetByIdAsync(produto.OperadoraId, cancellationToken)
                : null;

            dtos.Add(new PlanoDto
            {
                Id = plano.Id,
                Nome = plano.Nome,
                ProdutoId = plano.ProdutoId,
                ProdutoNome = produto?.Nome,
                OperadoraId = operadora?.Id,
                OperadoraNome = operadora?.RazaoSocial,
                Cobertura = plano.Cobertura,
                AbrangenciaGeografica = plano.AbrangenciaGeografica,
                TipoAcomodacao = plano.TipoAcomodacao,
                ValorReferencia = plano.ValorReferencia,
                Status = plano.Status,
                CreatedAt = plano.CreatedAt,
                UpdatedAt = plano.UpdatedAt
            });
        }

        logger.LogInformation("Planos listados: {Count} de {Total}", dtos.Count, result.TotalCount);

        return new PagedResult<PlanoDto>(dtos, result.TotalCount, result.PageNumber, result.PageSize);
    }
}

public record ObterPlanoQuery(Guid Id) : IRequest<Result<PlanoDto>>;

public class ObterPlanoQueryHandler(
    IPlanoRepository planoRepository,
    IProdutoRepository produtoRepository,
    IOperadoraRepository operadoraRepository,
    ILogger<ObterPlanoQueryHandler> logger)
    : IRequestHandler<ObterPlanoQuery, Result<PlanoDto>>
{
    public async Task<Result<PlanoDto>> Handle(
        ObterPlanoQuery request,
        CancellationToken cancellationToken)
    {
        var plano = await planoRepository.GetByIdAsync(request.Id, cancellationToken);
        if (plano == null)
            return Result<PlanoDto>.Failure("Plano não encontrado.");

        var produto = await produtoRepository.GetByIdAsync(plano.ProdutoId, cancellationToken);
        var operadora = produto != null
            ? await operadoraRepository.GetByIdAsync(produto.OperadoraId, cancellationToken)
            : null;

        var dto = new PlanoDto
        {
            Id = plano.Id,
            Nome = plano.Nome,
            ProdutoId = plano.ProdutoId,
            ProdutoNome = produto?.Nome,
            OperadoraId = operadora?.Id,
            OperadoraNome = operadora?.RazaoSocial,
            Cobertura = plano.Cobertura,
            AbrangenciaGeografica = plano.AbrangenciaGeografica,
            TipoAcomodacao = plano.TipoAcomodacao,
            ValorReferencia = plano.ValorReferencia,
            Status = plano.Status,
            CreatedAt = plano.CreatedAt,
            UpdatedAt = plano.UpdatedAt
        };

        logger.LogInformation("Plano obtido: {PlanoId}", plano.Id);

        return Result<PlanoDto>.Success(dto);
    }
}
