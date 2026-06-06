namespace BeneficiosPlataforma.Application.OrganizacaoHierarquica.Commands;

using Common;
using Domain.OrganizacaoHierarquica;
using Domain.ValueObjects;
using MediatR;
using Microsoft.Extensions.Logging;
using Infrastructure.Persistence;
using Infrastructure.MultiTenancy;

public record CriarSubestipulanteCommand(
    string RazaoSocial,
    string? NomeFantasia,
    string Cnpj,
    Guid EstipulanteId,
    EnderecoDto? Endereco = null,
    TelefoneDto? Telefone = null,
    EmailDto? Email = null) : IRequest<Result<SubestipulanteDto>>;

public class CriarSubestipulanteCommandHandler(
    ISubestipulanteRepository repository,
    IEstipulanteRepository estipulanteRepository,
    ITenantContext tenantContext,
    ILogger<CriarSubestipulanteCommandHandler> logger)
    : IRequestHandler<CriarSubestipulanteCommand, Result<SubestipulanteDto>>
{
    public async Task<Result<SubestipulanteDto>> Handle(
        CriarSubestipulanteCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            var estipulante = await estipulanteRepository.GetByIdAsync(request.EstipulanteId, cancellationToken);
            if (estipulante == null)
                return Result<SubestipulanteDto>.Failure("Estipulante não encontrado.");
            if (estipulante.TenantId != tenantContext.TenantId)
                return Result<SubestipulanteDto>.Failure("Estipulante não pertence ao tenant atual.");

            var cnpj = new Cnpj(request.Cnpj);
            var exists = await repository.ExistsByCnpjAsync(cnpj, cancellationToken: cancellationToken);
            if (exists)
                return Result<SubestipulanteDto>.Failure("CNPJ já existe para este tenant.");

            Endereco? endereco = null;
            if (request.Endereco != null)
                endereco = new Endereco(
                    request.Endereco.Logradouro,
                    request.Endereco.Numero,
                    request.Endereco.Complemento,
                    request.Endereco.Bairro,
                    request.Endereco.Cidade,
                    request.Endereco.Uf,
                    request.Endereco.Cep);

            Telefone? telefone = null;
            if (request.Telefone != null)
                telefone = new Telefone(request.Telefone.Numero);

            Email? email = null;
            if (request.Email != null)
                email = new Email(request.Email.Endereco);

            var subestipulante = new Subestipulante(
                tenantContext.TenantId,
                request.RazaoSocial,
                request.NomeFantasia,
                cnpj,
                request.EstipulanteId,
                endereco,
                telefone,
                email);

            await repository.AddAsync(subestipulante, cancellationToken);
            await repository.SaveChangesAsync(cancellationToken);

            logger.LogInformation("Subestipulante criado: {SubestipulanteId}", subestipulante.Id);

            return Result<SubestipulanteDto>.Success(MapToDto(subestipulante, null));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro ao criar subestipulante");
            return Result<SubestipulanteDto>.Failure("Erro ao criar subestipulante.");
        }
    }

    private static SubestipulanteDto MapToDto(Subestipulante subestipulante, string? estipulanteNome) => new()
    {
        Id = subestipulante.Id,
        RazaoSocial = subestipulante.RazaoSocial,
        NomeFantasia = subestipulante.NomeFantasia,
        Cnpj = subestipulante.Cnpj.Value,
        Endereco = subestipulante.Endereco != null ? MapEnderecoToDto(subestipulante.Endereco) : null,
        Telefone = subestipulante.Telefone != null ? new TelefoneDto { Numero = subestipulante.Telefone.Numero } : null,
        Email = subestipulante.Email != null ? new EmailDto { Endereco = subestipulante.Email.Endereco } : null,
        EstipulanteId = subestipulante.EstipulanteId,
        EstipulanteRazaoSocial = estipulanteNome,
        Status = subestipulante.Status,
        CreatedAt = subestipulante.CreatedAt,
        UpdatedAt = subestipulante.UpdatedAt
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

public record AtualizarSubestipulanteCommand(
    Guid Id,
    string RazaoSocial,
    string? NomeFantasia,
    EnderecoDto? Endereco = null,
    TelefoneDto? Telefone = null,
    EmailDto? Email = null) : IRequest<Result<SubestipulanteDto>>;

public class AtualizarSubestipulanteCommandHandler(
    ISubestipulanteRepository repository,
    ILogger<AtualizarSubestipulanteCommandHandler> logger)
    : IRequestHandler<AtualizarSubestipulanteCommand, Result<SubestipulanteDto>>
{
    public async Task<Result<SubestipulanteDto>> Handle(
        AtualizarSubestipulanteCommand request,
        CancellationToken cancellationToken)
    {
        var subestipulante = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (subestipulante == null)
            return Result<SubestipulanteDto>.Failure("Subestipulante não encontrado.");

        Endereco? endereco = null;
        if (request.Endereco != null)
            endereco = new Endereco(
                request.Endereco.Logradouro,
                request.Endereco.Numero,
                request.Endereco.Complemento,
                request.Endereco.Bairro,
                request.Endereco.Cidade,
                request.Endereco.Uf,
                request.Endereco.Cep);

        Telefone? telefone = null;
        if (request.Telefone != null)
            telefone = new Telefone(request.Telefone.Numero);

        Email? email = null;
        if (request.Email != null)
            email = new Email(request.Email.Endereco);

        subestipulante.Atualizar(
            request.RazaoSocial,
            request.NomeFantasia,
            endereco,
            telefone,
            email);

        repository.Update(subestipulante);
        await repository.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Subestipulante atualizado: {SubestipulanteId}", subestipulante.Id);

        return Result<SubestipulanteDto>.Success(MapToDto(subestipulante, null));
    }

    private static SubestipulanteDto MapToDto(Subestipulante subestipulante, string? estipulanteNome) => new()
    {
        Id = subestipulante.Id,
        RazaoSocial = subestipulante.RazaoSocial,
        NomeFantasia = subestipulante.NomeFantasia,
        Cnpj = subestipulante.Cnpj.Value,
        Endereco = subestipulante.Endereco != null ? MapEnderecoToDto(subestipulante.Endereco) : null,
        Telefone = subestipulante.Telefone != null ? new TelefoneDto { Numero = subestipulante.Telefone.Numero } : null,
        Email = subestipulante.Email != null ? new EmailDto { Endereco = subestipulante.Email.Endereco } : null,
        EstipulanteId = subestipulante.EstipulanteId,
        EstipulanteRazaoSocial = estipulanteNome,
        Status = subestipulante.Status,
        CreatedAt = subestipulante.CreatedAt,
        UpdatedAt = subestipulante.UpdatedAt
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

public record ExcluirSubestipulanteCommand(Guid Id) : IRequest<Result<bool>>;

public class ExcluirSubestipulanteCommandHandler(
    ISubestipulanteRepository repository,
    ILogger<ExcluirSubestipulanteCommandHandler> logger)
    : IRequestHandler<ExcluirSubestipulanteCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(
        ExcluirSubestipulanteCommand request,
        CancellationToken cancellationToken)
    {
        var subestipulante = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (subestipulante == null)
            return Result<bool>.Failure("Subestipulante não encontrado.");

        repository.Delete(subestipulante);
        await repository.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Subestipulante excluído: {SubestipulanteId}", subestipulante.Id);

        return Result<bool>.Success(true);
    }
}

public record AlterarStatusSubestipulanteCommand(
    Guid Id,
    string Status) : IRequest<Result<SubestipulanteDto>>;

public class AlterarStatusSubestipulanteCommandHandler(
    ISubestipulanteRepository repository,
    ILogger<AlterarStatusSubestipulanteCommandHandler> logger)
    : IRequestHandler<AlterarStatusSubestipulanteCommand, Result<SubestipulanteDto>>
{
    public async Task<Result<SubestipulanteDto>> Handle(
        AlterarStatusSubestipulanteCommand request,
        CancellationToken cancellationToken)
    {
        var subestipulante = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (subestipulante == null)
            return Result<SubestipulanteDto>.Failure("Subestipulante não encontrado.");

        if (request.Status == "ATIVO")
            subestipulante.Ativar();
        else
            subestipulante.Inativar();

        repository.Update(subestipulante);
        await repository.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Status do Subestipulante alterado: {SubestipulanteId} -> {Status}", subestipulante.Id, request.Status);

        return Result<SubestipulanteDto>.Success(MapToDto(subestipulante, null));
    }

    private static SubestipulanteDto MapToDto(Subestipulante subestipulante, string? estipulanteNome) => new()
    {
        Id = subestipulante.Id,
        RazaoSocial = subestipulante.RazaoSocial,
        NomeFantasia = subestipulante.NomeFantasia,
        Cnpj = subestipulante.Cnpj.Value,
        Endereco = subestipulante.Endereco != null ? MapEnderecoToDto(subestipulante.Endereco) : null,
        Telefone = subestipulante.Telefone != null ? new TelefoneDto { Numero = subestipulante.Telefone.Numero } : null,
        Email = subestipulante.Email != null ? new EmailDto { Endereco = subestipulante.Email.Endereco } : null,
        EstipulanteId = subestipulante.EstipulanteId,
        EstipulanteRazaoSocial = estipulanteNome,
        Status = subestipulante.Status,
        CreatedAt = subestipulante.CreatedAt,
        UpdatedAt = subestipulante.UpdatedAt
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
