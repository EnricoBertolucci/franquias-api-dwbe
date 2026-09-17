using Franquias.Api.DTOs;
using Franquias.Api.Models.Enums;
using Franquias.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Franquias.Api.Controllers;

[ApiController]
[Route("api/royalties")]
[Authorize]
public class RoyaltiesController : ControllerBase
{
    private readonly IRoyaltyService _royaltyService;

    public RoyaltiesController(IRoyaltyService royaltyService)
    {
        _royaltyService = royaltyService;
    }

    [HttpPost("calcular")]
    [Authorize(Roles = "Administrador,Gestor")]
    public async Task<ActionResult<RoyaltyRespostaDto>> Calcular(RoyaltyCalculoRequestDto dto)
    {
        if (!PodeAcessarUnidade(dto.UnidadeFranqueadaId))
        {
            return Forbid();
        }

        var royalty = await _royaltyService.CalcularAsync(dto);
        return Ok(royalty);
    }

    [HttpGet]
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

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Administrador")]
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
