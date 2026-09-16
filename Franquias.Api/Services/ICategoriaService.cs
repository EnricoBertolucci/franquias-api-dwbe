using Franquias.Api.DTOs;

namespace Franquias.Api.Services;

public interface ICategoriaService
{
    Task<List<CategoriaDto>> ListarAsync(bool? ativo);
    Task<CategoriaDto> ObterPorIdAsync(int id);
    Task<CategoriaDto> CriarAsync(CategoriaDto dto);
    Task<CategoriaDto> AtualizarAsync(int id, CategoriaDto dto);
    Task RemoverAsync(int id);
}
