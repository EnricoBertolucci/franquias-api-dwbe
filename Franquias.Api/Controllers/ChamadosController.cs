using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Franquias.Api.DTOs;
using Franquias.Api.Models.Enums;
using Franquias.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Franquias.Api.Controllers;

/// <summary>
/// Abertura, acompanhamento e encerramento de chamados de suporte entre unidade e franqueadora.
/// </summary>
[ApiController]
[Route("api/chamados")]
[Authorize]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class ChamadosController : ControllerBase
{
    private readonly IChamadoService _chamadoService;

    public ChamadosController(IChamadoService chamadoService)
    {
        _chamadoService = chamadoService;
    }

    /// <summary>Abre um chamado para a unidade informada. O usuário responsável é extraído do token, não aceito no payload.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(ChamadoRespostaDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ChamadoRespostaDto>> Criar(ChamadoCriacaoDto dto)
    {
        if (!PodeAcessarUnidade(dto.UnidadeFranqueadaId))
        {
            return Forbid();
        }

        var usuarioId = ObterUsuarioIdAtual();
        var chamado = await _chamadoService.CriarAsync(dto, usuarioId);

        return CreatedAtAction(nameof(ObterPorId), new { id = chamado.Id }, chamado);
    }

    /// <summary>Lista chamados por unidade, status e prioridade, com paginação e ordenação. Sem <paramref name="unidadeId"/>, apenas Administrador pode consultar.</summary>
    /// <param name="unidadeId">Filtro pela unidade. Gestor/Operador só podem consultar a própria unidade.</param>
    /// <param name="status">Filtro por status do chamado.</param>
    /// <param name="prioridade">Filtro por prioridade do chamado.</param>
    /// <param name="pagina">Número da página, iniciando em 1.</param>
    /// <param name="tamanhoPagina">Quantidade de registros por página.</param>
    /// <param name="ordenarPor">Campo de ordenação (ex.: abertura, prioridade).</param>
    /// <param name="decrescente">Define se a ordenação é decrescente.</param>
    [HttpGet]
    [ProducesResponseType(typeof(ResultadoPaginadoDto<ChamadoRespostaDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ResultadoPaginadoDto<ChamadoRespostaDto>>> Listar(
        [FromQuery] int? unidadeId,
        [FromQuery] StatusChamado? status,
        [FromQuery] PrioridadeChamado? prioridade,
        [FromQuery] int pagina = 1,
        [FromQuery] int tamanhoPagina = 10,
        [FromQuery] string? ordenarPor = "abertura",
        [FromQuery] bool decrescente = true)
    {
        if (unidadeId.HasValue && !PodeAcessarUnidade(unidadeId.Value))
        {
            return Forbid();
        }

        if (!unidadeId.HasValue && !User.IsInRole("Administrador"))
        {
            return Forbid();
        }

        var resultado = await _chamadoService.ListarAsync(
            unidadeId, status, prioridade, pagina, tamanhoPagina, ordenarPor, decrescente);

        return Ok(resultado);
    }

    /// <summary>Consulta um chamado pelo identificador. Gestor/Operador só podem consultar chamados da própria unidade.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ChamadoRespostaDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ChamadoRespostaDto>> ObterPorId(int id)
    {
        var chamado = await _chamadoService.ObterPorIdAsync(id);

        if (!PodeAcessarUnidade(chamado.UnidadeFranqueadaId))
        {
            return Forbid();
        }

        return Ok(chamado);
    }

    /// <summary>Atualiza status, prioridade e/ou descrição de um chamado. Use o endpoint de encerramento para fechar (status Encerrado é rejeitado aqui com 400). Bloqueado com 400 se o chamado já estiver encerrado. Requer perfil Administrador.</summary>
    [HttpPut("{id:int}")]
    [Authorize(Roles = "Administrador")]
    [ProducesResponseType(typeof(ChamadoRespostaDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ChamadoRespostaDto>> Atualizar(int id, ChamadoAtualizacaoDto dto)
    {
        var chamado = await _chamadoService.AtualizarAsync(id, dto);
        return Ok(chamado);
    }

    /// <summary>Encerra um chamado, preenchendo a data de encerramento. Rejeitado com 400 se já estiver encerrado. Requer perfil Administrador.</summary>
    [HttpPatch("{id:int}/encerrar")]
    [Authorize(Roles = "Administrador")]
    [ProducesResponseType(typeof(ChamadoRespostaDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ChamadoRespostaDto>> Encerrar(int id)
    {
        var chamado = await _chamadoService.EncerrarAsync(id);
        return Ok(chamado);
    }

    private int ObterUsuarioIdAtual()
    {
        var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

        return int.Parse(idClaim!);
    }

    private bool PodeAcessarUnidade(int unidadeId)
    {
        if (User.IsInRole("Administrador"))
        {
            return true;
        }

        var unidadeClaim = User.FindFirst("unidadeFranqueadaId")?.Value;
        return unidadeClaim is not null && int.TryParse(unidadeClaim, out var unidadeUsuario) && unidadeUsuario == unidadeId;
    }
}
