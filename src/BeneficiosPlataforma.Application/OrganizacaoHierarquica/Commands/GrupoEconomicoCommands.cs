namespace BeneficiosPlataforma.Application.OrganizacaoHierarquica.Commands;

using Common;
using Domain.OrganizacaoHierarquica;
using Domain.ValueObjects;
using MediatR;
using Microsoft.Extensions.Logging;
using Infrastructure.Persistence;
using Infrastructure.MultiTenancy;

public record CriarGrupoEconomicoCommand(
    string Nome,
    string CnpjRaiz,
    string Responsavel) : IRequest<Result<GrupoEconomicoDto>>;

public class CriarGrupoEconomicoCommandHandler(
    IGrupoEconomicoRepository repository,
    ITenantContext tenantContext,
    ILogger<CriarGrupoEconomicoCommandHandler> logger)
    : IRequestHandler<CriarGrupoEconomicoCommand, Result<GrupoEconomicoDto>>
{
    public async Task<Result<GrupoEconomicoDto>> Handle(
        CriarGrupoEconomicoCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            var cnpjRaiz = new CnpjRaiz(request.CnpjRaiz);

            var exists = await repository.ExistsByCnpjRaizAsync(cnpjRaiz, cancellationToken: cancellationToken);
            if (exists)
                return Result<GrupoEconomicoDto>.Failure("CNPJ Raiz já existe para este tenant.");

            var grupo = new GrupoEconomico(
                tenantContext.TenantId,
                request.Nome,
                cnpjRaiz,
                request.Responsavel);

            await repository.AddAsync(grupo, cancellationToken);
            await repository.SaveChangesAsync(cancellationToken);

            logger.LogInformation("Grupo Econômico criado: {GrupoId}", grupo.Id);

            return Result<GrupoEconomicoDto>.Success(MapToDto(grupo));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro ao criar grupo econômico");
            return Result<GrupoEconomicoDto>.Failure("Erro ao criar grupo econômico.");
        }
    }

    private static GrupoEconomicoDto MapToDto(GrupoEconomico grupo) => new()
    {
        Id = grupo.Id,
        Nome = grupo.Nome,
        CnpjRaiz = grupo.CnpjRaiz.Value,
        Responsavel = grupo.Responsavel,
        Status = grupo.Status,
        CreatedAt = grupo.CreatedAt,
        UpdatedAt = grupo.UpdatedAt
    };
}

public record AtualizarGrupoEconomicoCommand(
    Guid Id,
    string Nome,
    string Responsavel) : IRequest<Result<GrupoEconomicoDto>>;

public class AtualizarGrupoEconomicoCommandHandler(
    IGrupoEconomicoRepository repository,
    ILogger<AtualizarGrupoEconomicoCommandHandler> logger)
    : IRequestHandler<AtualizarGrupoEconomicoCommand, Result<GrupoEconomicoDto>>
{
    public async Task<Result<GrupoEconomicoDto>> Handle(
        AtualizarGrupoEconomicoCommand request,
        CancellationToken cancellationToken)
    {
        var grupo = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (grupo == null)
            return Result<GrupoEconomicoDto>.Failure("Grupo econômico não encontrado.");

        grupo.Atualizar(request.Nome, request.Responsavel);
        repository.Update(grupo);
        await repository.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Grupo Econômico atualizado: {GrupoId}", grupo.Id);

        return Result<GrupoEconomicoDto>.Success(MapToDto(grupo));
    }

    private static GrupoEconomicoDto MapToDto(GrupoEconomico grupo) => new()
    {
        Id = grupo.Id,
        Nome = grupo.Nome,
        CnpjRaiz = grupo.CnpjRaiz.Value,
        Responsavel = grupo.Responsavel,
        Status = grupo.Status,
        CreatedAt = grupo.CreatedAt,
        UpdatedAt = grupo.UpdatedAt
    };
}

public record ExcluirGrupoEconomicoCommand(Guid Id) : IRequest<Result<bool>>;

public class ExcluirGrupoEconomicoCommandHandler(
    IGrupoEconomicoRepository grupoRepository,
    IEstipulanteRepository estipulanteRepository,
    ILogger<ExcluirGrupoEconomicoCommandHandler> logger)
    : IRequestHandler<ExcluirGrupoEconomicoCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(
        ExcluirGrupoEconomicoCommand request,
        CancellationToken cancellationToken)
    {
        var grupo = await grupoRepository.GetByIdAsync(request.Id, cancellationToken);
        if (grupo == null)
            return Result<bool>.Failure("Grupo econômico não encontrado.");

        var estipulantes = await estipulanteRepository.GetByGrupoEconomicoAsync(request.Id, cancellationToken);
        var estipulantesAtivos = estipulantes.Where(e => e.Status == "ATIVO").ToList();

        if (estipulantesAtivos.Any())
            return Result<bool>.Failure("Não é possível excluir um grupo com estipulantes ativos.");

        grupoRepository.Delete(grupo);
        await grupoRepository.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Grupo Econômico excluído: {GrupoId}", grupo.Id);

        return Result<bool>.Success(true);
    }
}

public record AlterarStatusGrupoEconomicoCommand(
    Guid Id,
    string Status) : IRequest<Result<GrupoEconomicoDto>>;

public class AlterarStatusGrupoEconomicoCommandHandler(
    IGrupoEconomicoRepository repository,
    ILogger<AlterarStatusGrupoEconomicoCommandHandler> logger)
    : IRequestHandler<AlterarStatusGrupoEconomicoCommand, Result<GrupoEconomicoDto>>
{
    public async Task<Result<GrupoEconomicoDto>> Handle(
        AlterarStatusGrupoEconomicoCommand request,
        CancellationToken cancellationToken)
    {
        var grupo = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (grupo == null)
            return Result<GrupoEconomicoDto>.Failure("Grupo econômico não encontrado.");

        if (request.Status == "ATIVO")
            grupo.Ativar();
        else
            grupo.Inativar();

        repository.Update(grupo);
        await repository.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Status do Grupo Econômico alterado: {GrupoId} -> {Status}", grupo.Id, request.Status);

        return Result<GrupoEconomicoDto>.Success(MapToDto(grupo));
    }

    private static GrupoEconomicoDto MapToDto(GrupoEconomico grupo) => new()
    {
        Id = grupo.Id,
        Nome = grupo.Nome,
        CnpjRaiz = grupo.CnpjRaiz.Value,
        Responsavel = grupo.Responsavel,
        Status = grupo.Status,
        CreatedAt = grupo.CreatedAt,
        UpdatedAt = grupo.UpdatedAt
    };
}
