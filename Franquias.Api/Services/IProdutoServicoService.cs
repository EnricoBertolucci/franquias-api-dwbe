using Franquias.Api.DTOs;

namespace Franquias.Api.Services;

public interface IProdutoServicoService
{
    Task<ResultadoPaginadoDto<ProdutoServicoRespostaDto>> ListarAsync(
        string? nome,
        int? categoriaId,
        bool? ativo,
        int pagina,
        int tamanhoPagina,
        string? ordenarPor,
        bool decrescente);

    Task<ProdutoServicoRespostaDto> ObterPorIdAsync(int id);
    Task<ProdutoServicoRespostaDto> CriarAsync(ProdutoServicoCriacaoDto dto);
    Task<ProdutoServicoRespostaDto> AtualizarAsync(int id, ProdutoServicoAtualizacaoDto dto);
    Task InativarAsync(int id);
    Task AtivarAsync(int id);
}
