namespace BeneficiosPlataforma.Application.Catalogo.Commands;

using Common;
using Domain.Catalogo;
using MediatR;
using Microsoft.Extensions.Logging;
using Infrastructure.MultiTenancy;

public record CriarProdutoCommand(
    string Nome,
    Guid OperadoraId,
    string TipoBeneficio,
    string Modalidade,
    string? RegistroAnsProduto = null) : IRequest<Result<ProdutoDto>>;

public class CriarProdutoCommandHandler(
    IProdutoRepository produtoRepository,
    IOperadoraRepository operadoraRepository,
    ITenantContext tenantContext,
    ILogger<CriarProdutoCommandHandler> logger)
    : IRequestHandler<CriarProdutoCommand, Result<ProdutoDto>>
{
    public async Task<Result<ProdutoDto>> Handle(
        CriarProdutoCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            var operadora = await operadoraRepository.GetByIdAsync(request.OperadoraId, cancellationToken);
            if (operadora == null)
                return Result<ProdutoDto>.Failure("Operadora não encontrada.");

            if (operadora.Status != "ATIVO")
                return Result<ProdutoDto>.Failure("Operadora deve estar ativa para adicionar produtos.");

            var produto = new Produto(
                tenantContext.TenantId,
                request.Nome,
                request.OperadoraId,
                request.TipoBeneficio,
                request.Modalidade,
                request.RegistroAnsProduto);

            await produtoRepository.AddAsync(produto, cancellationToken);
            await produtoRepository.SaveChangesAsync(cancellationToken);

            logger.LogInformation("Produto criado: {ProdutoId}", produto.Id);

            return Result<ProdutoDto>.Success(MapToDto(produto, operadora));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro ao criar produto");
            return Result<ProdutoDto>.Failure("Erro ao criar produto.");
        }
    }

    private static ProdutoDto MapToDto(Produto produto, Operadora operadora) => new()
    {
        Id = produto.Id,
        Nome = produto.Nome,
        OperadoraId = produto.OperadoraId,
        OperadoraNome = operadora.RazaoSocial,
        TipoBeneficio = produto.TipoBeneficio,
        Modalidade = produto.Modalidade,
        RegistroAnsProduto = produto.RegistroAnsProduto,
        Status = produto.Status,
        CreatedAt = produto.CreatedAt,
        UpdatedAt = produto.UpdatedAt
    };
}

public record AtualizarProdutoCommand(
    Guid Id,
    string Nome,
    string TipoBeneficio,
    string Modalidade,
    string? RegistroAnsProduto = null) : IRequest<Result<ProdutoDto>>;

public class AtualizarProdutoCommandHandler(
    IProdutoRepository produtoRepository,
    IOperadoraRepository operadoraRepository,
    ILogger<AtualizarProdutoCommandHandler> logger)
    : IRequestHandler<AtualizarProdutoCommand, Result<ProdutoDto>>
{
    public async Task<Result<ProdutoDto>> Handle(
        AtualizarProdutoCommand request,
        CancellationToken cancellationToken)
    {
        var produto = await produtoRepository.GetByIdAsync(request.Id, cancellationToken);
        if (produto == null)
            return Result<ProdutoDto>.Failure("Produto não encontrado.");

        var operadora = await operadoraRepository.GetByIdAsync(produto.OperadoraId, cancellationToken);
        if (operadora == null)
            return Result<ProdutoDto>.Failure("Operadora não encontrada.");

        produto.Atualizar(request.Nome, request.TipoBeneficio, request.Modalidade, request.RegistroAnsProduto);
        produtoRepository.Update(produto);
        await produtoRepository.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Produto atualizado: {ProdutoId}", produto.Id);

        return Result<ProdutoDto>.Success(MapToDto(produto, operadora));
    }

    private static ProdutoDto MapToDto(Produto produto, Operadora operadora) => new()
    {
        Id = produto.Id,
        Nome = produto.Nome,
        OperadoraId = produto.OperadoraId,
        OperadoraNome = operadora.RazaoSocial,
        TipoBeneficio = produto.TipoBeneficio,
        Modalidade = produto.Modalidade,
        RegistroAnsProduto = produto.RegistroAnsProduto,
        Status = produto.Status,
        CreatedAt = produto.CreatedAt,
        UpdatedAt = produto.UpdatedAt
    };
}

public record ExcluirProdutoCommand(Guid Id) : IRequest<Result<bool>>;

public class ExcluirProdutoCommandHandler(
    IProdutoRepository produtoRepository,
    IPlanoRepository planoRepository,
    ILogger<ExcluirProdutoCommandHandler> logger)
    : IRequestHandler<ExcluirProdutoCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(
        ExcluirProdutoCommand request,
        CancellationToken cancellationToken)
    {
        var produto = await produtoRepository.GetByIdAsync(request.Id, cancellationToken);
        if (produto == null)
            return Result<bool>.Failure("Produto não encontrado.");

        var planos = await planoRepository.GetByProdutoAsync(request.Id, cancellationToken);
        var planosAtivos = planos.Where(p => p.Status == "ATIVO").ToList();

        if (planosAtivos.Any())
            return Result<bool>.Failure("Não é possível excluir um produto com planos ativos.");

        produtoRepository.Delete(produto);
        await produtoRepository.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Produto excluído: {ProdutoId}", produto.Id);

        return Result<bool>.Success(true);
    }
}

public record AlterarStatusProdutoCommand(
    Guid Id,
    string Status) : IRequest<Result<ProdutoDto>>;

public class AlterarStatusProdutoCommandHandler(
    IProdutoRepository produtoRepository,
    IOperadoraRepository operadoraRepository,
    ILogger<AlterarStatusProdutoCommandHandler> logger)
    : IRequestHandler<AlterarStatusProdutoCommand, Result<ProdutoDto>>
{
    public async Task<Result<ProdutoDto>> Handle(
        AlterarStatusProdutoCommand request,
        CancellationToken cancellationToken)
    {
        var produto = await produtoRepository.GetByIdAsync(request.Id, cancellationToken);
        if (produto == null)
            return Result<ProdutoDto>.Failure("Produto não encontrado.");

        var operadora = await operadoraRepository.GetByIdAsync(produto.OperadoraId, cancellationToken);
        if (operadora == null)
            return Result<ProdutoDto>.Failure("Operadora não encontrada.");

        if (request.Status == "ATIVO")
            produto.Ativar();
        else
            produto.Inativar();

        produtoRepository.Update(produto);
        await produtoRepository.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Status do Produto alterado: {ProdutoId} -> {Status}", produto.Id, request.Status);

        return Result<ProdutoDto>.Success(MapToDto(produto, operadora));
    }

    private static ProdutoDto MapToDto(Produto produto, Operadora operadora) => new()
    {
        Id = produto.Id,
        Nome = produto.Nome,
        OperadoraId = produto.OperadoraId,
        OperadoraNome = operadora.RazaoSocial,
        TipoBeneficio = produto.TipoBeneficio,
        Modalidade = produto.Modalidade,
        RegistroAnsProduto = produto.RegistroAnsProduto,
        Status = produto.Status,
        CreatedAt = produto.CreatedAt,
        UpdatedAt = produto.UpdatedAt
    };
}
