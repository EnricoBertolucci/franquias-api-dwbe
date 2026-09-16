using Franquias.Api.DTOs;
using Franquias.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Franquias.Api.Controllers;

[ApiController]
[Route("api/unidades")]
[Authorize]
public class UnidadesController : ControllerBase
{
    private readonly IUnidadeFranqueadaService _unidadeService;

    public UnidadesController(IUnidadeFranqueadaService unidadeService)
    {
        _unidadeService = unidadeService;
    }

    [HttpGet]
    [Authorize(Roles = "Administrador")]
    public async Task<ActionResult<ResultadoPaginadoDto<UnidadeFranqueadaRespostaDto>>> Listar(
        [FromQuery] string? nome,
        [FromQuery] string? cidade,
        [FromQuery] string? cnpj,
        [FromQuery] string? responsavel,
        [FromQuery] int pagina = 1,
        [FromQuery] int tamanhoPagina = 10,
        [FromQuery] string? ordenarPor = "nome",
        [FromQuery] bool decrescente = false)
    {
        var resultado = await _unidadeService.ListarAsync(
            nome, cidade, cnpj, responsavel, pagina, tamanhoPagina, ordenarPor, decrescente);

        return Ok(resultado);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<UnidadeFranqueadaRespostaDto>> ObterPorId(int id)
    {
        if (!User.IsInRole("Administrador"))
        {
            var unidadeClaim = User.FindFirst("unidadeFranqueadaId")?.Value;
            if (unidadeClaim is null || !int.TryParse(unidadeClaim, out var unidadeId) || unidadeId != id)
            {
                return Forbid();
            }
        }

        var unidade = await _unidadeService.ObterPorIdAsync(id);
        return Ok(unidade);
    }

    [HttpPost]
    [Authorize(Roles = "Administrador")]
    public async Task<ActionResult<UnidadeFranqueadaRespostaDto>> Criar(UnidadeFranqueadaCriacaoDto dto)
    {
        var unidade = await _unidadeService.CriarAsync(dto);
        return CreatedAtAction(nameof(ObterPorId), new { id = unidade.Id }, unidade);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Administrador")]
    public async Task<ActionResult<UnidadeFranqueadaRespostaDto>> Atualizar(int id, UnidadeFranqueadaAtualizacaoDto dto)
    {
        var unidade = await _unidadeService.AtualizarAsync(id, dto);
        return Ok(unidade);
    }

    [HttpPatch("{id:int}/inativar")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Inativar(int id)
    {
        await _unidadeService.InativarAsync(id);
        return NoContent();
    }

    [HttpPatch("{id:int}/ativar")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Ativar(int id)
    {
        await _unidadeService.AtivarAsync(id);
        return NoContent();
    }
}
