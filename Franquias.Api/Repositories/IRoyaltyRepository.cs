using Franquias.Api.Models;
using Franquias.Api.Models.Enums;

namespace Franquias.Api.Repositories;

public interface IRoyaltyRepository
{
    Task<Royalty?> ObterPorIdAsync(int id);
    Task<Royalty?> ObterPorUnidadeEPeriodoAsync(int unidadeId, int anoReferencia, int mesReferencia);
    Task<decimal> ObterFaturamentoUnidadeAsync(int unidadeId, DateTime inicio, DateTime fim);
    Task<(List<Royalty> Itens, int Total)> ListarAsync(
        int? unidadeId,
        int? anoReferencia,
        int? mesReferencia,
        StatusPagamento? statusPagamento,
        int pagina,
        int tamanhoPagina,
        string? ordenarPor,
        bool decrescente);
    Task AdicionarAsync(Royalty royalty);
    Task SalvarAlteracoesAsync();
}
