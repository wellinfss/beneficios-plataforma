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
[Route("api/operadoras")]
[Authorize]
public class OperadorasController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<OperadorasController> _logger;

    public OperadorasController(IMediator mediator, ILogger<OperadorasController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet]
    [Authorize(Policy = "operadoras:read")]
    public async Task<IActionResult> ListarOperadoras(
        [FromQuery] string? razaoSocial,
        [FromQuery] string? tipo,
        [FromQuery] string? status,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var query = new ListarOperadorasQuery(razaoSocial, tipo, status, page, pageSize);
            var result = await _mediator.Send(query, cancellationToken);

            return Ok(Result<PagedResult<OperadoraDto>>.Success(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao listar operadoras");
            return BadRequest(Result<object>.Failure("Erro ao listar operadoras"));
        }
    }

    [HttpGet("{id}")]
    [Authorize(Policy = "operadoras:read")]
    public async Task<IActionResult> ObterOperadora(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var query = new ObterOperadoraQuery(id);
            var result = await _mediator.Send(query, cancellationToken);

            if (!result.IsSuccess)
                return NotFound(result);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter operadora {OperadoraId}", id);
            return BadRequest(Result<object>.Failure("Erro ao obter operadora"));
        }
    }

    [HttpGet("{id}/produtos")]
    [Authorize(Policy = "operadoras:read")]
    public async Task<IActionResult> ListarProdutosPorOperadora(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var query = new ListarProdutosPorOperadoraQuery(id);
            var result = await _mediator.Send(query, cancellationToken);

            return Ok(Result<IEnumerable<ProdutoDto>>.Success(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao listar produtos da operadora {OperadoraId}", id);
            return BadRequest(Result<object>.Failure("Erro ao listar produtos"));
        }
    }

    [HttpPost]
    [Authorize(Policy = "operadoras:write")]
    public async Task<IActionResult> CriarOperadora(
        [FromBody] CriarOperadoraRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var command = new CriarOperadoraCommand(request.RazaoSocial, request.Cnpj, request.Tipo, request.RegistroAns);
            var result = await _mediator.Send(command, cancellationToken);

            if (!result.IsSuccess)
                return BadRequest(result);

            return CreatedAtAction(nameof(ObterOperadora), new { id = result.Value.Id }, result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar operadora");
            return BadRequest(Result<object>.Failure("Erro ao criar operadora"));
        }
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "operadoras:write")]
    public async Task<IActionResult> AtualizarOperadora(
        Guid id,
        [FromBody] AtualizarOperadoraRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var command = new AtualizarOperadoraCommand(id, request.RazaoSocial, request.Tipo, request.RegistroAns);
            var result = await _mediator.Send(command, cancellationToken);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar operadora {OperadoraId}", id);
            return BadRequest(Result<object>.Failure("Erro ao atualizar operadora"));
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "operadoras:write")]
    public async Task<IActionResult> ExcluirOperadora(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var command = new ExcluirOperadoraCommand(id);
            var result = await _mediator.Send(command, cancellationToken);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao excluir operadora {OperadoraId}", id);
            return BadRequest(Result<object>.Failure("Erro ao excluir operadora"));
        }
    }

    [HttpPatch("{id}/status")]
    [Authorize(Policy = "operadoras:write")]
    public async Task<IActionResult> AlterarStatus(
        Guid id,
        [FromBody] AlterarStatusRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var command = new AlterarStatusOperadoraCommand(id, request.Status);
            var result = await _mediator.Send(command, cancellationToken);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao alterar status da operadora {OperadoraId}", id);
            return BadRequest(Result<object>.Failure("Erro ao alterar status"));
        }
    }

    [HttpPut("{id}/integracao")]
    [Authorize(Policy = "operadoras:write")]
    public async Task<IActionResult> AtualizarIntegracao(
        Guid id,
        [FromBody] AtualizarIntegracaoRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var command = new AtualizarIntegracaoOperadoraCommand(
                id,
                request.EndpointIntegracao,
                request.FormatoIntegracao,
                request.CredenciaisPlanoTexto);
            var result = await _mediator.Send(command, cancellationToken);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar integração da operadora {OperadoraId}", id);
            return BadRequest(Result<object>.Failure("Erro ao atualizar integração"));
        }
    }
}

public class CriarOperadoraRequest
{
    public string RazaoSocial { get; set; } = null!;
    public string Cnpj { get; set; } = null!;
    public string Tipo { get; set; } = null!;
    public string? RegistroAns { get; set; }
}

public class AtualizarOperadoraRequest
{
    public string RazaoSocial { get; set; } = null!;
    public string Tipo { get; set; } = null!;
    public string? RegistroAns { get; set; }
}

public class AtualizarIntegracaoRequest
{
    public string? EndpointIntegracao { get; set; }
    public string? FormatoIntegracao { get; set; }
    public string? CredenciaisPlanoTexto { get; set; }
}
