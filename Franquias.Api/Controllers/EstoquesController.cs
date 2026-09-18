using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Franquias.Api.DTOs;
using Franquias.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Franquias.Api.Controllers;

/// <summary>
/// Consulta de saldo de estoque e registro de movimentações por unidade.
/// </summary>
[ApiController]
[Route("api/estoques")]
[Authorize]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class EstoquesController : ControllerBase
{
    private readonly IEstoqueService _estoqueService;

    public EstoquesController(IEstoqueService estoqueService)
    {
        _estoqueService = estoqueService;
    }

    /// <summary>Lista o saldo de estoque de todos os produtos de uma unidade. Gestor/Operador só podem consultar a própria unidade.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<EstoqueRespostaDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<List<EstoqueRespostaDto>>> ListarPorUnidade([FromQuery] int unidadeId)
    {
        if (!PodeAcessarUnidade(unidadeId))
        {
            return Forbid();
        }

        var estoques = await _estoqueService.ListarPorUnidadeAsync(unidadeId);
        return Ok(estoques);
    }

    /// <summary>Consulta o saldo de um produto específico em uma unidade.</summary>
    [HttpGet("{unidadeId:int}/{produtoId:int}")]
    [ProducesResponseType(typeof(EstoqueRespostaDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EstoqueRespostaDto>> ObterPorUnidadeEProduto(int unidadeId, int produtoId)
    {
        if (!PodeAcessarUnidade(unidadeId))
        {
            return Forbid();
        }

        var estoque = await _estoqueService.ObterPorUnidadeEProdutoAsync(unidadeId, produtoId);
        return Ok(estoque);
    }

    /// <summary>Lista os itens de uma unidade cuja quantidade atual está abaixo da quantidade mínima definida.</summary>
    [HttpGet("{unidadeId:int}/criticos")]
    [ProducesResponseType(typeof(List<EstoqueRespostaDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<List<EstoqueRespostaDto>>> ListarCriticos(int unidadeId)
    {
        if (!PodeAcessarUnidade(unidadeId))
        {
            return Forbid();
        }

        var estoques = await _estoqueService.ListarCriticosPorUnidadeAsync(unidadeId);
        return Ok(estoques);
    }

    /// <summary>Registra uma movimentação de entrada ou saída de estoque. Saídas maiores que o saldo disponível são rejeitadas com 400.</summary>
    [HttpPost("movimentacoes")]
    [ProducesResponseType(typeof(MovimentacaoEstoqueRespostaDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MovimentacaoEstoqueRespostaDto>> RegistrarMovimentacao(MovimentacaoEstoqueCriacaoDto dto)
    {
        if (!PodeAcessarUnidade(dto.UnidadeFranqueadaId))
        {
            return Forbid();
        }

        var usuarioId = ObterUsuarioIdAtual();
        var movimentacao = await _estoqueService.RegistrarMovimentacaoAsync(dto, usuarioId);

        return CreatedAtAction(
            nameof(ObterPorUnidadeEProduto),
            new { unidadeId = dto.UnidadeFranqueadaId, produtoId = dto.ProdutoServicoId },
            movimentacao);
    }

    /// <summary>Define a quantidade mínima de estoque de um produto em uma unidade. Requer perfil Administrador ou Gestor.</summary>
    [HttpPut("{unidadeId:int}/{produtoId:int}/minimo")]
    [Authorize(Roles = "Administrador,Gestor")]
    [ProducesResponseType(typeof(EstoqueRespostaDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EstoqueRespostaDto>> AtualizarEstoqueMinimo(
        int unidadeId, int produtoId, EstoqueMinimoAtualizacaoDto dto)
    {
        if (!PodeAcessarUnidade(unidadeId))
        {
            return Forbid();
        }

        var estoque = await _estoqueService.AtualizarEstoqueMinimoAsync(unidadeId, produtoId, dto.QuantidadeMinima);
        return Ok(estoque);
    }

    private int ObterUsuarioIdAtual()
    {
        var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

        return int.Parse(idClaim!);
    }

    private bool PodeAcessarUnidade(int unidadeId)
    {
        if (User.IsInRole("Administrador"))
        {
            return true;
        }

        var unidadeClaim = User.FindFirst("unidadeFranqueadaId")?.Value;
        return unidadeClaim is not null && int.TryParse(unidadeClaim, out var unidadeUsuario) && unidadeUsuario == unidadeId;
    }
}
