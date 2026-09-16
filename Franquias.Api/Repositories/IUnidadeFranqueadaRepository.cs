using Franquias.Api.Models;

namespace Franquias.Api.Repositories;

public interface IUnidadeFranqueadaRepository
{
    Task<UnidadeFranqueada?> ObterPorIdAsync(int id);
    Task<(List<UnidadeFranqueada> Itens, int Total)> ListarAsync(
        string? nome,
        string? cidade,
        string? cnpj,
        string? responsavel,
        int pagina,
        int tamanhoPagina,
        string? ordenarPor,
        bool decrescente);
    Task<bool> ExisteCnpjAsync(string cnpj, int? idExcluido = null);
    Task AdicionarAsync(UnidadeFranqueada unidade);
    Task SalvarAlteracoesAsync();
}
