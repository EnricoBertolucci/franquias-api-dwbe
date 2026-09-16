using Franquias.Api.Models;

namespace Franquias.Api.Repositories;

public interface IFranqueadoraRepository
{
    Task<Franqueadora?> ObterPorIdAsync(int id);
    Task<List<Franqueadora>> ListarAsync();
    Task<bool> ExisteCnpjAsync(string cnpj, int? idExcluido = null);
    Task AdicionarAsync(Franqueadora franqueadora);
    Task SalvarAlteracoesAsync();
}
