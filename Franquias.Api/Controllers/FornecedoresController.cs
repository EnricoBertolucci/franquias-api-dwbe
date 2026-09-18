using Franquias.Api.DTOs;
using Franquias.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Franquias.Api.Controllers;

/// <summary>
/// CRUD de fornecedores e associação com produtos/serviços.
/// </summary>
[ApiController]
[Route("api/fornecedores")]
[Authorize]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class FornecedoresController : ControllerBase
{
    private readonly IFornecedorService _fornecedorService;

    public FornecedoresController(IFornecedorService fornecedorService)
    {
        _fornecedorService = fornecedorService;
    }

    /// <summary>Lista fornecedores com filtros por nome, CNPJ e situação, com paginação e ordenação.</summary>
    /// <param name="nome">Filtro parcial pela razão social/nome.</param>
    /// <param name="cnpj">Filtro pelo CNPJ.</param>
    /// <param name="ativo">Filtro por situação (ativo/inativo).</param>
    /// <param name="pagina">Número da página, iniciando em 1.</param>
    /// <param name="tamanhoPagina">Quantidade de registros por página.</param>
    /// <param name="ordenarPor">Campo de ordenação (ex.: razaosocial, cnpj).</param>
    /// <param name="decrescente">Define se a ordenação é decrescente.</param>
    [HttpGet]
    [ProducesResponseType(typeof(ResultadoPaginadoDto<FornecedorRespostaDto>), StatusCodes.Status200OK)]
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

    /// <summary>Consulta um fornecedor pelo identificador, incluindo produtos/serviços vinculados.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(FornecedorRespostaDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<FornecedorRespostaDto>> ObterPorId(int id)
    {
        var fornecedor = await _fornecedorService.ObterPorIdAsync(id);
        return Ok(fornecedor);
    }

    /// <summary>Cadastra um fornecedor. Falha com 400 se já existir fornecedor com o mesmo CNPJ. Requer perfil Administrador.</summary>
    [HttpPost]
    [Authorize(Roles = "Administrador")]
    [ProducesResponseType(typeof(FornecedorRespostaDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<FornecedorRespostaDto>> Criar(FornecedorCriacaoDto dto)
    {
        var fornecedor = await _fornecedorService.CriarAsync(dto);
        return CreatedAtAction(nameof(ObterPorId), new { id = fornecedor.Id }, fornecedor);
    }

    /// <summary>Atualiza um fornecedor. Requer perfil Administrador.</summary>
    [HttpPut("{id:int}")]
    [Authorize(Roles = "Administrador")]
    [ProducesResponseType(typeof(FornecedorRespostaDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<FornecedorRespostaDto>> Atualizar(int id, FornecedorAtualizacaoDto dto)
    {
        var fornecedor = await _fornecedorService.AtualizarAsync(id, dto);
        return Ok(fornecedor);
    }

    /// <summary>Inativa um fornecedor sem excluí-lo. Requer perfil Administrador.</summary>
    [HttpPatch("{id:int}/inativar")]
    [Authorize(Roles = "Administrador")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Inativar(int id)
    {
        await _fornecedorService.InativarAsync(id);
        return NoContent();
    }

    /// <summary>Reativa um fornecedor previamente inativado. Requer perfil Administrador.</summary>
    [HttpPatch("{id:int}/ativar")]
    [Authorize(Roles = "Administrador")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Ativar(int id)
    {
        await _fornecedorService.AtivarAsync(id);
        return NoContent();
    }

    /// <summary>Associa um produto/serviço ao fornecedor. Requer perfil Administrador.</summary>
    [HttpPost("{id:int}/produtos/{produtoId:int}")]
    [Authorize(Roles = "Administrador")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AssociarProduto(int id, int produtoId)
    {
        await _fornecedorService.AssociarProdutoAsync(id, produtoId);
        return NoContent();
    }

    /// <summary>Remove a associação entre um fornecedor e um produto/serviço. Requer perfil Administrador.</summary>
    [HttpDelete("{id:int}/produtos/{produtoId:int}")]
    [Authorize(Roles = "Administrador")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DesassociarProduto(int id, int produtoId)
    {
        await _fornecedorService.DesassociarProdutoAsync(id, produtoId);
        return NoContent();
    }
}
