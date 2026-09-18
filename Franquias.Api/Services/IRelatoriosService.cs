using Franquias.Api.DTOs;
using Franquias.Api.Models.Enums;

namespace Franquias.Api.Services;

public interface IRelatoriosService
{
    Task<List<FaturamentoUnidadeDto>> ObterFaturamentoAsync(int? unidadeId, DateTime? inicio, DateTime? fim);
    Task<List<RankingUnidadeDto>> ObterRankingUnidadesAsync(DateTime? inicio, DateTime? fim);
    Task<RoyaltiesRelatorioDto> ObterRoyaltiesAsync(DateTime? inicio, DateTime? fim);
    Task<List<ProdutoMaisVendidoDto>> ObterProdutosMaisVendidosAsync(DateTime? inicio, DateTime? fim);
    Task<List<EstoqueCriticoDto>> ObterEstoqueCriticoAsync(int? unidadeId);
    Task<List<ChamadosPorStatusDto>> ObterChamadosPorStatusAsync(int? unidadeId, PrioridadeChamado? prioridade);
}
