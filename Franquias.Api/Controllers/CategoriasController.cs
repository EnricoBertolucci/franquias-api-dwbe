using Franquias.Api.DTOs;
using Franquias.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Franquias.Api.Controllers;

[ApiController]
[Route("api/categorias")]
[Authorize]
public class CategoriasController : ControllerBase
{
    private readonly ICategoriaService _categoriaService;

    public CategoriasController(ICategoriaService categoriaService)
    {
        _categoriaService = categoriaService;
    }

    [HttpGet]
    public async Task<ActionResult<List<CategoriaDto>>> Listar([FromQuery] bool? ativo)
    {
        var categorias = await _categoriaService.ListarAsync(ativo);
        return Ok(categorias);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CategoriaDto>> ObterPorId(int id)
    {
        var categoria = await _categoriaService.ObterPorIdAsync(id);
        return Ok(categoria);
    }

    [HttpPost]
    [Authorize(Roles = "Administrador")]
    public async Task<ActionResult<CategoriaDto>> Criar(CategoriaDto dto)
    {
        var categoria = await _categoriaService.CriarAsync(dto);
        return CreatedAtAction(nameof(ObterPorId), new { id = categoria.Id }, categoria);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Administrador")]
    public async Task<ActionResult<CategoriaDto>> Atualizar(int id, CategoriaDto dto)
    {
        var categoria = await _categoriaService.AtualizarAsync(id, dto);
        return Ok(categoria);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Remover(int id)
    {
        await _categoriaService.RemoverAsync(id);
        return NoContent();
    }
}
