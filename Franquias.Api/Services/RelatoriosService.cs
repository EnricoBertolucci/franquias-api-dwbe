using Franquias.Api.DTOs;
using Franquias.Api.Models.Enums;
using Franquias.Api.Repositories;

namespace Franquias.Api.Services;

public class RelatoriosService : IRelatoriosService
{
    private readonly IRelatoriosRepository _relatoriosRepository;

    public RelatoriosService(IRelatoriosRepository relatoriosRepository)
    {
        _relatoriosRepository = relatoriosRepository;
    }

    public async Task<List<FaturamentoUnidadeDto>> ObterFaturamentoAsync(int? unidadeId, DateTime? inicio, DateTime? fim)
    {
        var (dataInicio, dataFim) = ResolverPeriodo(inicio, fim);
        return await _relatoriosRepository.ObterFaturamentoAsync(unidadeId, dataInicio, dataFim);
    }

    public async Task<List<RankingUnidadeDto>> ObterRankingUnidadesAsync(DateTime? inicio, DateTime? fim)
    {
        var (dataInicio, dataFim) = ResolverPeriodo(inicio, fim);
        var faturamentos = await _relatoriosRepository.ObterFaturamentoAsync(null, dataInicio, dataFim);

        var ranking = faturamentos
            .OrderByDescending(f => f.ValorTotal)
            .ThenBy(f => f.UnidadeFranqueadaNome)
            .Select((f, indice) => new RankingUnidadeDto
            {
                Posicao = indice + 1,
                UnidadeFranqueadaId = f.UnidadeFranqueadaId,
                UnidadeFranqueadaNome = f.UnidadeFranqueadaNome,
                QuantidadeVendas = f.QuantidadeVendas,
                ValorTotal = f.ValorTotal
            })
            .ToList();

        return ranking;
    }

    public async Task<RoyaltiesRelatorioDto> ObterRoyaltiesAsync(DateTime? inicio, DateTime? fim)
    {
        var (dataInicio, dataFim) = ResolverPeriodo(inicio, fim);
        var itens = await _relatoriosRepository.ObterRoyaltiesAsync(dataInicio, dataFim);

        return new RoyaltiesRelatorioDto
        {
            PeriodoInicio = dataInicio,
            PeriodoFim = dataFim,
            ValorTotal = itens.Sum(i => i.ValorTotal),
            Itens = itens
        };
    }

    public async Task<List<ProdutoMaisVendidoDto>> ObterProdutosMaisVendidosAsync(DateTime? inicio, DateTime? fim)
    {
        var (dataInicio, dataFim) = ResolverPeriodo(inicio, fim);
        return await _relatoriosRepository.ObterProdutosMaisVendidosAsync(dataInicio, dataFim);
    }

    public async Task<List<EstoqueCriticoDto>> ObterEstoqueCriticoAsync(int? unidadeId)
    {
        return await _relatoriosRepository.ObterEstoqueCriticoAsync(unidadeId);
    }

    public async Task<List<ChamadosPorStatusDto>> ObterChamadosPorStatusAsync(int? unidadeId, PrioridadeChamado? prioridade)
    {
        return await _relatoriosRepository.ObterChamadosPorStatusAsync(unidadeId, prioridade);
    }

    private static (DateTime Inicio, DateTime Fim) ResolverPeriodo(DateTime? inicio, DateTime? fim)
    {
        if (!inicio.HasValue && !fim.HasValue)
        {
            var hoje = DateTime.UtcNow.Date;
            var primeiroDiaMesAtual = new DateTime(hoje.Year, hoje.Month, 1, 0, 0, 0, DateTimeKind.Utc);
            return (primeiroDiaMesAtual, primeiroDiaMesAtual.AddMonths(1));
        }

        var dataInicio = inicio?.Date ?? DateTime.MinValue;
        var dataFimExclusiva = fim.HasValue ? fim.Value.Date.AddDays(1) : DateTime.UtcNow.Date.AddDays(1);

        return (dataInicio, dataFimExclusiva);
    }
}
