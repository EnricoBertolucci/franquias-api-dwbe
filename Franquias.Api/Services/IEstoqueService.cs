using Franquias.Api.DTOs;

namespace Franquias.Api.Services;

public interface IEstoqueService
{
    Task<List<EstoqueRespostaDto>> ListarPorUnidadeAsync(int unidadeId);
    Task<EstoqueRespostaDto> ObterPorUnidadeEProdutoAsync(int unidadeId, int produtoId);
    Task<List<EstoqueRespostaDto>> ListarCriticosPorUnidadeAsync(int unidadeId);
    Task<MovimentacaoEstoqueRespostaDto> RegistrarMovimentacaoAsync(MovimentacaoEstoqueCriacaoDto dto, int usuarioId);
    Task<EstoqueRespostaDto> AtualizarEstoqueMinimoAsync(int unidadeId, int produtoId, int quantidadeMinima);
}
