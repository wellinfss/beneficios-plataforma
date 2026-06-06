namespace BeneficiosPlataforma.API.Controllers;

using Application.Common;
using Application.OrganizacaoHierarquica;
using Application.OrganizacaoHierarquica.Commands;
using Application.OrganizacaoHierarquica.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/estipulantes")]
[Authorize]
public class EstipulantesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<EstipulantesController> _logger;

    public EstipulantesController(IMediator mediator, ILogger<EstipulantesController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet]
    [Authorize(Policy = "estipulantes:read")]
    public async Task<IActionResult> ListarEstipulantes(
        [FromQuery] string? nome,
        [FromQuery] string? cnpj,
        [FromQuery] Guid? grupoEconomicoId,
        [FromQuery] string? status,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var query = new ListarEstipulantesQuery(nome, cnpj, grupoEconomicoId, status, page, pageSize);
            var result = await _mediator.Send(query, cancellationToken);

            return Ok(Result<PagedResult<EstipulanteDto>>.Success(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao listar estipulantes");
            return BadRequest(Result<object>.Failure("Erro ao listar estipulantes"));
        }
    }

    [HttpGet("{id}")]
    [Authorize(Policy = "estipulantes:read")]
    public async Task<IActionResult> ObterEstipulante(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var query = new ObterEstipulanteQuery(id);
            var result = await _mediator.Send(query, cancellationToken);

            if (!result.IsSuccess)
                return NotFound(result);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter estipulante {EstipulanteId}", id);
            return BadRequest(Result<object>.Failure("Erro ao obter estipulante"));
        }
    }

    [HttpGet("{id}/hierarquia")]
    [Authorize(Policy = "estipulantes:read")]
    public async Task<IActionResult> ObterHierarquia(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var query = new ObterHierarquiaEstipulanteQuery(id);
            var result = await _mediator.Send(query, cancellationToken);

            if (!result.IsSuccess)
                return NotFound(result);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter hierarquia do estipulante {EstipulanteId}", id);
            return BadRequest(Result<object>.Failure("Erro ao obter hierarquia"));
        }
    }

    [HttpGet("{id}/subestipulantes")]
    [Authorize(Policy = "subestipulantes:read")]
    public async Task<IActionResult> ListarSubestipulantes(
        Guid id,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var query = new ListarSubestipulantesQuery(id, null, null, page, pageSize);
            var result = await _mediator.Send(query, cancellationToken);

            return Ok(Result<PagedResult<SubestipulanteDto>>.Success(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao listar subestipulantes do estipulante {EstipulanteId}", id);
            return BadRequest(Result<object>.Failure("Erro ao listar subestipulantes"));
        }
    }

    [HttpPost]
    [Authorize(Policy = "estipulantes:write")]
    public async Task<IActionResult> CriarEstipulante(
        [FromBody] CriarEstipulanteRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var command = new CriarEstipulanteCommand(
                request.RazaoSocial,
                request.NomeFantasia,
                request.Cnpj,
                new EnderecoDto
                {
                    Logradouro = request.Endereco.Logradouro,
                    Numero = request.Endereco.Numero,
                    Complemento = request.Endereco.Complemento,
                    Bairro = request.Endereco.Bairro,
                    Cidade = request.Endereco.Cidade,
                    Uf = request.Endereco.Uf,
                    Cep = request.Endereco.Cep
                },
                new TelefoneDto { Numero = request.Telefone.Numero },
                new EmailDto { Endereco = request.Email.Endereco },
                request.GrupoEconomicoId);

            var result = await _mediator.Send(command, cancellationToken);

            if (!result.IsSuccess)
                return BadRequest(result);

            return CreatedAtAction(nameof(ObterEstipulante), new { id = result.Value.Id }, result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar estipulante");
            return BadRequest(Result<object>.Failure("Erro ao criar estipulante"));
        }
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "estipulantes:write")]
    public async Task<IActionResult> AtualizarEstipulante(
        Guid id,
        [FromBody] AtualizarEstipulanteRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var command = new AtualizarEstipulanteCommand(
                id,
                request.RazaoSocial,
                request.NomeFantasia,
                new EnderecoDto
                {
                    Logradouro = request.Endereco.Logradouro,
                    Numero = request.Endereco.Numero,
                    Complemento = request.Endereco.Complemento,
                    Bairro = request.Endereco.Bairro,
                    Cidade = request.Endereco.Cidade,
                    Uf = request.Endereco.Uf,
                    Cep = request.Endereco.Cep
                },
                new TelefoneDto { Numero = request.Telefone.Numero },
                new EmailDto { Endereco = request.Email.Endereco },
                request.GrupoEconomicoId);

            var result = await _mediator.Send(command, cancellationToken);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar estipulante {EstipulanteId}", id);
            return BadRequest(Result<object>.Failure("Erro ao atualizar estipulante"));
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "estipulantes:write")]
    public async Task<IActionResult> ExcluirEstipulante(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var command = new ExcluirEstipulanteCommand(id);
            var result = await _mediator.Send(command, cancellationToken);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao excluir estipulante {EstipulanteId}", id);
            return BadRequest(Result<object>.Failure("Erro ao excluir estipulante"));
        }
    }

    [HttpPatch("{id}/status")]
    [Authorize(Policy = "estipulantes:write")]
    public async Task<IActionResult> AlterarStatus(
        Guid id,
        [FromBody] AlterarStatusRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var command = new AlterarStatusEstipulanteCommand(id, request.Status);
            var result = await _mediator.Send(command, cancellationToken);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao alterar status do estipulante {EstipulanteId}", id);
            return BadRequest(Result<object>.Failure("Erro ao alterar status"));
        }
    }
}

public class CriarEstipulanteRequest
{
    public string RazaoSocial { get; set; } = null!;
    public string? NomeFantasia { get; set; }
    public string Cnpj { get; set; } = null!;
    public EnderecoRequest Endereco { get; set; } = null!;
    public TelefoneRequest Telefone { get; set; } = null!;
    public EmailRequest Email { get; set; } = null!;
    public Guid? GrupoEconomicoId { get; set; }
}

public class AtualizarEstipulanteRequest
{
    public string RazaoSocial { get; set; } = null!;
    public string? NomeFantasia { get; set; }
    public EnderecoRequest Endereco { get; set; } = null!;
    public TelefoneRequest Telefone { get; set; } = null!;
    public EmailRequest Email { get; set; } = null!;
    public Guid? GrupoEconomicoId { get; set; }
}

public class EnderecoRequest
{
    public string Logradouro { get; set; } = null!;
    public string Numero { get; set; } = null!;
    public string? Complemento { get; set; }
    public string Bairro { get; set; } = null!;
    public string Cidade { get; set; } = null!;
    public string Uf { get; set; } = null!;
    public string Cep { get; set; } = null!;
}

public class TelefoneRequest
{
    public string Numero { get; set; } = null!;
}

public class EmailRequest
{
    public string Endereco { get; set; } = null!;
}
