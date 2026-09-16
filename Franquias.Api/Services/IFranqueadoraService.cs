using Franquias.Api.DTOs;

namespace Franquias.Api.Services;

public interface IFranqueadoraService
{
    Task<List<FranqueadoraDto>> ListarAsync();
    Task<FranqueadoraDto> CriarAsync(FranqueadoraDto dto);
    Task<FranqueadoraDto> AtualizarAsync(int id, FranqueadoraDto dto);
}
