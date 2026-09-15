using Franquias.Api.DTOs;
using Franquias.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Franquias.Api.Controllers;

[ApiController]
[Route("api/usuarios")]
[Authorize]
public class UsuariosController : ControllerBase
{
    private readonly IUsuarioService _usuarioService;

    public UsuariosController(IUsuarioService usuarioService)
    {
        _usuarioService = usuarioService;
    }

    [HttpPost]
    [Authorize(Roles = "Administrador")]
    public async Task<ActionResult<UsuarioRespostaDto>> Criar(UsuarioCriacaoDto dto)
    {
        var usuario = await _usuarioService.CriarAsync(dto);
        return CreatedAtAction(nameof(ObterPorId), new { id = usuario.Id }, usuario);
    }

    [HttpGet]
    [Authorize(Roles = "Administrador")]
    public async Task<ActionResult<List<UsuarioRespostaDto>>> Listar()
    {
        var usuarios = await _usuarioService.ListarAsync();
        return Ok(usuarios);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<UsuarioRespostaDto>> ObterPorId(int id)
    {
        var usuario = await _usuarioService.ObterPorIdAsync(id);
        return Ok(usuario);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Administrador")]
    public async Task<ActionResult<UsuarioRespostaDto>> Atualizar(int id, UsuarioAtualizacaoDto dto)
    {
        var usuario = await _usuarioService.AtualizarAsync(id, dto);
        return Ok(usuario);
    }

    [HttpPatch("{id:int}/inativar")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Inativar(int id)
    {
        await _usuarioService.InativarAsync(id);
        return NoContent();
    }

    [HttpPatch("{id:int}/ativar")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Ativar(int id)
    {
        await _usuarioService.AtivarAsync(id);
        return NoContent();
    }
}
