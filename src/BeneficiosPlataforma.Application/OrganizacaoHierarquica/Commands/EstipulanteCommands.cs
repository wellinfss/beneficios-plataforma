namespace BeneficiosPlataforma.Application.OrganizacaoHierarquica.Commands;

using Common;
using Domain.OrganizacaoHierarquica;
using Domain.ValueObjects;
using MediatR;
using Microsoft.Extensions.Logging;
using Infrastructure.Persistence;
using Infrastructure.MultiTenancy;

public record CriarEstipulanteCommand(
    string RazaoSocial,
    string? NomeFantasia,
    string Cnpj,
    EnderecoDto Endereco,
    TelefoneDto Telefone,
    EmailDto Email,
    Guid? GrupoEconomicoId = null) : IRequest<Result<EstipulanteDto>>;

public class CriarEstipulanteCommandHandler(
    IEstipulanteRepository repository,
    IGrupoEconomicoRepository grupoRepository,
    ITenantContext tenantContext,
    ILogger<CriarEstipulanteCommandHandler> logger)
    : IRequestHandler<CriarEstipulanteCommand, Result<EstipulanteDto>>
{
    public async Task<Result<EstipulanteDto>> Handle(
        CriarEstipulanteCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            if (request.GrupoEconomicoId.HasValue)
            {
                var grupo = await grupoRepository.GetByIdAsync(request.GrupoEconomicoId.Value, cancellationToken);
                if (grupo == null)
                    return Result<EstipulanteDto>.Failure("Grupo Econômico não encontrado.");
                if (grupo.TenantId != tenantContext.TenantId)
                    return Result<EstipulanteDto>.Failure("Grupo Econômico não pertence ao tenant atual.");
            }

            var cnpj = new Cnpj(request.Cnpj);
            var exists = await repository.ExistsByCnpjAsync(cnpj, cancellationToken: cancellationToken);
            if (exists)
                return Result<EstipulanteDto>.Failure("CNPJ já existe para este tenant.");

            var endereco = new Endereco(
                request.Endereco.Logradouro,
                request.Endereco.Numero,
                request.Endereco.Complemento,
                request.Endereco.Bairro,
                request.Endereco.Cidade,
                request.Endereco.Uf,
                request.Endereco.Cep);

            var telefone = new Telefone(request.Telefone.Numero);
            var email = new Email(request.Email.Endereco);

            var estipulante = new Estipulante(
                tenantContext.TenantId,
                request.RazaoSocial,
                request.NomeFantasia,
                cnpj,
                endereco,
                telefone,
                email,
                request.GrupoEconomicoId);

            await repository.AddAsync(estipulante, cancellationToken);
            await repository.SaveChangesAsync(cancellationToken);

            logger.LogInformation("Estipulante criado: {EstipulanteId}", estipulante.Id);

            return Result<EstipulanteDto>.Success(MapToDto(estipulante, null));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro ao criar estipulante");
            return Result<EstipulanteDto>.Failure("Erro ao criar estipulante.");
        }
    }

    private static EstipulanteDto MapToDto(Estipulante estipulante, string? grupoNome) => new()
    {
        Id = estipulante.Id,
        RazaoSocial = estipulante.RazaoSocial,
        NomeFantasia = estipulante.NomeFantasia,
        Cnpj = estipulante.Cnpj.Value,
        Endereco = MapEnderecoToDto(estipulante.Endereco),
        Telefone = new TelefoneDto { Numero = estipulante.Telefone.Numero },
        Email = new EmailDto { Endereco = estipulante.Email.Endereco },
        GrupoEconomicoId = estipulante.GrupoEconomicoId,
        GrupoEconomicoNome = grupoNome,
        Status = estipulante.Status,
        CreatedAt = estipulante.CreatedAt,
        UpdatedAt = estipulante.UpdatedAt
    };

    private static EnderecoDto MapEnderecoToDto(Endereco endereco) => new()
    {
        Logradouro = endereco.Logradouro,
        Numero = endereco.Numero,
        Complemento = endereco.Complemento,
        Bairro = endereco.Bairro,
        Cidade = endereco.Cidade,
        Uf = endereco.Uf,
        Cep = endereco.Cep
    };
}

public record AtualizarEstipulanteCommand(
    Guid Id,
    string RazaoSocial,
    string? NomeFantasia,
    EnderecoDto Endereco,
    TelefoneDto Telefone,
    EmailDto Email,
    Guid? GrupoEconomicoId = null) : IRequest<Result<EstipulanteDto>>;

