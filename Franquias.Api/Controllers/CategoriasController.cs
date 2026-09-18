using Franquias.Api.DTOs;
using Franquias.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Franquias.Api.Controllers;

/// <summary>
/// CRUD de categorias do catálogo de produtos/serviços.
/// </summary>
[ApiController]
[Route("api/categorias")]
[Authorize]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class CategoriasController : ControllerBase
{
    private readonly ICategoriaService _categoriaService;

    public CategoriasController(ICategoriaService categoriaService)
    {
        _categoriaService = categoriaService;
    }

    /// <summary>Lista categorias, opcionalmente filtrando por situação (ativa/inativa).</summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<CategoriaDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<CategoriaDto>>> Listar([FromQuery] bool? ativo)
    {
        var categorias = await _categoriaService.ListarAsync(ativo);
        return Ok(categorias);
    }

    /// <summary>Consulta uma categoria pelo identificador.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(CategoriaDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CategoriaDto>> ObterPorId(int id)
    {
        var categoria = await _categoriaService.ObterPorIdAsync(id);
        return Ok(categoria);
    }

    /// <summary>Cadastra uma categoria. Requer perfil Administrador.</summary>
    [HttpPost]
    [Authorize(Roles = "Administrador")]
    [ProducesResponseType(typeof(CategoriaDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<CategoriaDto>> Criar(CategoriaDto dto)
    {
        var categoria = await _categoriaService.CriarAsync(dto);
        return CreatedAtAction(nameof(ObterPorId), new { id = categoria.Id }, categoria);
    }

    /// <summary>Atualiza uma categoria. Requer perfil Administrador.</summary>
    [HttpPut("{id:int}")]
    [Authorize(Roles = "Administrador")]
    [ProducesResponseType(typeof(CategoriaDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CategoriaDto>> Atualizar(int id, CategoriaDto dto)
    {
        var categoria = await _categoriaService.AtualizarAsync(id, dto);
        return Ok(categoria);
    }

    /// <summary>Remove uma categoria. Falha com 400 se houver produtos/serviços vinculados. Requer perfil Administrador.</summary>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Administrador")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Remover(int id)
    {
        await _categoriaService.RemoverAsync(id);
        return NoContent();
    }
}
