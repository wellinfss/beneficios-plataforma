namespace BeneficiosPlataforma.API.Controllers;

using Application.Common;
using Application.OrganizacaoHierarquica;
using Application.OrganizacaoHierarquica.Commands;
using Application.OrganizacaoHierarquica.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/subestipulantes")]
[Authorize]
public class SubestipulantesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<SubestipulantesController> _logger;

    public SubestipulantesController(IMediator mediator, ILogger<SubestipulantesController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet]
    [Authorize(Policy = "subestipulantes:read")]
    public async Task<IActionResult> ListarSubestipulantes(
        [FromQuery] Guid? estipulanteId,
        [FromQuery] string? nome,
        [FromQuery] string? status,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var query = new ListarSubestipulantesQuery(estipulanteId, nome, status, page, pageSize);
            var result = await _mediator.Send(query, cancellationToken);

            return Ok(Result<PagedResult<SubestipulanteDto>>.Success(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao listar subestipulantes");
            return BadRequest(Result<object>.Failure("Erro ao listar subestipulantes"));
        }
    }

    [HttpGet("{id}")]
    [Authorize(Policy = "subestipulantes:read")]
    public async Task<IActionResult> ObterSubestipulante(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var query = new ObterSubestipulanteQuery(id);
            var result = await _mediator.Send(query, cancellationToken);

            if (!result.IsSuccess)
                return NotFound(result);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter subestipulante {SubestipulanteId}", id);
            return BadRequest(Result<object>.Failure("Erro ao obter subestipulante"));
        }
    }

    [HttpPost]
    [Authorize(Policy = "subestipulantes:write")]
    public async Task<IActionResult> CriarSubestipulante(
        [FromBody] CriarSubestipulanteRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            EnderecoDto? endereco = null;
            if (request.Endereco != null)
                endereco = new EnderecoDto
                {
                    Logradouro = request.Endereco.Logradouro,
                    Numero = request.Endereco.Numero,
                    Complemento = request.Endereco.Complemento,
                    Bairro = request.Endereco.Bairro,
                    Cidade = request.Endereco.Cidade,
                    Uf = request.Endereco.Uf,
                    Cep = request.Endereco.Cep
                };

            TelefoneDto? telefone = null;
            if (request.Telefone != null)
                telefone = new TelefoneDto { Numero = request.Telefone.Numero };

            EmailDto? email = null;
            if (request.Email != null)
                email = new EmailDto { Endereco = request.Email.Endereco };

            var command = new CriarSubestipulanteCommand(
                request.RazaoSocial,
                request.NomeFantasia,
                request.Cnpj,
                request.EstipulanteId,
                endereco,
                telefone,
                email);

            var result = await _mediator.Send(command, cancellationToken);

            if (!result.IsSuccess)
                return BadRequest(result);

            return CreatedAtAction(nameof(ObterSubestipulante), new { id = result.Value.Id }, result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar subestipulante");
            return BadRequest(Result<object>.Failure("Erro ao criar subestipulante"));
        }
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "subestipulantes:write")]
    public async Task<IActionResult> AtualizarSubestipulante(
        Guid id,
        [FromBody] AtualizarSubestipulanteRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            EnderecoDto? endereco = null;
            if (request.Endereco != null)
                endereco = new EnderecoDto
                {
                    Logradouro = request.Endereco.Logradouro,
                    Numero = request.Endereco.Numero,
                    Complemento = request.Endereco.Complemento,
                    Bairro = request.Endereco.Bairro,
                    Cidade = request.Endereco.Cidade,
                    Uf = request.Endereco.Uf,
                    Cep = request.Endereco.Cep
                };

            TelefoneDto? telefone = null;
            if (request.Telefone != null)
                telefone = new TelefoneDto { Numero = request.Telefone.Numero };

            EmailDto? email = null;
            if (request.Email != null)
                email = new EmailDto { Endereco = request.Email.Endereco };

            var command = new AtualizarSubestipulanteCommand(
                id,
                request.RazaoSocial,
                request.NomeFantasia,
                endereco,
                telefone,
                email);

            var result = await _mediator.Send(command, cancellationToken);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar subestipulante {SubestipulanteId}", id);
            return BadRequest(Result<object>.Failure("Erro ao atualizar subestipulante"));
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "subestipulantes:write")]
    public async Task<IActionResult> ExcluirSubestipulante(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var command = new ExcluirSubestipulanteCommand(id);
            var result = await _mediator.Send(command, cancellationToken);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao excluir subestipulante {SubestipulanteId}", id);
            return BadRequest(Result<object>.Failure("Erro ao excluir subestipulante"));
        }
    }

    [HttpPatch("{id}/status")]
    [Authorize(Policy = "subestipulantes:write")]
    public async Task<IActionResult> AlterarStatus(
        Guid id,
        [FromBody] AlterarStatusRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var command = new AlterarStatusSubestipulanteCommand(id, request.Status);
            var result = await _mediator.Send(command, cancellationToken);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao alterar status do subestipulante {SubestipulanteId}", id);
            return BadRequest(Result<object>.Failure("Erro ao alterar status"));
        }
    }
}

public class CriarSubestipulanteRequest
{
    public string RazaoSocial { get; set; } = null!;
    public string? NomeFantasia { get; set; }
    public string Cnpj { get; set; } = null!;
    public Guid EstipulanteId { get; set; }
    public EnderecoRequest? Endereco { get; set; }
    public TelefoneRequest? Telefone { get; set; }
    public EmailRequest? Email { get; set; }
}

public class AtualizarSubestipulanteRequest
{
    public string RazaoSocial { get; set; } = null!;
    public string? NomeFantasia { get; set; }
    public EnderecoRequest? Endereco { get; set; }
    public TelefoneRequest? Telefone { get; set; }
    public EmailRequest? Email { get; set; }
}
