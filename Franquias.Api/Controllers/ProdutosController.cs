using Franquias.Api.DTOs;
using Franquias.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Franquias.Api.Controllers;

[ApiController]
[Route("api/produtos")]
[Authorize]
public class ProdutosController : ControllerBase
{
    private readonly IProdutoServicoService _produtoService;

    public ProdutosController(IProdutoServicoService produtoService)
    {
        _produtoService = produtoService;
    }

    [HttpGet]
    public async Task<ActionResult<ResultadoPaginadoDto<ProdutoServicoRespostaDto>>> Listar(
        [FromQuery] string? nome,
        [FromQuery] int? categoriaId,
        [FromQuery] bool? ativo,
        [FromQuery] int pagina = 1,
        [FromQuery] int tamanhoPagina = 10,
        [FromQuery] string? ordenarPor = "nome",
        [FromQuery] bool decrescente = false)
    {
        var resultado = await _produtoService.ListarAsync(
            nome, categoriaId, ativo, pagina, tamanhoPagina, ordenarPor, decrescente);

        return Ok(resultado);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ProdutoServicoRespostaDto>> ObterPorId(int id)
    {
        var produto = await _produtoService.ObterPorIdAsync(id);
        return Ok(produto);
    }

    [HttpPost]
    [Authorize(Roles = "Administrador")]
    public async Task<ActionResult<ProdutoServicoRespostaDto>> Criar(ProdutoServicoCriacaoDto dto)
    {
        var produto = await _produtoService.CriarAsync(dto);
        return CreatedAtAction(nameof(ObterPorId), new { id = produto.Id }, produto);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Administrador")]
    public async Task<ActionResult<ProdutoServicoRespostaDto>> Atualizar(int id, ProdutoServicoAtualizacaoDto dto)
    {
        var produto = await _produtoService.AtualizarAsync(id, dto);
        return Ok(produto);
    }

    [HttpPatch("{id:int}/inativar")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Inativar(int id)
    {
        await _produtoService.InativarAsync(id);
        return NoContent();
    }

    [HttpPatch("{id:int}/ativar")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Ativar(int id)
    {
        await _produtoService.AtivarAsync(id);
        return NoContent();
    }
}
