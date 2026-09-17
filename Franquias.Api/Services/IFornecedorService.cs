using Franquias.Api.DTOs;

namespace Franquias.Api.Services;

public interface IFornecedorService
{
    Task<ResultadoPaginadoDto<FornecedorRespostaDto>> ListarAsync(
        string? nome,
        string? cnpj,
        bool? ativo,
        int pagina,
        int tamanhoPagina,
        string? ordenarPor,
        bool decrescente);

    Task<FornecedorRespostaDto> ObterPorIdAsync(int id);
    Task<FornecedorRespostaDto> CriarAsync(FornecedorCriacaoDto dto);
    Task<FornecedorRespostaDto> AtualizarAsync(int id, FornecedorAtualizacaoDto dto);
    Task InativarAsync(int id);
    Task AtivarAsync(int id);
    Task AssociarProdutoAsync(int fornecedorId, int produtoId);
    Task DesassociarProdutoAsync(int fornecedorId, int produtoId);
}
