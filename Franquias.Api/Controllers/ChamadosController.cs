using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Franquias.Api.DTOs;
using Franquias.Api.Models.Enums;
using Franquias.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Franquias.Api.Controllers;

[ApiController]
[Route("api/chamados")]
[Authorize]
public class ChamadosController : ControllerBase
{
    private readonly IChamadoService _chamadoService;

    public ChamadosController(IChamadoService chamadoService)
    {
        _chamadoService = chamadoService;
    }

    [HttpPost]
    public async Task<ActionResult<ChamadoRespostaDto>> Criar(ChamadoCriacaoDto dto)
    {
        if (!PodeAcessarUnidade(dto.UnidadeFranqueadaId))
        {
            return Forbid();
        }

        var usuarioId = ObterUsuarioIdAtual();
        var chamado = await _chamadoService.CriarAsync(dto, usuarioId);

        return CreatedAtAction(nameof(ObterPorId), new { id = chamado.Id }, chamado);
    }

    [HttpGet]
    public async Task<ActionResult<ResultadoPaginadoDto<ChamadoRespostaDto>>> Listar(
        [FromQuery] int? unidadeId,
        [FromQuery] StatusChamado? status,
        [FromQuery] PrioridadeChamado? prioridade,
        [FromQuery] int pagina = 1,
        [FromQuery] int tamanhoPagina = 10,
        [FromQuery] string? ordenarPor = "abertura",
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

        var resultado = await _chamadoService.ListarAsync(
            unidadeId, status, prioridade, pagina, tamanhoPagina, ordenarPor, decrescente);

        return Ok(resultado);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ChamadoRespostaDto>> ObterPorId(int id)
    {
        var chamado = await _chamadoService.ObterPorIdAsync(id);

        if (!PodeAcessarUnidade(chamado.UnidadeFranqueadaId))
        {
            return Forbid();
        }

        return Ok(chamado);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Administrador")]
    public async Task<ActionResult<ChamadoRespostaDto>> Atualizar(int id, ChamadoAtualizacaoDto dto)
    {
        var chamado = await _chamadoService.AtualizarAsync(id, dto);
        return Ok(chamado);
    }

    [HttpPatch("{id:int}/encerrar")]
    [Authorize(Roles = "Administrador")]
    public async Task<ActionResult<ChamadoRespostaDto>> Encerrar(int id)
    {
        var chamado = await _chamadoService.EncerrarAsync(id);
        return Ok(chamado);
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
