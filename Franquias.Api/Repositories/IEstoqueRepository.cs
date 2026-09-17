using Franquias.Api.Models;

namespace Franquias.Api.Repositories;

public interface IEstoqueRepository
{
    Task<Estoque?> ObterPorUnidadeEProdutoAsync(int unidadeId, int produtoId);
    Task<List<Estoque>> ListarPorUnidadeAsync(int unidadeId);
    Task<List<Estoque>> ListarCriticosPorUnidadeAsync(int unidadeId);
    Task AdicionarAsync(Estoque estoque);
    Task RegistrarMovimentacaoAsync(Estoque estoque, MovimentacaoEstoque movimentacao);
    Task SalvarAlteracoesAsync();
}
