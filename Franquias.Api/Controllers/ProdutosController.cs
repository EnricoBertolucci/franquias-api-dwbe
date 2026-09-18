using Franquias.Api.DTOs;
using Franquias.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Franquias.Api.Controllers;

/// <summary>
/// CRUD de produtos ou serviços padronizados pela rede.
/// </summary>
[ApiController]
[Route("api/produtos")]
[Authorize]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class ProdutosController : ControllerBase
{
    private readonly IProdutoServicoService _produtoService;

    public ProdutosController(IProdutoServicoService produtoService)
    {
        _produtoService = produtoService;
    }

    /// <summary>Lista produtos/serviços com filtros por nome, categoria e situação, com paginação e ordenação.</summary>
    /// <param name="nome">Filtro parcial pelo nome.</param>
    /// <param name="categoriaId">Filtro pela categoria.</param>
    /// <param name="ativo">Filtro por situação (ativo/inativo).</param>
    /// <param name="pagina">Número da página, iniciando em 1.</param>
    /// <param name="tamanhoPagina">Quantidade de registros por página.</param>
    /// <param name="ordenarPor">Campo de ordenação (ex.: nome, precoBase).</param>
    /// <param name="decrescente">Define se a ordenação é decrescente.</param>
    [HttpGet]
    [ProducesResponseType(typeof(ResultadoPaginadoDto<ProdutoServicoRespostaDto>), StatusCodes.Status200OK)]
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

    /// <summary>Consulta um produto/serviço pelo identificador.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ProdutoServicoRespostaDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProdutoServicoRespostaDto>> ObterPorId(int id)
    {
        var produto = await _produtoService.ObterPorIdAsync(id);
        return Ok(produto);
    }

    /// <summary>Cadastra um produto/serviço. Requer perfil Administrador.</summary>
    [HttpPost]
    [Authorize(Roles = "Administrador")]
    [ProducesResponseType(typeof(ProdutoServicoRespostaDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ProdutoServicoRespostaDto>> Criar(ProdutoServicoCriacaoDto dto)
    {
        var produto = await _produtoService.CriarAsync(dto);
        return CreatedAtAction(nameof(ObterPorId), new { id = produto.Id }, produto);
    }

    /// <summary>Atualiza um produto/serviço. Requer perfil Administrador.</summary>
    [HttpPut("{id:int}")]
    [Authorize(Roles = "Administrador")]
    [ProducesResponseType(typeof(ProdutoServicoRespostaDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProdutoServicoRespostaDto>> Atualizar(int id, ProdutoServicoAtualizacaoDto dto)
    {
        var produto = await _produtoService.AtualizarAsync(id, dto);
        return Ok(produto);
    }

    /// <summary>Inativa um produto/serviço sem excluí-lo. Requer perfil Administrador.</summary>
    [HttpPatch("{id:int}/inativar")]
    [Authorize(Roles = "Administrador")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Inativar(int id)
    {
        await _produtoService.InativarAsync(id);
        return NoContent();
    }

    /// <summary>Reativa um produto/serviço previamente inativado. Requer perfil Administrador.</summary>
    [HttpPatch("{id:int}/ativar")]
    [Authorize(Roles = "Administrador")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Ativar(int id)
    {
        await _produtoService.AtivarAsync(id);
        return NoContent();
    }
}
