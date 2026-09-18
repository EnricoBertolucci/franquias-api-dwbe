using Franquias.Api.DTOs;
using Franquias.Api.Models.Enums;

namespace Franquias.Api.Services;

public interface IChamadoService
{
    Task<ChamadoRespostaDto> CriarAsync(ChamadoCriacaoDto dto, int usuarioId);
    Task<ResultadoPaginadoDto<ChamadoRespostaDto>> ListarAsync(
        int? unidadeId,
        StatusChamado? status,
        PrioridadeChamado? prioridade,
        int pagina,
        int tamanhoPagina,
        string? ordenarPor,
        bool decrescente);
    Task<ChamadoRespostaDto> ObterPorIdAsync(int id);
    Task<ChamadoRespostaDto> AtualizarAsync(int id, ChamadoAtualizacaoDto dto);
    Task<ChamadoRespostaDto> EncerrarAsync(int id);
}
