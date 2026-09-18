using Franquias.Api.DTOs;
using Franquias.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Franquias.Api.Controllers;

/// <summary>
/// Cadastro e gerenciamento de usuários do sistema.
/// </summary>
[ApiController]
[Route("api/usuarios")]
[Authorize]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class UsuariosController : ControllerBase
{
    private readonly IUsuarioService _usuarioService;

    public UsuariosController(IUsuarioService usuarioService)
    {
        _usuarioService = usuarioService;
    }

    /// <summary>Cadastra um novo usuário. Requer perfil Administrador.</summary>
    [HttpPost]
    [Authorize(Roles = "Administrador")]
    [ProducesResponseType(typeof(UsuarioRespostaDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<UsuarioRespostaDto>> Criar(UsuarioCriacaoDto dto)
    {
        var usuario = await _usuarioService.CriarAsync(dto);
        return CreatedAtAction(nameof(ObterPorId), new { id = usuario.Id }, usuario);
    }

    /// <summary>Lista todos os usuários cadastrados. Requer perfil Administrador.</summary>
    [HttpGet]
    [Authorize(Roles = "Administrador")]
    [ProducesResponseType(typeof(List<UsuarioRespostaDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<List<UsuarioRespostaDto>>> Listar()
    {
        var usuarios = await _usuarioService.ListarAsync();
        return Ok(usuarios);
    }

    /// <summary>Consulta um usuário pelo identificador.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(UsuarioRespostaDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UsuarioRespostaDto>> ObterPorId(int id)
    {
        var usuario = await _usuarioService.ObterPorIdAsync(id);
        return Ok(usuario);
    }

    /// <summary>Atualiza os dados de um usuário. Requer perfil Administrador.</summary>
    [HttpPut("{id:int}")]
    [Authorize(Roles = "Administrador")]
    [ProducesResponseType(typeof(UsuarioRespostaDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UsuarioRespostaDto>> Atualizar(int id, UsuarioAtualizacaoDto dto)
    {
        var usuario = await _usuarioService.AtualizarAsync(id, dto);
        return Ok(usuario);
    }

    /// <summary>Inativa um usuário, impedindo login futuro sem excluir o registro. Requer perfil Administrador.</summary>
    [HttpPatch("{id:int}/inativar")]
    [Authorize(Roles = "Administrador")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Inativar(int id)
    {
        await _usuarioService.InativarAsync(id);
        return NoContent();
    }

    /// <summary>Reativa um usuário previamente inativado. Requer perfil Administrador.</summary>
    [HttpPatch("{id:int}/ativar")]
    [Authorize(Roles = "Administrador")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Ativar(int id)
    {
        await _usuarioService.AtivarAsync(id);
        return NoContent();
    }
}
