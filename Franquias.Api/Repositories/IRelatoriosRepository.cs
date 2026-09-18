using Franquias.Api.DTOs;
using Franquias.Api.Models.Enums;

namespace Franquias.Api.Repositories;

public interface IRelatoriosRepository
{
    Task<List<FaturamentoUnidadeDto>> ObterFaturamentoAsync(int? unidadeId, DateTime inicio, DateTime fim);
    Task<List<RoyaltyPorUnidadeDto>> ObterRoyaltiesAsync(DateTime inicio, DateTime fim);
    Task<List<ProdutoMaisVendidoDto>> ObterProdutosMaisVendidosAsync(DateTime inicio, DateTime fim);
    Task<List<EstoqueCriticoDto>> ObterEstoqueCriticoAsync(int? unidadeId);
    Task<List<ChamadosPorStatusDto>> ObterChamadosPorStatusAsync(int? unidadeId, PrioridadeChamado? prioridade);
}
