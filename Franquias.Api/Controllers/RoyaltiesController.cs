using Franquias.Api.DTOs;
using Franquias.Api.Models.Enums;
using Franquias.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Franquias.Api.Controllers;

/// <summary>
/// Cálculo e acompanhamento de royalties por unidade e período.
/// </summary>
[ApiController]
[Route("api/royalties")]
[Authorize]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class RoyaltiesController : ControllerBase
{
    private readonly IRoyaltyService _royaltyService;

    public RoyaltiesController(IRoyaltyService royaltyService)
    {
        _royaltyService = royaltyService;
    }

    /// <summary>Calcula (ou recalcula) o royalty de uma unidade para um período, com base no faturamento das vendas confirmadas. Rejeitado com 400 se o pagamento já tiver sido confirmado. Requer perfil Administrador ou Gestor.</summary>
    [HttpPost("calcular")]
    [Authorize(Roles = "Administrador,Gestor")]
    [ProducesResponseType(typeof(RoyaltyRespostaDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RoyaltyRespostaDto>> Calcular(RoyaltyCalculoRequestDto dto)
    {
        if (!PodeAcessarUnidade(dto.UnidadeFranqueadaId))
        {
            return Forbid();
        }

        var royalty = await _royaltyService.CalcularAsync(dto);
        return Ok(royalty);
    }

    /// <summary>Lista royalties por unidade, ano, mês e status de pagamento, com paginação e ordenação. Sem <paramref name="unidadeId"/>, apenas Administrador pode consultar.</summary>
    /// <param name="unidadeId">Filtro pela unidade. Gestor/Operador só podem consultar a própria unidade.</param>
    /// <param name="anoReferencia">Ano de referência do royalty.</param>
    /// <param name="mesReferencia">Mês de referência do royalty.</param>
    /// <param name="statusPagamento">Filtro por status de pagamento.</param>
    /// <param name="pagina">Número da página, iniciando em 1.</param>
    /// <param name="tamanhoPagina">Quantidade de registros por página.</param>
    /// <param name="ordenarPor">Campo de ordenação (ex.: periodo, valorCalculado).</param>
    /// <param name="decrescente">Define se a ordenação é decrescente.</param>
    [HttpGet]
    [ProducesResponseType(typeof(ResultadoPaginadoDto<RoyaltyRespostaDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ResultadoPaginadoDto<RoyaltyRespostaDto>>> Listar(
        [FromQuery] int? unidadeId,
        [FromQuery] int? anoReferencia,
        [FromQuery] int? mesReferencia,
        [FromQuery] StatusPagamento? statusPagamento,
        [FromQuery] int pagina = 1,
        [FromQuery] int tamanhoPagina = 10,
        [FromQuery] string? ordenarPor = "periodo",
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

        var resultado = await _royaltyService.ListarAsync(
            unidadeId, anoReferencia, mesReferencia, statusPagamento, pagina, tamanhoPagina, ordenarPor, decrescente);

        return Ok(resultado);
    }

    /// <summary>Atualiza o status de pagamento de um royalty. Requer perfil Administrador.</summary>
    [HttpPut("{id:int}")]
    [Authorize(Roles = "Administrador")]
    [ProducesResponseType(typeof(RoyaltyRespostaDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RoyaltyRespostaDto>> AtualizarStatus(int id, RoyaltyAtualizacaoStatusDto dto)
    {
        var royalty = await _royaltyService.AtualizarStatusAsync(id, dto);
        return Ok(royalty);
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
