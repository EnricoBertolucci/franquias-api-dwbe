using Franquias.Api.Models;

namespace Franquias.Api.Repositories;

public interface IFornecedorRepository
{
    Task<Fornecedor?> ObterPorIdAsync(int id);
    Task<(List<Fornecedor> Itens, int Total)> ListarAsync(
        string? nome,
        string? cnpj,
        bool? ativo,
        int pagina,
        int tamanhoPagina,
        string? ordenarPor,
        bool decrescente);
    Task<bool> ExisteCnpjAsync(string cnpj, int? idExcluido = null);
    Task AdicionarAsync(Fornecedor fornecedor);
    Task<bool> ExisteAssociacaoAsync(int fornecedorId, int produtoId);
    Task AdicionarAssociacaoAsync(int fornecedorId, int produtoId);
    Task<bool> RemoverAssociacaoAsync(int fornecedorId, int produtoId);
    Task SalvarAlteracoesAsync();
}
