namespace BeneficiosPlataforma.Application.Catalogo.Queries;

using Common;
using Domain.Catalogo;
using MediatR;
using Microsoft.Extensions.Logging;

public record ListarProdutosQuery(
    string? Nome,
    Guid? OperadoraId,
    string? TipoBeneficio,
    string? Status,
    int Page,
    int PageSize) : IRequest<PagedResult<ProdutoDto>>;

public class ListarProdutosQueryHandler(
    IProdutoRepository repository,
    IOperadoraRepository operadoraRepository,
    ILogger<ListarProdutosQueryHandler> logger)
    : IRequestHandler<ListarProdutosQuery, PagedResult<ProdutoDto>>
{
    public async Task<PagedResult<ProdutoDto>> Handle(
        ListarProdutosQuery request,
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

        var dtos = new List<ProdutoDto>();
        foreach (var produto in result.Items)
        {
            var operadora = await operadoraRepository.GetByIdAsync(produto.OperadoraId, cancellationToken);
            dtos.Add(new ProdutoDto
            {
                Id = produto.Id,
                Nome = produto.Nome,
                OperadoraId = produto.OperadoraId,
                OperadoraNome = operadora?.RazaoSocial,
                TipoBeneficio = produto.TipoBeneficio,
                Modalidade = produto.Modalidade,
                RegistroAnsProduto = produto.RegistroAnsProduto,
                Status = produto.Status,
                CreatedAt = produto.CreatedAt,
                UpdatedAt = produto.UpdatedAt
            });
        }

        logger.LogInformation("Produtos listados: {Count} de {Total}", dtos.Count, result.TotalCount);

        return new PagedResult<ProdutoDto>(dtos, result.TotalCount, result.PageNumber, result.PageSize);
    }
}

public record ObterProdutoQuery(Guid Id) : IRequest<Result<ProdutoDto>>;

public class ObterProdutoQueryHandler(
    IProdutoRepository produtoRepository,
    IOperadoraRepository operadoraRepository,
    ILogger<ObterProdutoQueryHandler> logger)
    : IRequestHandler<ObterProdutoQuery, Result<ProdutoDto>>
{
    public async Task<Result<ProdutoDto>> Handle(
        ObterProdutoQuery request,
        CancellationToken cancellationToken)
    {
        var produto = await produtoRepository.GetByIdAsync(request.Id, cancellationToken);
        if (produto == null)
            return Result<ProdutoDto>.Failure("Produto não encontrado.");

        var operadora = await operadoraRepository.GetByIdAsync(produto.OperadoraId, cancellationToken);

        var dto = new ProdutoDto
        {
            Id = produto.Id,
            Nome = produto.Nome,
            OperadoraId = produto.OperadoraId,
            OperadoraNome = operadora?.RazaoSocial,
            TipoBeneficio = produto.TipoBeneficio,
            Modalidade = produto.Modalidade,
            RegistroAnsProduto = produto.RegistroAnsProduto,
            Status = produto.Status,
            CreatedAt = produto.CreatedAt,
            UpdatedAt = produto.UpdatedAt
        };

        logger.LogInformation("Produto obtido: {ProdutoId}", produto.Id);

        return Result<ProdutoDto>.Success(dto);
    }
}

public record ListarPlanosPorProdutoQuery(Guid ProdutoId) : IRequest<IEnumerable<PlanoDto>>;

public class ListarPlanosPorProdutoQueryHandler(
    IPlanoRepository planoRepository,
    IProdutoRepository produtoRepository,
    IOperadoraRepository operadoraRepository,
    ILogger<ListarPlanosPorProdutoQueryHandler> logger)
    : IRequestHandler<ListarPlanosPorProdutoQuery, IEnumerable<PlanoDto>>
{
    public async Task<IEnumerable<PlanoDto>> Handle(
        ListarPlanosPorProdutoQuery request,
        CancellationToken cancellationToken)
    {
        var produto = await produtoRepository.GetByIdAsync(request.ProdutoId, cancellationToken);
        if (produto == null)
            return Enumerable.Empty<PlanoDto>();

        var operadora = await operadoraRepository.GetByIdAsync(produto.OperadoraId, cancellationToken);
        var planos = await planoRepository.GetByProdutoAsync(request.ProdutoId, cancellationToken);

        var dtos = planos
            .Select(p => new PlanoDto
            {
                Id = p.Id,
                Nome = p.Nome,
                ProdutoId = p.ProdutoId,
                ProdutoNome = produto.Nome,
                OperadoraId = operadora?.Id,
                OperadoraNome = operadora?.RazaoSocial,
                Cobertura = p.Cobertura,
                AbrangenciaGeografica = p.AbrangenciaGeografica,
                TipoAcomodacao = p.TipoAcomodacao,
                ValorReferencia = p.ValorReferencia,
                Status = p.Status,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt
            })
            .ToList();

        logger.LogInformation("Planos listados para produto: {ProdutoId}, {Count} planos", request.ProdutoId, dtos.Count);

        return dtos;
    }
}
