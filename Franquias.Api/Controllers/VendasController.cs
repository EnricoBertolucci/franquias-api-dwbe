using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Franquias.Api.DTOs;
using Franquias.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Franquias.Api.Controllers;

[ApiController]
[Route("api/vendas")]
[Authorize]
public class VendasController : ControllerBase
{
    private readonly IVendaService _vendaService;

    public VendasController(IVendaService vendaService)
    {
        _vendaService = vendaService;
    }

    [HttpGet]
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

    [HttpGet("{id:int}")]
    public async Task<ActionResult<VendaRespostaDto>> ObterPorId(int id)
    {
        var venda = await _vendaService.ObterPorIdAsync(id);

        if (!PodeAcessarUnidade(venda.UnidadeFranqueadaId))
        {
            return Forbid();
        }

        return Ok(venda);
    }

    [HttpPost]
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
