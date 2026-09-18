using Franquias.Api.Data;
using Franquias.Api.DTOs;
using Franquias.Api.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace Franquias.Api.Repositories;

public class RelatoriosRepository : IRelatoriosRepository
{
    private readonly AppDbContext _context;

    public RelatoriosRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<FaturamentoUnidadeDto>> ObterFaturamentoAsync(int? unidadeId, DateTime inicio, DateTime fim)
    {
        var query = _context.Vendas
            .Where(v => v.Status == StatusVenda.Confirmada && v.DataVenda >= inicio && v.DataVenda < fim);

        if (unidadeId.HasValue)
        {
            query = query.Where(v => v.UnidadeFranqueadaId == unidadeId.Value);
        }

        var itens = await query
            .GroupBy(v => new { v.UnidadeFranqueadaId, v.UnidadeFranqueada.Nome })
            .Select(g => new FaturamentoUnidadeDto
            {
                UnidadeFranqueadaId = g.Key.UnidadeFranqueadaId,
                UnidadeFranqueadaNome = g.Key.Nome,
                QuantidadeVendas = g.Count(),
                ValorTotal = g.Sum(v => v.ValorTotal)
            })
            .OrderBy(f => f.UnidadeFranqueadaNome)
            .ToListAsync();

        foreach (var item in itens)
        {
            item.PeriodoInicio = inicio;
            item.PeriodoFim = fim;
        }

        return itens;
    }

    public async Task<List<RoyaltyPorUnidadeDto>> ObterRoyaltiesAsync(DateTime inicio, DateTime fim)
    {
        var chaveInicio = (inicio.Year * 100) + inicio.Month;
        var chaveFimExclusiva = (fim.Year * 100) + fim.Month;

        return await _context.Royalties
            .Where(r => (r.AnoReferencia * 100) + r.MesReferencia >= chaveInicio
                && (r.AnoReferencia * 100) + r.MesReferencia < chaveFimExclusiva)
            .GroupBy(r => new { r.UnidadeFranqueadaId, r.UnidadeFranqueada.Nome })
            .Select(g => new RoyaltyPorUnidadeDto
            {
                UnidadeFranqueadaId = g.Key.UnidadeFranqueadaId,
                UnidadeFranqueadaNome = g.Key.Nome,
                QuantidadeRoyalties = g.Count(),
                ValorTotal = g.Sum(r => r.ValorCalculado)
            })
            .OrderBy(r => r.UnidadeFranqueadaNome)
            .ToListAsync();
    }

    public async Task<List<ProdutoMaisVendidoDto>> ObterProdutosMaisVendidosAsync(DateTime inicio, DateTime fim)
    {
        return await _context.ItensVenda
            .Where(i => i.Venda.Status == StatusVenda.Confirmada
                && i.Venda.DataVenda >= inicio && i.Venda.DataVenda < fim)
            .GroupBy(i => new { i.ProdutoServicoId, i.ProdutoServico.Nome })
            .Select(g => new ProdutoMaisVendidoDto
            {
                ProdutoServicoId = g.Key.ProdutoServicoId,
                ProdutoServicoNome = g.Key.Nome,
                QuantidadeVendida = g.Sum(i => i.Quantidade),
                ValorTotalVendido = g.Sum(i => i.Subtotal)
            })
            .OrderByDescending(p => p.QuantidadeVendida)
            .ThenByDescending(p => p.ValorTotalVendido)
            .ToListAsync();
    }

    public async Task<List<EstoqueCriticoDto>> ObterEstoqueCriticoAsync(int? unidadeId)
    {
        var query = _context.Estoques
            .Include(e => e.UnidadeFranqueada)
            .Include(e => e.ProdutoServico)
            .Where(e => e.QuantidadeAtual < e.QuantidadeMinima);

        if (unidadeId.HasValue)
        {
            query = query.Where(e => e.UnidadeFranqueadaId == unidadeId.Value);
        }

        return await query
            .OrderBy(e => e.UnidadeFranqueada.Nome)
            .ThenBy(e => e.ProdutoServico.Nome)
            .Select(e => new EstoqueCriticoDto
            {
                UnidadeFranqueadaId = e.UnidadeFranqueadaId,
                UnidadeFranqueadaNome = e.UnidadeFranqueada.Nome,
                ProdutoServicoId = e.ProdutoServicoId,
                ProdutoServicoNome = e.ProdutoServico.Nome,
                QuantidadeAtual = e.QuantidadeAtual,
                QuantidadeMinima = e.QuantidadeMinima
            })
            .ToListAsync();
    }

    public async Task<List<ChamadosPorStatusDto>> ObterChamadosPorStatusAsync(int? unidadeId, PrioridadeChamado? prioridade)
    {
        var query = _context.ChamadosSuporte.AsQueryable();

        if (unidadeId.HasValue)
        {
            query = query.Where(c => c.UnidadeFranqueadaId == unidadeId.Value);
        }

        if (prioridade.HasValue)
        {
            query = query.Where(c => c.Prioridade == prioridade.Value);
        }

        return await query
            .GroupBy(c => c.Status)
            .Select(g => new ChamadosPorStatusDto
            {
                Status = g.Key,
                Quantidade = g.Count()
            })
            .OrderBy(c => c.Status)
            .ToListAsync();
    }
}