public class AtualizarEstipulanteCommandHandler(
    IEstipulanteRepository repository,
    IGrupoEconomicoRepository grupoRepository,
    ITenantContext tenantContext,
    ILogger<AtualizarEstipulanteCommandHandler> logger)
    : IRequestHandler<AtualizarEstipulanteCommand, Result<EstipulanteDto>>
{
    public async Task<Result<EstipulanteDto>> Handle(
        AtualizarEstipulanteCommand request,
        CancellationToken cancellationToken)
    {
        var estipulante = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (estipulante == null)
            return Result<EstipulanteDto>.Failure("Estipulante não encontrado.");

        if (request.GrupoEconomicoId.HasValue)
        {
            var grupo = await grupoRepository.GetByIdAsync(request.GrupoEconomicoId.Value, cancellationToken);
            if (grupo == null)
                return Result<EstipulanteDto>.Failure("Grupo Econômico não encontrado.");
            if (grupo.TenantId != tenantContext.TenantId)
                return Result<EstipulanteDto>.Failure("Grupo Econômico não pertence ao tenant atual.");
        }

        var endereco = new Endereco(
            request.Endereco.Logradouro,
            request.Endereco.Numero,
            request.Endereco.Complemento,
            request.Endereco.Bairro,
            request.Endereco.Cidade,
            request.Endereco.Uf,
            request.Endereco.Cep);

        var telefone = new Telefone(request.Telefone.Numero);
        var email = new Email(request.Email.Endereco);

        estipulante.Atualizar(
            request.RazaoSocial,
            request.NomeFantasia,
            endereco,
            telefone,
            email,
            request.GrupoEconomicoId);

        repository.Update(estipulante);
        await repository.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Estipulante atualizado: {EstipulanteId}", estipulante.Id);

        return Result<EstipulanteDto>.Success(MapToDto(estipulante, null));
    }

    private static EstipulanteDto MapToDto(Estipulante estipulante, string? grupoNome) => new()
    {
        Id = estipulante.Id,
        RazaoSocial = estipulante.RazaoSocial,
        NomeFantasia = estipulante.NomeFantasia,
        Cnpj = estipulante.Cnpj.Value,
        Endereco = MapEnderecoToDto(estipulante.Endereco),
        Telefone = new TelefoneDto { Numero = estipulante.Telefone.Numero },
        Email = new EmailDto { Endereco = estipulante.Email.Endereco },
        GrupoEconomicoId = estipulante.GrupoEconomicoId,
        GrupoEconomicoNome = grupoNome,
        Status = estipulante.Status,
        CreatedAt = estipulante.CreatedAt,
        UpdatedAt = estipulante.UpdatedAt
    };

    private static EnderecoDto MapEnderecoToDto(Endereco endereco) => new()
    {
        Logradouro = endereco.Logradouro,
        Numero = endereco.Numero,
        Complemento = endereco.Complemento,
        Bairro = endereco.Bairro,
        Cidade = endereco.Cidade,
        Uf = endereco.Uf,
        Cep = endereco.Cep
    };
}

public record ExcluirEstipulanteCommand(Guid Id) : IRequest<Result<bool>>;

public class ExcluirEstipulanteCommandHandler(
    IEstipulanteRepository estipulanteRepository,
    ISubestipulanteRepository subestipulanteRepository,
    ILogger<ExcluirEstipulanteCommandHandler> logger)
    : IRequestHandler<ExcluirEstipulanteCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(
        ExcluirEstipulanteCommand request,
        CancellationToken cancellationToken)
    {
        var estipulante = await estipulanteRepository.GetByIdAsync(request.Id, cancellationToken);
        if (estipulante == null)
            return Result<bool>.Failure("Estipulante não encontrado.");

        var subestipulantes = await subestipulanteRepository.GetByEstipulanteAsync(request.Id, cancellationToken);
        var subestipulantesAtivos = subestipulantes.Where(s => s.Status == "ATIVO").ToList();

        if (subestipulantesAtivos.Any())
            return Result<bool>.Failure("Não é possível excluir um estipulante com subestipulantes ativos.");

        estipulanteRepository.Delete(estipulante);
        await estipulanteRepository.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Estipulante excluído: {EstipulanteId}", estipulante.Id);

        return Result<bool>.Success(true);
    }
}

public record AlterarStatusEstipulanteCommand(
    Guid Id,
    string Status) : IRequest<Result<EstipulanteDto>>;

public class AlterarStatusEstipulanteCommandHandler(
    IEstipulanteRepository repository,
    ILogger<AlterarStatusEstipulanteCommandHandler> logger)
    : IRequestHandler<AlterarStatusEstipulanteCommand, Result<EstipulanteDto>>
{
    public async Task<Result<EstipulanteDto>> Handle(
        AlterarStatusEstipulanteCommand request,
        CancellationToken cancellationToken)
    {
        var estipulante = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (estipulante == null)
            return Result<EstipulanteDto>.Failure("Estipulante não encontrado.");

        if (request.Status == "ATIVO")
            estipulante.Ativar();
        else
            estipulante.Inativar();

        repository.Update(estipulante);
        await repository.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Status do Estipulante alterado: {EstipulanteId} -> {Status}", estipulante.Id, request.Status);

        return Result<EstipulanteDto>.Success(MapToDto(estipulante, null));
    }

    private static EstipulanteDto MapToDto(Estipulante estipulante, string? grupoNome) => new()
    {
        Id = estipulante.Id,
        RazaoSocial = estipulante.RazaoSocial,
        NomeFantasia = estipulante.NomeFantasia,
        Cnpj = estipulante.Cnpj.Value,
        Endereco = MapEnderecoToDto(estipulante.Endereco),
        Telefone = new TelefoneDto { Numero = estipulante.Telefone.Numero },
        Email = new EmailDto { Endereco = estipulante.Email.Endereco },
        GrupoEconomicoId = estipulante.GrupoEconomicoId,
        GrupoEconomicoNome = grupoNome,
        Status = estipulante.Status,
        CreatedAt = estipulante.CreatedAt,
        UpdatedAt = estipulante.UpdatedAt
    };

    private static EnderecoDto MapEnderecoToDto(Endereco endereco) => new()
    {
        Logradouro = endereco.Logradouro,
        Numero = endereco.Numero,
        Complemento = endereco.Complemento,
        Bairro = endereco.Bairro,
        Cidade = endereco.Cidade,
        Uf = endereco.Uf,
        Cep = endereco.Cep
    };
}
