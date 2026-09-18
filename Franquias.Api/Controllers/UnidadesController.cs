using Franquias.Api.DTOs;
using Franquias.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Franquias.Api.Controllers;

/// <summary>
/// Cadastro, consulta e situação das unidades franqueadas.
/// </summary>
[ApiController]
[Route("api/unidades")]
[Authorize]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class UnidadesController : ControllerBase
{
    private readonly IUnidadeFranqueadaService _unidadeService;

    public UnidadesController(IUnidadeFranqueadaService unidadeService)
    {
        _unidadeService = unidadeService;
    }

    /// <summary>Lista unidades com filtros por nome, cidade, CNPJ e responsável, com paginação e ordenação. Requer perfil Administrador.</summary>
    /// <param name="nome">Filtro parcial pelo nome da unidade.</param>
    /// <param name="cidade">Filtro parcial pela cidade.</param>
    /// <param name="cnpj">Filtro pelo CNPJ (exato ou parcial).</param>
    /// <param name="responsavel">Filtro parcial pelo nome do responsável/franqueado.</param>
    /// <param name="pagina">Número da página, iniciando em 1.</param>
    /// <param name="tamanhoPagina">Quantidade de registros por página.</param>
    /// <param name="ordenarPor">Campo de ordenação (ex.: nome, cidade, cnpj).</param>
    /// <param name="decrescente">Define se a ordenação é decrescente.</param>
    [HttpGet]
    [Authorize(Roles = "Administrador")]
    [ProducesResponseType(typeof(ResultadoPaginadoDto<UnidadeFranqueadaRespostaDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ResultadoPaginadoDto<UnidadeFranqueadaRespostaDto>>> Listar(
        [FromQuery] string? nome,
        [FromQuery] string? cidade,
        [FromQuery] string? cnpj,
        [FromQuery] string? responsavel,
        [FromQuery] int pagina = 1,
        [FromQuery] int tamanhoPagina = 10,
        [FromQuery] string? ordenarPor = "nome",
        [FromQuery] bool decrescente = false)
    {
        var resultado = await _unidadeService.ListarAsync(
            nome, cidade, cnpj, responsavel, pagina, tamanhoPagina, ordenarPor, decrescente);

        return Ok(resultado);
    }

    /// <summary>Consulta uma unidade pelo identificador. Gestor/Operador só podem consultar a própria unidade.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(UnidadeFranqueadaRespostaDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UnidadeFranqueadaRespostaDto>> ObterPorId(int id)
    {
        if (!User.IsInRole("Administrador"))
        {
            var unidadeClaim = User.FindFirst("unidadeFranqueadaId")?.Value;
            if (unidadeClaim is null || !int.TryParse(unidadeClaim, out var unidadeId) || unidadeId != id)
            {
                return Forbid();
            }
        }

        var unidade = await _unidadeService.ObterPorIdAsync(id);
        return Ok(unidade);
    }

    /// <summary>Cadastra uma nova unidade franqueada, podendo incluir responsáveis no mesmo payload. Requer perfil Administrador.</summary>
    [HttpPost]
    [Authorize(Roles = "Administrador")]
    [ProducesResponseType(typeof(UnidadeFranqueadaRespostaDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<UnidadeFranqueadaRespostaDto>> Criar(UnidadeFranqueadaCriacaoDto dto)
    {
        var unidade = await _unidadeService.CriarAsync(dto);
        return CreatedAtAction(nameof(ObterPorId), new { id = unidade.Id }, unidade);
    }

    /// <summary>Atualiza os dados de uma unidade franqueada. Requer perfil Administrador.</summary>
    [HttpPut("{id:int}")]
    [Authorize(Roles = "Administrador")]
    [ProducesResponseType(typeof(UnidadeFranqueadaRespostaDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UnidadeFranqueadaRespostaDto>> Atualizar(int id, UnidadeFranqueadaAtualizacaoDto dto)
    {
        var unidade = await _unidadeService.AtualizarAsync(id, dto);
        return Ok(unidade);
    }

    /// <summary>Inativa uma unidade, impedindo o registro de novas vendas. Requer perfil Administrador.</summary>
    [HttpPatch("{id:int}/inativar")]
    [Authorize(Roles = "Administrador")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Inativar(int id)
    {
        await _unidadeService.InativarAsync(id);
        return NoContent();
    }

    /// <summary>Reativa uma unidade previamente inativada. Requer perfil Administrador.</summary>
    [HttpPatch("{id:int}/ativar")]
    [Authorize(Roles = "Administrador")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Ativar(int id)
    {
        await _unidadeService.AtivarAsync(id);
        return NoContent();
    }
}
