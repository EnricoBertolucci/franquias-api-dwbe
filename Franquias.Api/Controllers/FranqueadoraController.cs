using Franquias.Api.DTOs;
using Franquias.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Franquias.Api.Controllers;

/// <summary>
/// Cadastro da rede/franqueadora.
/// </summary>
[ApiController]
[Route("api/franqueadora")]
[Authorize]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class FranqueadoraController : ControllerBase
{
    private readonly IFranqueadoraService _franqueadoraService;

    public FranqueadoraController(IFranqueadoraService franqueadoraService)
    {
        _franqueadoraService = franqueadoraService;
    }

    /// <summary>Lista as franqueadoras cadastradas.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<FranqueadoraDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<FranqueadoraDto>>> Listar()
    {
        var franqueadoras = await _franqueadoraService.ListarAsync();
        return Ok(franqueadoras);
    }

    /// <summary>Cadastra a rede/franqueadora. Requer perfil Administrador.</summary>
    [HttpPost]
    [Authorize(Roles = "Administrador")]
    [ProducesResponseType(typeof(FranqueadoraDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<FranqueadoraDto>> Criar(FranqueadoraDto dto)
    {
        var franqueadora = await _franqueadoraService.CriarAsync(dto);
        return CreatedAtAction(nameof(Listar), new { }, franqueadora);
    }

    /// <summary>Atualiza os dados da franqueadora. Requer perfil Administrador.</summary>
    [HttpPut("{id:int}")]
    [Authorize(Roles = "Administrador")]
    [ProducesResponseType(typeof(FranqueadoraDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<FranqueadoraDto>> Atualizar(int id, FranqueadoraDto dto)
    {
        var franqueadora = await _franqueadoraService.AtualizarAsync(id, dto);
        return Ok(franqueadora);
    }
}
