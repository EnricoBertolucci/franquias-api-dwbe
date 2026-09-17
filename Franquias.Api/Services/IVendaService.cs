using Franquias.Api.DTOs;

namespace Franquias.Api.Services;

public interface IVendaService
{
    Task<VendaRespostaDto> ObterPorIdAsync(int id);
    Task<ResultadoPaginadoDto<VendaRespostaDto>> ListarAsync(
        int? unidadeId,
        DateTime? dataInicio,
        DateTime? dataFim,
        int pagina,
        int tamanhoPagina,
        string? ordenarPor,
        bool decrescente);
    Task<VendaRespostaDto> CriarAsync(VendaCriacaoDto dto, int usuarioId);
}
