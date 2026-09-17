using Franquias.Api.Data;
using Franquias.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Franquias.Api.Repositories;

public class EstoqueRepository : IEstoqueRepository
{
    private readonly AppDbContext _context;

    public EstoqueRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Estoque?> ObterPorUnidadeEProdutoAsync(int unidadeId, int produtoId)
    {
        return await _context.Estoques
            .Include(e => e.ProdutoServico)
            .FirstOrDefaultAsync(e => e.UnidadeFranqueadaId == unidadeId && e.ProdutoServicoId == produtoId);
    }

    public async Task<List<Estoque>> ListarPorUnidadeAsync(int unidadeId)
    {
        return await _context.Estoques
            .Include(e => e.ProdutoServico)
            .Where(e => e.UnidadeFranqueadaId == unidadeId)
            .OrderBy(e => e.ProdutoServico.Nome)
            .ToListAsync();
    }

    public async Task<List<Estoque>> ListarCriticosPorUnidadeAsync(int unidadeId)
    {
        return await _context.Estoques
            .Include(e => e.ProdutoServico)
            .Where(e => e.UnidadeFranqueadaId == unidadeId && e.QuantidadeAtual < e.QuantidadeMinima)
            .OrderBy(e => e.ProdutoServico.Nome)
            .ToListAsync();
    }

    public async Task AdicionarAsync(Estoque estoque)
    {
        await _context.Estoques.AddAsync(estoque);
    }

    public async Task RegistrarMovimentacaoAsync(Estoque estoque, MovimentacaoEstoque movimentacao)
    {
        await using var transacao = await _context.Database.BeginTransactionAsync();
        try
        {
            if (estoque.Id == 0)
            {
                await _context.Estoques.AddAsync(estoque);
            }

            await _context.MovimentacoesEstoque.AddAsync(movimentacao);
            await _context.SaveChangesAsync();
            await transacao.CommitAsync();
        }
        catch
        {
            await transacao.RollbackAsync();
            throw;
        }
    }

    public async Task SalvarAlteracoesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
