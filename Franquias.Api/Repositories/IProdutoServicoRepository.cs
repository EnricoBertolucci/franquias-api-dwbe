using Franquias.Api.Models;

namespace Franquias.Api.Repositories;

public interface IProdutoServicoRepository
{
    Task<ProdutoServico?> ObterPorIdAsync(int id);
    Task<(List<ProdutoServico> Itens, int Total)> ListarAsync(
        string? nome,
        int? categoriaId,
        bool? ativo,
        int pagina,
        int tamanhoPagina,
        string? ordenarPor,
        bool decrescente);
    Task AdicionarAsync(ProdutoServico produto);
    Task SalvarAlteracoesAsync();
}
