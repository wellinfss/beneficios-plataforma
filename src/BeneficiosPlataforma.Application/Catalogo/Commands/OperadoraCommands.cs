namespace BeneficiosPlataforma.Application.Catalogo.Commands;

using Common;
using Domain.Catalogo;
using Domain.ValueObjects;
using MediatR;
using Microsoft.Extensions.Logging;
using Infrastructure.MultiTenancy;

public record CriarOperadoraCommand(
    string RazaoSocial,
    string Cnpj,
    string Tipo,
    string? RegistroAns = null) : IRequest<Result<OperadoraDto>>;

public class CriarOperadoraCommandHandler(
    IOperadoraRepository repository,
    ITenantContext tenantContext,
    ILogger<CriarOperadoraCommandHandler> logger)
    : IRequestHandler<CriarOperadoraCommand, Result<OperadoraDto>>
{
    public async Task<Result<OperadoraDto>> Handle(
        CriarOperadoraCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            var cnpj = new Cnpj(request.Cnpj);

            var exists = await repository.ExistsByCnpjAsync(cnpj, cancellationToken: cancellationToken);
            if (exists)
                return Result<OperadoraDto>.Failure("CNPJ já existe para este tenant.");

            RegistroAns? registroAns = null;
            if (!string.IsNullOrWhiteSpace(request.RegistroAns))
                registroAns = new RegistroAns(request.RegistroAns);

            var operadora = new Operadora(
                tenantContext.TenantId,
                request.RazaoSocial,
                cnpj,
                request.Tipo,
                registroAns);

            await repository.AddAsync(operadora, cancellationToken);
            await repository.SaveChangesAsync(cancellationToken);

            logger.LogInformation("Operadora criada: {OperadoraId}", operadora.Id);

            return Result<OperadoraDto>.Success(MapToDto(operadora));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro ao criar operadora");
            return Result<OperadoraDto>.Failure("Erro ao criar operadora.");
        }
    }

    private static OperadoraDto MapToDto(Operadora operadora) => new()
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
}

public record AtualizarOperadoraCommand(
    Guid Id,
    string RazaoSocial,
    string Tipo,
    string? RegistroAns = null) : IRequest<Result<OperadoraDto>>;

public class AtualizarOperadoraCommandHandler(
    IOperadoraRepository repository,
    ILogger<AtualizarOperadoraCommandHandler> logger)
    : IRequestHandler<AtualizarOperadoraCommand, Result<OperadoraDto>>
{
    public async Task<Result<OperadoraDto>> Handle(
        AtualizarOperadoraCommand request,
        CancellationToken cancellationToken)
    {
        var operadora = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (operadora == null)
            return Result<OperadoraDto>.Failure("Operadora não encontrada.");

        RegistroAns? registroAns = null;
        if (!string.IsNullOrWhiteSpace(request.RegistroAns))
            registroAns = new RegistroAns(request.RegistroAns);

        operadora.Atualizar(request.RazaoSocial, request.Tipo, registroAns);
        repository.Update(operadora);
        await repository.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Operadora atualizada: {OperadoraId}", operadora.Id);

        return Result<OperadoraDto>.Success(MapToDto(operadora));
    }

    private static OperadoraDto MapToDto(Operadora operadora) => new()
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
}

public record ExcluirOperadoraCommand(Guid Id) : IRequest<Result<bool>>;

public class ExcluirOperadoraCommandHandler(
    IOperadoraRepository operadoraRepository,
    IProdutoRepository produtoRepository,
    ILogger<ExcluirOperadoraCommandHandler> logger)
    : IRequestHandler<ExcluirOperadoraCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(
        ExcluirOperadoraCommand request,
        CancellationToken cancellationToken)
    {
        var operadora = await operadoraRepository.GetByIdAsync(request.Id, cancellationToken);
        if (operadora == null)
            return Result<bool>.Failure("Operadora não encontrada.");

        var produtos = await produtoRepository.GetByOperadoraAsync(request.Id, cancellationToken);
        var produtosAtivos = produtos.Where(p => p.Status == "ATIVO").ToList();

        if (produtosAtivos.Any())
            return Result<bool>.Failure("Não é possível excluir uma operadora com produtos ativos.");

        operadoraRepository.Delete(operadora);
        await operadoraRepository.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Operadora excluída: {OperadoraId}", operadora.Id);

        return Result<bool>.Success(true);
    }
}

public record AlterarStatusOperadoraCommand(
    Guid Id,
    string Status) : IRequest<Result<OperadoraDto>>;

public class AlterarStatusOperadoraCommandHandler(
    IOperadoraRepository repository,
    ILogger<AlterarStatusOperadoraCommandHandler> logger)
    : IRequestHandler<AlterarStatusOperadoraCommand, Result<OperadoraDto>>
{
    public async Task<Result<OperadoraDto>> Handle(
        AlterarStatusOperadoraCommand request,
        CancellationToken cancellationToken)
    {
        var operadora = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (operadora == null)
            return Result<OperadoraDto>.Failure("Operadora não encontrada.");

        if (request.Status == "ATIVO")
            operadora.Ativar();
        else
            operadora.Inativar();

        repository.Update(operadora);
        await repository.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Status da Operadora alterado: {OperadoraId} -> {Status}", operadora.Id, request.Status);

        return Result<OperadoraDto>.Success(MapToDto(operadora));
    }

    private static OperadoraDto MapToDto(Operadora operadora) => new()
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
}

public record AtualizarIntegracaoOperadoraCommand(
    Guid Id,
    string? EndpointIntegracao,
    string? FormatoIntegracao,
    string? CredenciaisPlanoTexto) : IRequest<Result<OperadoraDto>>;

public class AtualizarIntegracaoOperadoraCommandHandler(
    IOperadoraRepository repository,
    IEncryptionService encryptionService,
    ILogger<AtualizarIntegracaoOperadoraCommandHandler> logger)
    : IRequestHandler<AtualizarIntegracaoOperadoraCommand, Result<OperadoraDto>>
{
    public async Task<Result<OperadoraDto>> Handle(
        AtualizarIntegracaoOperadoraCommand request,
        CancellationToken cancellationToken)
    {
        var operadora = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (operadora == null)
            return Result<OperadoraDto>.Failure("Operadora não encontrada.");

        string? credenciaisEncriptadas = null;
        if (!string.IsNullOrWhiteSpace(request.CredenciaisPlanoTexto))
            credenciaisEncriptadas = encryptionService.Encrypt(request.CredenciaisPlanoTexto);

        operadora.AtualizarIntegracao(request.EndpointIntegracao, request.FormatoIntegracao, credenciaisEncriptadas);
        repository.Update(operadora);
        await repository.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Integração da Operadora atualizada: {OperadoraId}", operadora.Id);

        return Result<OperadoraDto>.Success(MapToDto(operadora));
    }

    private static OperadoraDto MapToDto(Operadora operadora) => new()
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
}
