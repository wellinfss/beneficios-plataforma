namespace BeneficiosPlataforma.API.Controllers;

using Application.Common;
using Application.OrganizacaoHierarquica;
using Application.OrganizacaoHierarquica.Commands;
using Application.OrganizacaoHierarquica.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/grupos-economicos")]
[Authorize]
public class GruposEconomicosController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<GruposEconomicosController> _logger;

    public GruposEconomicosController(IMediator mediator, ILogger<GruposEconomicosController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet]
    [Authorize(Policy = "grupos-economicos:read")]
    public async Task<IActionResult> ListarGrupos(
        [FromQuery] string? nome,
        [FromQuery] string? status,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var query = new ListarGruposEconomicosQuery(nome, status, page, pageSize);
            var result = await _mediator.Send(query, cancellationToken);

            return Ok(Result<PagedResult<GrupoEconomicoDto>>.Success(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao listar grupos econômicos");
            return BadRequest(Result<object>.Failure("Erro ao listar grupos econômicos"));
        }
    }

    [HttpGet("{id}")]
    [Authorize(Policy = "grupos-economicos:read")]
    public async Task<IActionResult> ObterGrupo(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var query = new ObterGrupoEconomicoQuery(id);
            var result = await _mediator.Send(query, cancellationToken);

            if (!result.IsSuccess)
                return NotFound(result);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter grupo econômico {GrupoId}", id);
            return BadRequest(Result<object>.Failure("Erro ao obter grupo econômico"));
        }
    }

    [HttpPost]
    [Authorize(Policy = "grupos-economicos:write")]
    public async Task<IActionResult> CriarGrupo(
        [FromBody] CriarGrupoEconomicoRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var command = new CriarGrupoEconomicoCommand(request.Nome, request.CnpjRaiz, request.Responsavel);
            var result = await _mediator.Send(command, cancellationToken);

            if (!result.IsSuccess)
                return BadRequest(result);

            return CreatedAtAction(nameof(ObterGrupo), new { id = result.Value.Id }, result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar grupo econômico");
            return BadRequest(Result<object>.Failure("Erro ao criar grupo econômico"));
        }
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "grupos-economicos:write")]
    public async Task<IActionResult> AtualizarGrupo(
        Guid id,
        [FromBody] AtualizarGrupoEconomicoRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var command = new AtualizarGrupoEconomicoCommand(id, request.Nome, request.Responsavel);
            var result = await _mediator.Send(command, cancellationToken);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar grupo econômico {GrupoId}", id);
            return BadRequest(Result<object>.Failure("Erro ao atualizar grupo econômico"));
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "grupos-economicos:write")]
    public async Task<IActionResult> ExcluirGrupo(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var command = new ExcluirGrupoEconomicoCommand(id);
            var result = await _mediator.Send(command, cancellationToken);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao excluir grupo econômico {GrupoId}", id);
            return BadRequest(Result<object>.Failure("Erro ao excluir grupo econômico"));
        }
    }

    [HttpPatch("{id}/status")]
    [Authorize(Policy = "grupos-economicos:write")]
    public async Task<IActionResult> AlterarStatus(
        Guid id,
        [FromBody] AlterarStatusRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var command = new AlterarStatusGrupoEconomicoCommand(id, request.Status);
            var result = await _mediator.Send(command, cancellationToken);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao alterar status do grupo econômico {GrupoId}", id);
            return BadRequest(Result<object>.Failure("Erro ao alterar status"));
        }
    }
}

public class CriarGrupoEconomicoRequest
{
    public string Nome { get; set; } = null!;
    public string CnpjRaiz { get; set; } = null!;
    public string Responsavel { get; set; } = null!;
}

public class AtualizarGrupoEconomicoRequest
{
    public string Nome { get; set; } = null!;
    public string Responsavel { get; set; } = null!;
}
