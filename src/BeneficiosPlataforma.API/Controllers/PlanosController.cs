namespace BeneficiosPlataforma.API.Controllers;

using Application.Catalogo;
using Application.Catalogo.Commands;
using Application.Catalogo.Queries;
using Application.Common;
using Domain.Catalogo;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/planos")]
[Authorize]
public class PlanosController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<PlanosController> _logger;

    public PlanosController(IMediator mediator, ILogger<PlanosController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet]
    [Authorize(Policy = "planos:read")]
    public async Task<IActionResult> ListarPlanos(
        [FromQuery] string? nome,
        [FromQuery] Guid? operadoraId,
        [FromQuery] string? tipoBeneficio,
        [FromQuery] string? status,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var query = new ListarPlanosQuery(nome, operadoraId, tipoBeneficio, status, page, pageSize);
            var result = await _mediator.Send(query, cancellationToken);

            return Ok(Result<PagedResult<PlanoDto>>.Success(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao listar planos");
            return BadRequest(Result<object>.Failure("Erro ao listar planos"));
        }
    }

    [HttpGet("{id}")]
    [Authorize(Policy = "planos:read")]
    public async Task<IActionResult> ObterPlano(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var query = new ObterPlanoQuery(id);
            var result = await _mediator.Send(query, cancellationToken);

            if (!result.IsSuccess)
                return NotFound(result);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter plano {PlanoId}", id);
            return BadRequest(Result<object>.Failure("Erro ao obter plano"));
        }
    }

    [HttpPost]
    [Authorize(Policy = "planos:write")]
    public async Task<IActionResult> CriarPlano(
        [FromBody] CriarPlanoRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var command = new CriarPlanoCommand(
                request.Nome,
                request.ProdutoId,
                request.Cobertura,
                request.AbrangenciaGeografica,
                request.TipoAcomodacao,
                request.ValorReferencia);
            var result = await _mediator.Send(command, cancellationToken);

            if (!result.IsSuccess)
                return BadRequest(result);

            return CreatedAtAction(nameof(ObterPlano), new { id = result.Value.Id }, result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar plano");
            return BadRequest(Result<object>.Failure("Erro ao criar plano"));
        }
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "planos:write")]
    public async Task<IActionResult> AtualizarPlano(
        Guid id,
        [FromBody] AtualizarPlanoRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var command = new AtualizarPlanoCommand(
                id,
                request.Nome,
                request.Cobertura,
                request.AbrangenciaGeografica,
                request.TipoAcomodacao,
                request.ValorReferencia);
            var result = await _mediator.Send(command, cancellationToken);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar plano {PlanoId}", id);
            return BadRequest(Result<object>.Failure("Erro ao atualizar plano"));
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "planos:write")]
    public async Task<IActionResult> ExcluirPlano(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var command = new ExcluirPlanoCommand(id);
            var result = await _mediator.Send(command, cancellationToken);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao excluir plano {PlanoId}", id);
            return BadRequest(Result<object>.Failure("Erro ao excluir plano"));
        }
    }

    [HttpPatch("{id}/status")]
    [Authorize(Policy = "planos:write")]
    public async Task<IActionResult> AlterarStatus(
        Guid id,
        [FromBody] AlterarStatusRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var command = new AlterarStatusPlanoCommand(id, request.Status);
            var result = await _mediator.Send(command, cancellationToken);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao alterar status do plano {PlanoId}", id);
            return BadRequest(Result<object>.Failure("Erro ao alterar status"));
        }
    }
}

public class CriarPlanoRequest
{
    public string Nome { get; set; } = null!;
    public Guid ProdutoId { get; set; }
    public string? Cobertura { get; set; }
    public string? AbrangenciaGeografica { get; set; }
    public string? TipoAcomodacao { get; set; }
    public decimal? ValorReferencia { get; set; }
}

public class AtualizarPlanoRequest
{
    public string Nome { get; set; } = null!;
    public string? Cobertura { get; set; }
    public string? AbrangenciaGeografica { get; set; }
    public string? TipoAcomodacao { get; set; }
    public decimal? ValorReferencia { get; set; }
}
