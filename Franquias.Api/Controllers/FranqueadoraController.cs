using Franquias.Api.DTOs;
using Franquias.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Franquias.Api.Controllers;

[ApiController]
[Route("api/franqueadora")]
[Authorize]
public class FranqueadoraController : ControllerBase
{
    private readonly IFranqueadoraService _franqueadoraService;

    public FranqueadoraController(IFranqueadoraService franqueadoraService)
    {
        _franqueadoraService = franqueadoraService;
    }

    [HttpGet]
    public async Task<ActionResult<List<FranqueadoraDto>>> Listar()
    {
        var franqueadoras = await _franqueadoraService.ListarAsync();
        return Ok(franqueadoras);
    }

    [HttpPost]
    [Authorize(Roles = "Administrador")]
    public async Task<ActionResult<FranqueadoraDto>> Criar(FranqueadoraDto dto)
    {
        var franqueadora = await _franqueadoraService.CriarAsync(dto);
        return CreatedAtAction(nameof(Listar), new { }, franqueadora);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Administrador")]
    public async Task<ActionResult<FranqueadoraDto>> Atualizar(int id, FranqueadoraDto dto)
    {
        var franqueadora = await _franqueadoraService.AtualizarAsync(id, dto);
        return Ok(franqueadora);
    }
}
