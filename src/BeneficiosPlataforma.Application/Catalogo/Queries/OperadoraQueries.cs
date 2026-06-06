namespace BeneficiosPlataforma.Application.Catalogo.Queries;

using Common;
using Domain.Catalogo;
using MediatR;
using Microsoft.Extensions.Logging;

public record ListarOperadorasQuery(
    string? RazaoSocial,
    string? Tipo,
    string? Status,
    int Page,
    int PageSize) : IRequest<PagedResult<OperadoraDto>>;

public class ListarOperadorasQueryHandler(
    IOperadoraRepository repository,
    ILogger<ListarOperadorasQueryHandler> logger)
    : IRequestHandler<ListarOperadorasQuery, PagedResult<OperadoraDto>>
{
    public async Task<PagedResult<OperadoraDto>> Handle(
        ListarOperadorasQuery request,
        CancellationToken cancellationToken)
    {
        var result = await repository.GetAllPagedAsync(
            request.RazaoSocial,
            request.Tipo,
            request.Status,
            request.Page,
            request.PageSize,
            cancellationToken);

        var dtos = result.Items
            .Select(o => new OperadoraDto
            {
                Id = o.Id,
                RazaoSocial = o.RazaoSocial,
                Cnpj = o.Cnpj.Value,
                RegistroAns = o.RegistroAns?.Value,
                Tipo = o.Tipo,
                Status = o.Status,
                EndpointIntegracao = o.EndpointIntegracao,
                FormatoIntegracao = o.FormatoIntegracao,
                CreatedAt = o.CreatedAt,
                UpdatedAt = o.UpdatedAt
            })
            .ToList();

        logger.LogInformation("Operadoras listadas: {Count} de {Total}", dtos.Count, result.TotalCount);

        return new PagedResult<OperadoraDto>(dtos, result.TotalCount, result.PageNumber, result.PageSize);
    }
}

public record ObterOperadoraQuery(Guid Id) : IRequest<Result<OperadoraDto>>;

public class ObterOperadoraQueryHandler(
    IOperadoraRepository repository,
    ILogger<ObterOperadoraQueryHandler> logger)
    : IRequestHandler<ObterOperadoraQuery, Result<OperadoraDto>>
{
    public async Task<Result<OperadoraDto>> Handle(
        ObterOperadoraQuery request,
        CancellationToken cancellationToken)
    {
        var operadora = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (operadora == null)
            return Result<OperadoraDto>.Failure("Operadora não encontrada.");

        var dto = new OperadoraDto
        {
            Id = operadora.Id,
            RazaoSocial = operadora.RazaoSocial,
            Cnpj = operadora.Cnpj.Value,
            RegistroAns = operadora.RegistroAns?.Value,
            Tipo = operadora.Tipo,
            Status = operadora.Status,
            EndpointIntegracao = operadora.EndpointIntegracao,
            FormatoIntegracao = operadora.FormatoIntegracao,
            CreatedAt = operadora.CreatedAt,
            UpdatedAt = operadora.UpdatedAt
        };

        logger.LogInformation("Operadora obtida: {OperadoraId}", operadora.Id);

        return Result<OperadoraDto>.Success(dto);
    }
}

public record ListarProdutosPorOperadoraQuery(Guid OperadoraId) : IRequest<IEnumerable<ProdutoDto>>;

public class ListarProdutosPorOperadoraQueryHandler(
    IProdutoRepository produtoRepository,
    IOperadoraRepository operadoraRepository,
    ILogger<ListarProdutosPorOperadoraQueryHandler> logger)
    : IRequestHandler<ListarProdutosPorOperadoraQuery, IEnumerable<ProdutoDto>>
{
    public async Task<IEnumerable<ProdutoDto>> Handle(
        ListarProdutosPorOperadoraQuery request,
        CancellationToken cancellationToken)
    {
        var operadora = await operadoraRepository.GetByIdAsync(request.OperadoraId, cancellationToken);
        if (operadora == null)
            return Enumerable.Empty<ProdutoDto>();

        var produtos = await produtoRepository.GetByOperadoraAsync(request.OperadoraId, cancellationToken);

        var dtos = produtos
            .Select(p => new ProdutoDto
            {
                Id = p.Id,
                Nome = p.Nome,
                OperadoraId = p.OperadoraId,
                OperadoraNome = operadora.RazaoSocial,
                TipoBeneficio = p.TipoBeneficio,
                Modalidade = p.Modalidade,
                RegistroAnsProduto = p.RegistroAnsProduto,
                Status = p.Status,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt
            })
            .ToList();

        logger.LogInformation("Produtos listados para operadora: {OperadoraId}, {Count} produtos", request.OperadoraId, dtos.Count);

        return dtos;
    }
}
