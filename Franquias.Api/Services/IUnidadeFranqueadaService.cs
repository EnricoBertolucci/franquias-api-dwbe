using Franquias.Api.DTOs;

namespace Franquias.Api.Services;

public interface IUnidadeFranqueadaService
{
    Task<ResultadoPaginadoDto<UnidadeFranqueadaRespostaDto>> ListarAsync(
        string? nome,
        string? cidade,
        string? cnpj,
        string? responsavel,
        int pagina,
        int tamanhoPagina,
        string? ordenarPor,
        bool decrescente);

    Task<UnidadeFranqueadaRespostaDto> ObterPorIdAsync(int id);
    Task<UnidadeFranqueadaRespostaDto> CriarAsync(UnidadeFranqueadaCriacaoDto dto);
    Task<UnidadeFranqueadaRespostaDto> AtualizarAsync(int id, UnidadeFranqueadaAtualizacaoDto dto);
    Task InativarAsync(int id);
    Task AtivarAsync(int id);
}
