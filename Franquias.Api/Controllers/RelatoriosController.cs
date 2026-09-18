using Franquias.Api.DTOs;
using Franquias.Api.Models.Enums;
using Franquias.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Franquias.Api.Controllers;

/// <summary>
/// Relatórios e indicadores gerenciais da rede de franquias.
/// </summary>
[ApiController]
[Route("api/relatorios")]
[Authorize]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class RelatoriosController : ControllerBase
{
    private readonly IRelatoriosService _relatoriosService;

    public RelatoriosController(IRelatoriosService relatoriosService)
    {
        _relatoriosService = relatoriosService;
    }

    /// <summary>Faturamento por unidade e período (mês corrente por padrão, se <paramref name="inicio"/>/<paramref name="fim"/> não forem informados). Sem <paramref name="unidadeId"/>, apenas Administrador pode consultar (todas as unidades).</summary>
    /// <param name="unidadeId">Filtro pela unidade. Gestor/Operador só podem consultar a própria unidade.</param>
    /// <param name="inicio">Início do período.</param>
    /// <param name="fim">Fim do período.</param>
    [HttpGet("faturamento")]
    [ProducesResponseType(typeof(List<FaturamentoUnidadeDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
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

    /// <summary>Ranking de unidades por faturamento no período, ordenado decrescente. Requer perfil Administrador.</summary>
    [HttpGet("ranking-unidades")]
    [Authorize(Roles = "Administrador")]
    [ProducesResponseType(typeof(List<RankingUnidadeDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<List<RankingUnidadeDto>>> RankingUnidades(
        [FromQuery] DateTime? inicio,
        [FromQuery] DateTime? fim)
    {
        var resultado = await _relatoriosService.ObterRankingUnidadesAsync(inicio, fim);
        return Ok(resultado);
    }

    /// <summary>Total de royalties gerados no período, consolidado por unidade. Requer perfil Administrador.</summary>
    [HttpGet("royalties")]
    [Authorize(Roles = "Administrador")]
    [ProducesResponseType(typeof(RoyaltiesRelatorioDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<RoyaltiesRelatorioDto>> Royalties(
        [FromQuery] DateTime? inicio,
        [FromQuery] DateTime? fim)
    {
        var resultado = await _relatoriosService.ObterRoyaltiesAsync(inicio, fim);
        return Ok(resultado);
    }

    /// <summary>Produtos/serviços mais vendidos no período, ordenados por quantidade decrescente. Requer perfil Administrador.</summary>
    [HttpGet("produtos-mais-vendidos")]
    [Authorize(Roles = "Administrador")]
    [ProducesResponseType(typeof(List<ProdutoMaisVendidoDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<List<ProdutoMaisVendidoDto>>> ProdutosMaisVendidos(
        [FromQuery] DateTime? inicio,
        [FromQuery] DateTime? fim)
    {
        var resultado = await _relatoriosService.ObterProdutosMaisVendidosAsync(inicio, fim);
        return Ok(resultado);
    }

    /// <summary>Itens com quantidade atual abaixo da quantidade mínima. Sem <paramref name="unidadeId"/>, apenas Administrador pode consultar (todas as unidades).</summary>
    [HttpGet("estoque-critico")]
    [ProducesResponseType(typeof(List<EstoqueCriticoDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
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

    /// <summary>Quantidade de chamados agrupada por status, com filtro opcional por unidade e prioridade. Sem <paramref name="unidadeId"/>, apenas Administrador pode consultar (todas as unidades).</summary>
    [HttpGet("chamados-por-status")]
    [ProducesResponseType(typeof(List<ChamadosPorStatusDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
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
