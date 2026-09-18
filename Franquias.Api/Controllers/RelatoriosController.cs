using Franquias.Api.DTOs;
using Franquias.Api.Models.Enums;
using Franquias.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Franquias.Api.Controllers;

[ApiController]
[Route("api/relatorios")]
[Authorize]
public class RelatoriosController : ControllerBase
{
    private readonly IRelatoriosService _relatoriosService;

    public RelatoriosController(IRelatoriosService relatoriosService)
    {
        _relatoriosService = relatoriosService;
    }

    [HttpGet("faturamento")]
    public async Task<ActionResult<List<FaturamentoUnidadeDto>>> Faturamento(
        [FromQuery] int? unidadeId,
        [FromQuery] DateTime? inicio,
        [FromQuery] DateTime? fim)
    {
        if (unidadeId.HasValue && !PodeAcessarUnidade(unidadeId.Value))
        {
            return Forbid();
        }

        if (!unidadeId.HasValue && !User.IsInRole("Administrador"))
        {
            return Forbid();
        }

        var resultado = await _relatoriosService.ObterFaturamentoAsync(unidadeId, inicio, fim);
        return Ok(resultado);
    }

    [HttpGet("ranking-unidades")]
    [Authorize(Roles = "Administrador")]
    public async Task<ActionResult<List<RankingUnidadeDto>>> RankingUnidades(
        [FromQuery] DateTime? inicio,
        [FromQuery] DateTime? fim)
    {
        var resultado = await _relatoriosService.ObterRankingUnidadesAsync(inicio, fim);
        return Ok(resultado);
    }

    [HttpGet("royalties")]
    [Authorize(Roles = "Administrador")]
    public async Task<ActionResult<RoyaltiesRelatorioDto>> Royalties(
        [FromQuery] DateTime? inicio,
        [FromQuery] DateTime? fim)
    {
        var resultado = await _relatoriosService.ObterRoyaltiesAsync(inicio, fim);
        return Ok(resultado);
    }

    [HttpGet("produtos-mais-vendidos")]
    [Authorize(Roles = "Administrador")]
    public async Task<ActionResult<List<ProdutoMaisVendidoDto>>> ProdutosMaisVendidos(
        [FromQuery] DateTime? inicio,
        [FromQuery] DateTime? fim)
    {
        var resultado = await _relatoriosService.ObterProdutosMaisVendidosAsync(inicio, fim);
        return Ok(resultado);
    }

    [HttpGet("estoque-critico")]
    public async Task<ActionResult<List<EstoqueCriticoDto>>> EstoqueCritico([FromQuery] int? unidadeId)
    {
        if (unidadeId.HasValue && !PodeAcessarUnidade(unidadeId.Value))
        {
            return Forbid();
        }

        if (!unidadeId.HasValue && !User.IsInRole("Administrador"))
        {
            return Forbid();
        }

        var resultado = await _relatoriosService.ObterEstoqueCriticoAsync(unidadeId);
        return Ok(resultado);
    }

    [HttpGet("chamados-por-status")]
    public async Task<ActionResult<List<ChamadosPorStatusDto>>> ChamadosPorStatus(
        [FromQuery] int? unidadeId,
        [FromQuery] PrioridadeChamado? prioridade)
    {
        if (unidadeId.HasValue && !PodeAcessarUnidade(unidadeId.Value))
        {
            return Forbid();
        }

        if (!unidadeId.HasValue && !User.IsInRole("Administrador"))
        {
            return Forbid();
        }

        var resultado = await _relatoriosService.ObterChamadosPorStatusAsync(unidadeId, prioridade);
        return Ok(resultado);
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
