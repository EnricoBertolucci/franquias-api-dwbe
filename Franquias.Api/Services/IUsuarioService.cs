using Franquias.Api.DTOs;

namespace Franquias.Api.Services;

public interface IUsuarioService
{
    Task<UsuarioRespostaDto> CriarAsync(UsuarioCriacaoDto dto);
    Task<List<UsuarioRespostaDto>> ListarAsync();
    Task<UsuarioRespostaDto> ObterPorIdAsync(int id);
    Task<UsuarioRespostaDto> AtualizarAsync(int id, UsuarioAtualizacaoDto dto);
    Task InativarAsync(int id);
    Task AtivarAsync(int id);
}
