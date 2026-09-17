using Franquias.Api.DTOs;
using Franquias.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Franquias.Api.Controllers;

[ApiController]
[Route("api/fornecedores")]
[Authorize]
public class FornecedoresController : ControllerBase
{
    private readonly IFornecedorService _fornecedorService;

    public FornecedoresController(IFornecedorService fornecedorService)
    {
        _fornecedorService = fornecedorService;
    }

    [HttpGet]
    public async Task<ActionResult<ResultadoPaginadoDto<FornecedorRespostaDto>>> Listar(
        [FromQuery] string? nome,
        [FromQuery] string? cnpj,
        [FromQuery] bool? ativo,
        [FromQuery] int pagina = 1,
        [FromQuery] int tamanhoPagina = 10,
        [FromQuery] string? ordenarPor = "razaosocial",
        [FromQuery] bool decrescente = false)
    {
        var resultado = await _fornecedorService.ListarAsync(
            nome, cnpj, ativo, pagina, tamanhoPagina, ordenarPor, decrescente);

        return Ok(resultado);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<FornecedorRespostaDto>> ObterPorId(int id)
    {
        var fornecedor = await _fornecedorService.ObterPorIdAsync(id);
        return Ok(fornecedor);
    }

    [HttpPost]
    [Authorize(Roles = "Administrador")]
    public async Task<ActionResult<FornecedorRespostaDto>> Criar(FornecedorCriacaoDto dto)
    {
        var fornecedor = await _fornecedorService.CriarAsync(dto);
        return CreatedAtAction(nameof(ObterPorId), new { id = fornecedor.Id }, fornecedor);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Administrador")]
    public async Task<ActionResult<FornecedorRespostaDto>> Atualizar(int id, FornecedorAtualizacaoDto dto)
    {
        var fornecedor = await _fornecedorService.AtualizarAsync(id, dto);
        return Ok(fornecedor);
    }

    [HttpPatch("{id:int}/inativar")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Inativar(int id)
    {
        await _fornecedorService.InativarAsync(id);
        return NoContent();
    }

    [HttpPatch("{id:int}/ativar")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Ativar(int id)
    {
        await _fornecedorService.AtivarAsync(id);
        return NoContent();
    }

    [HttpPost("{id:int}/produtos/{produtoId:int}")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> AssociarProduto(int id, int produtoId)
    {
        await _fornecedorService.AssociarProdutoAsync(id, produtoId);
        return NoContent();
    }

    [HttpDelete("{id:int}/produtos/{produtoId:int}")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> DesassociarProduto(int id, int produtoId)
    {
        await _fornecedorService.DesassociarProdutoAsync(id, produtoId);
        return NoContent();
    }
}
