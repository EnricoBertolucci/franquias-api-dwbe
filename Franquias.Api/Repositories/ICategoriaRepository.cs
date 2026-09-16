using Franquias.Api.Models;

namespace Franquias.Api.Repositories;

public interface ICategoriaRepository
{
    Task<Categoria?> ObterPorIdAsync(int id);
    Task<List<Categoria>> ListarAsync(bool? ativo);
    Task<bool> ExisteNomeAsync(string nome, int? idExcluido = null);
    Task<bool> PossuiProdutosVinculadosAsync(int categoriaId);
    Task AdicionarAsync(Categoria categoria);
    void Remover(Categoria categoria);
    Task SalvarAlteracoesAsync();
}
