using Franquias.Api.Models;
using Franquias.Api.Models.Enums;

namespace Franquias.Api.Repositories;

public interface IChamadoRepository
{
    Task<ChamadoSuporte?> ObterPorIdAsync(int id);
    Task<(List<ChamadoSuporte> Itens, int Total)> ListarAsync(
        int? unidadeId,
        StatusChamado? status,
        PrioridadeChamado? prioridade,
        int pagina,
        int tamanhoPagina,
        string? ordenarPor,
        bool decrescente);
    Task AdicionarAsync(ChamadoSuporte chamado);
    Task SalvarAlteracoesAsync();
}
