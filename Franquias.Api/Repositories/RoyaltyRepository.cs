using Franquias.Api.Data;
using Franquias.Api.Models;
using Franquias.Api.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace Franquias.Api.Repositories;

public class RoyaltyRepository : IRoyaltyRepository
{
    private readonly AppDbContext _context;

    public RoyaltyRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Royalty?> ObterPorIdAsync(int id)
    {
        return await _context.Royalties
            .Include(r => r.UnidadeFranqueada)
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<Royalty?> ObterPorUnidadeEPeriodoAsync(int unidadeId, int anoReferencia, int mesReferencia)
    {
        return await _context.Royalties
            .Include(r => r.UnidadeFranqueada)
            .FirstOrDefaultAsync(r =>
                r.UnidadeFranqueadaId == unidadeId &&
                r.AnoReferencia == anoReferencia &&
                r.MesReferencia == mesReferencia);
    }

    public async Task<decimal> ObterFaturamentoUnidadeAsync(int unidadeId, DateTime inicio, DateTime fim)
    {
        return await _context.Vendas
            .Where(v =>
                v.UnidadeFranqueadaId == unidadeId &&
                v.Status == StatusVenda.Confirmada &&
                v.DataVenda >= inicio &&
                v.DataVenda < fim)
            .SumAsync(v => (decimal?)v.ValorTotal) ?? 0m;
    }

    public async Task<(List<Royalty> Itens, int Total)> ListarAsync(
        int? unidadeId,
        int? anoReferencia,
        int? mesReferencia,
        StatusPagamento? statusPagamento,
        int pagina,
        int tamanhoPagina,
        string? ordenarPor,
        bool decrescente)
    {
        var query = _context.Royalties
            .Include(r => r.UnidadeFranqueada)
            .AsQueryable();

        if (unidadeId.HasValue)
        {
            query = query.Where(r => r.UnidadeFranqueadaId == unidadeId.Value);
        }

        if (anoReferencia.HasValue)
        {
            query = query.Where(r => r.AnoReferencia == anoReferencia.Value);
        }

        if (mesReferencia.HasValue)
        {
            query = query.Where(r => r.MesReferencia == mesReferencia.Value);
        }

        if (statusPagamento.HasValue)
        {
            query = query.Where(r => r.StatusPagamento == statusPagamento.Value);
        }

        query = ordenarPor?.ToLower() switch
        {
            "valorcalculado" => decrescente ? query.OrderByDescending(r => r.ValorCalculado) : query.OrderBy(r => r.ValorCalculado),
            "vencimento" => decrescente ? query.OrderByDescending(r => r.DataVencimento) : query.OrderBy(r => r.DataVencimento),
            _ => decrescente
                ? query.OrderByDescending(r => r.AnoReferencia).ThenByDescending(r => r.MesReferencia)
                : query.OrderBy(r => r.AnoReferencia).ThenBy(r => r.MesReferencia)
        };

        var total = await query.CountAsync();

        var itens = await query
            .Skip((pagina - 1) * tamanhoPagina)
            .Take(tamanhoPagina)
            .ToListAsync();

        return (itens, total);
    }

    public async Task AdicionarAsync(Royalty royalty)
    {
        await _context.Royalties.AddAsync(royalty);
    }

    public async Task SalvarAlteracoesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
