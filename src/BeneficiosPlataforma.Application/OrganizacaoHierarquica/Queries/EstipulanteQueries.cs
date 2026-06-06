namespace BeneficiosPlataforma.Application.OrganizacaoHierarquica.Queries;

using Common;
using Domain.OrganizacaoHierarquica;
using MediatR;
using Microsoft.Extensions.Logging;

public record ListarEstipulantesQuery(
    string? Nome,
    string? Cnpj,
    Guid? GrupoEconomicoId,
    string? Status,
    int Page,
    int PageSize) : IRequest<PagedResult<EstipulanteDto>>;

public class ListarEstipulantesQueryHandler(
    IEstipulanteRepository repository,
    ILogger<ListarEstipulantesQueryHandler> logger)
    : IRequestHandler<ListarEstipulantesQuery, PagedResult<EstipulanteDto>>
{
    public async Task<PagedResult<EstipulanteDto>> Handle(
        ListarEstipulantesQuery request,
        CancellationToken cancellationToken)
    {
        var result = await repository.GetAllPagedAsync(
            request.Nome,
            request.Cnpj,
            request.GrupoEconomicoId,
            request.Status,
            request.Page,
            request.PageSize,
            cancellationToken);

        var dtos = result.Items
            .Select(e => MapToDto(e, null))
            .ToList();

        logger.LogInformation("Estipulantes listados: {Count} de {Total}", dtos.Count, result.TotalCount);

        return new PagedResult<EstipulanteDto>(dtos, result.TotalCount, result.PageNumber, result.PageSize);
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

public record ObterEstipulanteQuery(Guid Id) : IRequest<Result<EstipulanteDto>>;

public class ObterEstipulanteQueryHandler(
    IEstipulanteRepository repository,
    ILogger<ObterEstipulanteQueryHandler> logger)
    : IRequestHandler<ObterEstipulanteQuery, Result<EstipulanteDto>>
{
    public async Task<Result<EstipulanteDto>> Handle(
        ObterEstipulanteQuery request,
        CancellationToken cancellationToken)
    {
        var estipulante = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (estipulante == null)
            return Result<EstipulanteDto>.Failure("Estipulante não encontrado.");

        var dto = MapToDto(estipulante, null);

        logger.LogInformation("Estipulante obtido: {EstipulanteId}", estipulante.Id);

        return Result<EstipulanteDto>.Success(dto);
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

public record ObterHierarquiaEstipulanteQuery(Guid Id) : IRequest<Result<HierarquiaEstipulanteDto>>;

public class ObterHierarquiaEstipulanteQueryHandler(
    IEstipulanteRepository estipulanteRepository,
    ISubestipulanteRepository subestipulanteRepository,
    IGrupoEconomicoRepository grupoRepository,
    ILogger<ObterHierarquiaEstipulanteQueryHandler> logger)
    : IRequestHandler<ObterHierarquiaEstipulanteQuery, Result<HierarquiaEstipulanteDto>>
{
    public async Task<Result<HierarquiaEstipulanteDto>> Handle(
        ObterHierarquiaEstipulanteQuery request,
        CancellationToken cancellationToken)
    {
        var estipulante = await estipulanteRepository.GetByIdAsync(request.Id, cancellationToken);
        if (estipulante == null)
            return Result<HierarquiaEstipulanteDto>.Failure("Estipulante não encontrado.");

        string? grupoNome = null;
        if (estipulante.GrupoEconomicoId.HasValue)
        {
            var grupo = await grupoRepository.GetByIdAsync(estipulante.GrupoEconomicoId.Value, cancellationToken);
            if (grupo != null)
                grupoNome = grupo.Nome;
        }

        var subestipulantes = await subestipulanteRepository.GetByEstipulanteAsync(request.Id, cancellationToken);

        var dto = new HierarquiaEstipulanteDto
        {
            Estipulante = MapToDto(estipulante, grupoNome),
            Subestipulantes = subestipulantes
                .Select(s => new SubestipulanteDto
                {
                    Id = s.Id,
                    RazaoSocial = s.RazaoSocial,
                    NomeFantasia = s.NomeFantasia,
                    Cnpj = s.Cnpj.Value,
                    Endereco = s.Endereco != null ? MapEnderecoToDto(s.Endereco) : null,
                    Telefone = s.Telefone != null ? new TelefoneDto { Numero = s.Telefone.Numero } : null,
                    Email = s.Email != null ? new EmailDto { Endereco = s.Email.Endereco } : null,
                    EstipulanteId = s.EstipulanteId,
                    EstipulanteRazaoSocial = estipulante.RazaoSocial,
                    Status = s.Status,
                    CreatedAt = s.CreatedAt,
                    UpdatedAt = s.UpdatedAt
                })
                .ToList()
        };

        logger.LogInformation("Hierarquia do Estipulante obtida: {EstipulanteId}", estipulante.Id);

        return Result<HierarquiaEstipulanteDto>.Success(dto);
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

public class HierarquiaEstipulanteDto
{
    public EstipulanteDto Estipulante { get; set; } = null!;
    public IReadOnlyList<SubestipulanteDto> Subestipulantes { get; set; } = new List<SubestipulanteDto>();
}
