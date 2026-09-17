using Franquias.Api.Models;

namespace Franquias.Api.Repositories;

public interface IVendaRepository
{
    Task<Venda?> ObterPorIdAsync(int id);
    Task<(List<Venda> Itens, int Total)> ListarAsync(
        int? unidadeId,
        DateTime? dataInicio,
        DateTime? dataFim,
        int pagina,
        int tamanhoPagina,
        string? ordenarPor,
        bool decrescente);
    Task RegistrarVendaAsync(Venda venda, List<MovimentacaoEstoque> movimentacoes);
}
