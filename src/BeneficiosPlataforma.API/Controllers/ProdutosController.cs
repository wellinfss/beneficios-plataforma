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
[Route("api/produtos")]
[Authorize]
public class ProdutosController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<ProdutosController> _logger;

    public ProdutosController(IMediator mediator, ILogger<ProdutosController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet]
    [Authorize(Policy = "produtos:read")]
    public async Task<IActionResult> ListarProdutos(
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
            var query = new ListarProdutosQuery(nome, operadoraId, tipoBeneficio, status, page, pageSize);
            var result = await _mediator.Send(query, cancellationToken);

            return Ok(Result<PagedResult<ProdutoDto>>.Success(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao listar produtos");
            return BadRequest(Result<object>.Failure("Erro ao listar produtos"));
        }
    }

    [HttpGet("{id}")]
    [Authorize(Policy = "produtos:read")]
    public async Task<IActionResult> ObterProduto(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var query = new ObterProdutoQuery(id);
            var result = await _mediator.Send(query, cancellationToken);

            if (!result.IsSuccess)
                return NotFound(result);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter produto {ProdutoId}", id);
            return BadRequest(Result<object>.Failure("Erro ao obter produto"));
        }
    }

    [HttpGet("{id}/planos")]
    [Authorize(Policy = "produtos:read")]
    public async Task<IActionResult> ListarPlanosPorProduto(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var query = new ListarPlanosPorProdutoQuery(id);
            var result = await _mediator.Send(query, cancellationToken);

            return Ok(Result<IEnumerable<PlanoDto>>.Success(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao listar planos do produto {ProdutoId}", id);
            return BadRequest(Result<object>.Failure("Erro ao listar planos"));
        }
    }

    [HttpPost]
    [Authorize(Policy = "produtos:write")]
    public async Task<IActionResult> CriarProduto(
        [FromBody] CriarProdutoRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var command = new CriarProdutoCommand(
                request.Nome,
                request.OperadoraId,
                request.TipoBeneficio,
                request.Modalidade,
                request.RegistroAnsProduto);
            var result = await _mediator.Send(command, cancellationToken);

            if (!result.IsSuccess)
                return BadRequest(result);

            return CreatedAtAction(nameof(ObterProduto), new { id = result.Value.Id }, result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar produto");
            return BadRequest(Result<object>.Failure("Erro ao criar produto"));
        }
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "produtos:write")]
    public async Task<IActionResult> AtualizarProduto(
        Guid id,
        [FromBody] AtualizarProdutoRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var command = new AtualizarProdutoCommand(
                id,
                request.Nome,
                request.TipoBeneficio,
                request.Modalidade,
                request.RegistroAnsProduto);
            var result = await _mediator.Send(command, cancellationToken);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar produto {ProdutoId}", id);
            return BadRequest(Result<object>.Failure("Erro ao atualizar produto"));
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "produtos:write")]
    public async Task<IActionResult> ExcluirProduto(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var command = new ExcluirProdutoCommand(id);
            var result = await _mediator.Send(command, cancellationToken);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao excluir produto {ProdutoId}", id);
            return BadRequest(Result<object>.Failure("Erro ao excluir produto"));
        }
    }

    [HttpPatch("{id}/status")]
    [Authorize(Policy = "produtos:write")]
    public async Task<IActionResult> AlterarStatus(
        Guid id,
        [FromBody] AlterarStatusRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var command = new AlterarStatusProdutoCommand(id, request.Status);
            var result = await _mediator.Send(command, cancellationToken);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao alterar status do produto {ProdutoId}", id);
            return BadRequest(Result<object>.Failure("Erro ao alterar status"));
        }
    }
}

public class CriarProdutoRequest
{
    public string Nome { get; set; } = null!;
    public Guid OperadoraId { get; set; }
    public string TipoBeneficio { get; set; } = null!;
    public string Modalidade { get; set; } = null!;
    public string? RegistroAnsProduto { get; set; }
}

public class AtualizarProdutoRequest
{
    public string Nome { get; set; } = null!;
    public string TipoBeneficio { get; set; } = null!;
    public string Modalidade { get; set; } = null!;
    public string? RegistroAnsProduto { get; set; }
}
