using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Franquias.Api.DTOs;
using Franquias.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Franquias.Api.Controllers;

/// <summary>
/// Registro e consulta de vendas e seus itens.
/// </summary>
[ApiController]
[Route("api/vendas")]
[Authorize]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class VendasController : ControllerBase
{
    private readonly IVendaService _vendaService;

    public VendasController(IVendaService vendaService)
    {
        _vendaService = vendaService;
    }

    /// <summary>Lista vendas por unidade e intervalo de datas, com paginação e ordenação. Sem <paramref name="unidadeId"/>, apenas Administrador pode consultar (todas as unidades).</summary>
    /// <param name="unidadeId">Filtro pela unidade. Gestor/Operador só podem consultar a própria unidade.</param>
    /// <param name="dataInicio">Data inicial do intervalo (inclusive).</param>
    /// <param name="dataFim">Data final do intervalo (inclusive).</param>
    /// <param name="pagina">Número da página, iniciando em 1.</param>
    /// <param name="tamanhoPagina">Quantidade de registros por página.</param>
    /// <param name="ordenarPor">Campo de ordenação (ex.: data, valorTotal).</param>
    /// <param name="decrescente">Define se a ordenação é decrescente.</param>
    [HttpGet]
    [ProducesResponseType(typeof(ResultadoPaginadoDto<VendaRespostaDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ResultadoPaginadoDto<VendaRespostaDto>>> Listar(
        [FromQuery] int? unidadeId,
        [FromQuery] DateTime? dataInicio,
        [FromQuery] DateTime? dataFim,
        [FromQuery] int pagina = 1,
        [FromQuery] int tamanhoPagina = 10,
        [FromQuery] string? ordenarPor = "data",
        [FromQuery] bool decrescente = true)
    {
        if (unidadeId.HasValue && !PodeAcessarUnidade(unidadeId.Value))
        {
            return Forbid();
        }

        if (!unidadeId.HasValue && !User.IsInRole("Administrador"))
        {
            return Forbid();
        }

        var resultado = await _vendaService.ListarAsync(
            unidadeId, dataInicio, dataFim, pagina, tamanhoPagina, ordenarPor, decrescente);

        return Ok(resultado);
    }

    /// <summary>Consulta uma venda pelo identificador, incluindo os itens. Gestor/Operador só podem consultar vendas da própria unidade.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(VendaRespostaDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<VendaRespostaDto>> ObterPorId(int id)
    {
        var venda = await _vendaService.ObterPorIdAsync(id);

        if (!PodeAcessarUnidade(venda.UnidadeFranqueadaId))
        {
            return Forbid();
        }

        return Ok(venda);
    }

    /// <summary>Registra uma venda com seus itens. O valor total é calculado a partir do preço de catálogo, e o estoque é baixado automaticamente. Rejeitada com 400 se não houver itens, se a unidade estiver inativa ou se o saldo de algum item for insuficiente.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(VendaRespostaDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<VendaRespostaDto>> Criar(VendaCriacaoDto dto)
    {
        if (!PodeAcessarUnidade(dto.UnidadeFranqueadaId))
        {
            return Forbid();
        }

        var usuarioId = ObterUsuarioIdAtual();
        var venda = await _vendaService.CriarAsync(dto, usuarioId);

        return CreatedAtAction(nameof(ObterPorId), new { id = venda.Id }, venda);
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
