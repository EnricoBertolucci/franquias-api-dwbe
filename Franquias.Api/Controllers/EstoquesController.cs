using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Franquias.Api.DTOs;
using Franquias.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Franquias.Api.Controllers;

[ApiController]
[Route("api/estoques")]
[Authorize]
public class EstoquesController : ControllerBase
{
    private readonly IEstoqueService _estoqueService;

    public EstoquesController(IEstoqueService estoqueService)
    {
        _estoqueService = estoqueService;
    }

    [HttpGet]
    public async Task<ActionResult<List<EstoqueRespostaDto>>> ListarPorUnidade([FromQuery] int unidadeId)
    {
        if (!PodeAcessarUnidade(unidadeId))
        {
            return Forbid();
        }

        var estoques = await _estoqueService.ListarPorUnidadeAsync(unidadeId);
        return Ok(estoques);
    }

    [HttpGet("{unidadeId:int}/{produtoId:int}")]
    public async Task<ActionResult<EstoqueRespostaDto>> ObterPorUnidadeEProduto(int unidadeId, int produtoId)
    {
        if (!PodeAcessarUnidade(unidadeId))
        {
            return Forbid();
        }

        var estoque = await _estoqueService.ObterPorUnidadeEProdutoAsync(unidadeId, produtoId);
        return Ok(estoque);
    }

    [HttpGet("{unidadeId:int}/criticos")]
    public async Task<ActionResult<List<EstoqueRespostaDto>>> ListarCriticos(int unidadeId)
    {
        if (!PodeAcessarUnidade(unidadeId))
        {
            return Forbid();
        }

        var estoques = await _estoqueService.ListarCriticosPorUnidadeAsync(unidadeId);
        return Ok(estoques);
    }

    [HttpPost("movimentacoes")]
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

    [HttpPut("{unidadeId:int}/{produtoId:int}/minimo")]
    [Authorize(Roles = "Administrador,Gestor")]
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
