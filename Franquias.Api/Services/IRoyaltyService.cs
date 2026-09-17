using Franquias.Api.DTOs;
using Franquias.Api.Models.Enums;

namespace Franquias.Api.Services;

public interface IRoyaltyService
{
    Task<RoyaltyRespostaDto> CalcularAsync(RoyaltyCalculoRequestDto dto);
    Task<ResultadoPaginadoDto<RoyaltyRespostaDto>> ListarAsync(
        int? unidadeId,
        int? anoReferencia,
        int? mesReferencia,
        StatusPagamento? statusPagamento,
        int pagina,
        int tamanhoPagina,
        string? ordenarPor,
        bool decrescente);
    Task<RoyaltyRespostaDto> AtualizarStatusAsync(int id, RoyaltyAtualizacaoStatusDto dto);
}
