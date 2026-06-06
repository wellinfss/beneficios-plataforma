namespace BeneficiosPlataforma.Application.Catalogo.Commands;

using Common;
using Domain.Catalogo;
using MediatR;
using Microsoft.Extensions.Logging;
using Infrastructure.MultiTenancy;

public record CriarPlanoCommand(
    string Nome,
    Guid ProdutoId,
    string? Cobertura = null,
    string? AbrangenciaGeografica = null,
    string? TipoAcomodacao = null,
    decimal? ValorReferencia = null) : IRequest<Result<PlanoDto>>;

public class CriarPlanoCommandHandler(
    IPlanoRepository planoRepository,
    IProdutoRepository produtoRepository,
    IOperadoraRepository operadoraRepository,
    ITenantContext tenantContext,
    ILogger<CriarPlanoCommandHandler> logger)
    : IRequestHandler<CriarPlanoCommand, Result<PlanoDto>>
{
    public async Task<Result<PlanoDto>> Handle(
        CriarPlanoCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            var produto = await produtoRepository.GetByIdAsync(request.ProdutoId, cancellationToken);
            if (produto == null)
                return Result<PlanoDto>.Failure("Produto não encontrado.");

            if (produto.Status != "ATIVO")
                return Result<PlanoDto>.Failure("Produto deve estar ativo para adicionar planos.");

            var operadora = await operadoraRepository.GetByIdAsync(produto.OperadoraId, cancellationToken);

            var plano = new Plano(
                tenantContext.TenantId,
                request.Nome,
                request.ProdutoId,
                request.Cobertura,
                request.AbrangenciaGeografica,
                request.TipoAcomodacao,
                request.ValorReferencia);

            await planoRepository.AddAsync(plano, cancellationToken);
            await planoRepository.SaveChangesAsync(cancellationToken);

            logger.LogInformation("Plano criado: {PlanoId}", plano.Id);

            return Result<PlanoDto>.Success(MapToDto(plano, produto, operadora));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro ao criar plano");
            return Result<PlanoDto>.Failure("Erro ao criar plano.");
        }
    }

    private static PlanoDto MapToDto(Plano plano, Produto produto, Operadora? operadora) => new()
    {
        Id = plano.Id,
        Nome = plano.Nome,
        ProdutoId = plano.ProdutoId,
        ProdutoNome = produto.Nome,
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
}

public record AtualizarPlanoCommand(
    Guid Id,
    string Nome,
    string? Cobertura = null,
    string? AbrangenciaGeografica = null,
    string? TipoAcomodacao = null,
    decimal? ValorReferencia = null) : IRequest<Result<PlanoDto>>;

public class AtualizarPlanoCommandHandler(
    IPlanoRepository planoRepository,
    IProdutoRepository produtoRepository,
    IOperadoraRepository operadoraRepository,
    ILogger<AtualizarPlanoCommandHandler> logger)
    : IRequestHandler<AtualizarPlanoCommand, Result<PlanoDto>>
{
    public async Task<Result<PlanoDto>> Handle(
        AtualizarPlanoCommand request,
        CancellationToken cancellationToken)
    {
        var plano = await planoRepository.GetByIdAsync(request.Id, cancellationToken);
        if (plano == null)
            return Result<PlanoDto>.Failure("Plano não encontrado.");

        var produto = await produtoRepository.GetByIdAsync(plano.ProdutoId, cancellationToken);
        if (produto == null)
            return Result<PlanoDto>.Failure("Produto não encontrado.");

        var operadora = await operadoraRepository.GetByIdAsync(produto.OperadoraId, cancellationToken);

        plano.Atualizar(request.Nome, request.Cobertura, request.AbrangenciaGeografica, request.TipoAcomodacao, request.ValorReferencia);
        planoRepository.Update(plano);
        await planoRepository.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Plano atualizado: {PlanoId}", plano.Id);

        return Result<PlanoDto>.Success(MapToDto(plano, produto, operadora));
    }

    private static PlanoDto MapToDto(Plano plano, Produto produto, Operadora? operadora) => new()
    {
        Id = plano.Id,
        Nome = plano.Nome,
        ProdutoId = plano.ProdutoId,
        ProdutoNome = produto.Nome,
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
}

public record ExcluirPlanoCommand(Guid Id) : IRequest<Result<bool>>;

public class ExcluirPlanoCommandHandler(
    IPlanoRepository planoRepository,
    ILogger<ExcluirPlanoCommandHandler> logger)
    : IRequestHandler<ExcluirPlanoCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(
        ExcluirPlanoCommand request,
        CancellationToken cancellationToken)
    {
        var plano = await planoRepository.GetByIdAsync(request.Id, cancellationToken);
        if (plano == null)
            return Result<bool>.Failure("Plano não encontrado.");

        planoRepository.Delete(plano);
        await planoRepository.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Plano excluído: {PlanoId}", plano.Id);

        return Result<bool>.Success(true);
    }
}

public record AlterarStatusPlanoCommand(
    Guid Id,
    string Status) : IRequest<Result<PlanoDto>>;

public class AlterarStatusPlanoCommandHandler(
    IPlanoRepository planoRepository,
    IProdutoRepository produtoRepository,
    IOperadoraRepository operadoraRepository,
    ILogger<AlterarStatusPlanoCommandHandler> logger)
    : IRequestHandler<AlterarStatusPlanoCommand, Result<PlanoDto>>
{
    public async Task<Result<PlanoDto>> Handle(
        AlterarStatusPlanoCommand request,
        CancellationToken cancellationToken)
    {
        var plano = await planoRepository.GetByIdAsync(request.Id, cancellationToken);
        if (plano == null)
            return Result<PlanoDto>.Failure("Plano não encontrado.");

        var produto = await produtoRepository.GetByIdAsync(plano.ProdutoId, cancellationToken);
        if (produto == null)
            return Result<PlanoDto>.Failure("Produto não encontrado.");

        var operadora = await operadoraRepository.GetByIdAsync(produto.OperadoraId, cancellationToken);

        if (request.Status == "ATIVO")
            plano.Ativar();
        else
            plano.Inativar();

        planoRepository.Update(plano);
        await planoRepository.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Status do Plano alterado: {PlanoId} -> {Status}", plano.Id, request.Status);

        return Result<PlanoDto>.Success(MapToDto(plano, produto, operadora));
    }

    private static PlanoDto MapToDto(Plano plano, Produto produto, Operadora? operadora) => new()
    {
        Id = plano.Id,
        Nome = plano.Nome,
        ProdutoId = plano.ProdutoId,
        ProdutoNome = produto.Nome,
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
}
