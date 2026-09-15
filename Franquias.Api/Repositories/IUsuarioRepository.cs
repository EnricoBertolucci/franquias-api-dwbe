using Franquias.Api.Models;

namespace Franquias.Api.Repositories;

public interface IUsuarioRepository
{
    Task<Usuario?> ObterPorIdAsync(int id);
    Task<Usuario?> ObterPorEmailAsync(string email);
    Task<List<Usuario>> ListarAsync();
    Task<bool> ExisteEmailAsync(string email, int? idExcluido = null);
    Task AdicionarAsync(Usuario usuario);
    Task SalvarAlteracoesAsync();
}
