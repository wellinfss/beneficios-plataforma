namespace BeneficiosPlataforma.Application.OrganizacaoHierarquica.Queries;

using Common;
using Domain.OrganizacaoHierarquica;
using MediatR;
using Microsoft.Extensions.Logging;

public record ListarSubestipulantesQuery(
    Guid? EstipulanteId,
    string? Nome,
    string? Status,
    int Page,
    int PageSize) : IRequest<PagedResult<SubestipulanteDto>>;

public class ListarSubestipulantesQueryHandler(
    ISubestipulanteRepository repository,
    ILogger<ListarSubestipulantesQueryHandler> logger)
    : IRequestHandler<ListarSubestipulantesQuery, PagedResult<SubestipulanteDto>>
{
    public async Task<PagedResult<SubestipulanteDto>> Handle(
        ListarSubestipulantesQuery request,
        CancellationToken cancellationToken)
    {
        var result = await repository.GetAllPagedAsync(
            request.EstipulanteId,
            request.Nome,
            request.Status,
            request.Page,
            request.PageSize,
            cancellationToken);

        var dtos = result.Items
            .Select(s => MapToDto(s, null))
            .ToList();

        logger.LogInformation("Subestipulantes listados: {Count} de {Total}", dtos.Count, result.TotalCount);

        return new PagedResult<SubestipulanteDto>(dtos, result.TotalCount, result.PageNumber, result.PageSize);
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

public record ObterSubestipulanteQuery(Guid Id) : IRequest<Result<SubestipulanteDto>>;

public class ObterSubestipulanteQueryHandler(
    ISubestipulanteRepository repository,
    ILogger<ObterSubestipulanteQueryHandler> logger)
    : IRequestHandler<ObterSubestipulanteQuery, Result<SubestipulanteDto>>
{
    public async Task<Result<SubestipulanteDto>> Handle(
        ObterSubestipulanteQuery request,
        CancellationToken cancellationToken)
    {
        var subestipulante = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (subestipulante == null)
            return Result<SubestipulanteDto>.Failure("Subestipulante não encontrado.");

        var dto = MapToDto(subestipulante, null);

        logger.LogInformation("Subestipulante obtido: {SubestipulanteId}", subestipulante.Id);

        return Result<SubestipulanteDto>.Success(dto);
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
