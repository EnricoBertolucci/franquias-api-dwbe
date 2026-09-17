using Franquias.Api.Data;
using Franquias.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Franquias.Api.Repositories;

public class VendaRepository : IVendaRepository
{
    private readonly AppDbContext _context;

    public VendaRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Venda?> ObterPorIdAsync(int id)
    {
        return await _context.Vendas
            .Include(v => v.Itens)
                .ThenInclude(i => i.ProdutoServico)
            .FirstOrDefaultAsync(v => v.Id == id);
    }

    public async Task<(List<Venda> Itens, int Total)> ListarAsync(
        int? unidadeId,
        DateTime? dataInicio,
        DateTime? dataFim,
        int pagina,
        int tamanhoPagina,
        string? ordenarPor,
        bool decrescente)
    {
        var query = _context.Vendas
            .Include(v => v.Itens)
                .ThenInclude(i => i.ProdutoServico)
            .AsQueryable();

        if (unidadeId.HasValue)
        {
            query = query.Where(v => v.UnidadeFranqueadaId == unidadeId.Value);
        }

        if (dataInicio.HasValue)
        {
            query = query.Where(v => v.DataVenda >= dataInicio.Value);
        }

        if (dataFim.HasValue)
        {
            query = query.Where(v => v.DataVenda <= dataFim.Value);
        }

        query = ordenarPor?.ToLower() switch
        {
            "valortotal" => decrescente ? query.OrderByDescending(v => v.ValorTotal) : query.OrderBy(v => v.ValorTotal),
            _ => decrescente ? query.OrderByDescending(v => v.DataVenda) : query.OrderBy(v => v.DataVenda)
        };

        var total = await query.CountAsync();

        var itens = await query
            .Skip((pagina - 1) * tamanhoPagina)
            .Take(tamanhoPagina)
            .ToListAsync();

        return (itens, total);
    }

    public async Task RegistrarVendaAsync(Venda venda, List<MovimentacaoEstoque> movimentacoes)
    {
        await using var transacao = await _context.Database.BeginTransactionAsync();
        try
        {
            await _context.Vendas.AddAsync(venda);
            await _context.MovimentacoesEstoque.AddRangeAsync(movimentacoes);
            await _context.SaveChangesAsync();
            await transacao.CommitAsync();
        }
        catch
        {
            await transacao.RollbackAsync();
            throw;
        }
    }
}
