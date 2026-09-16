using Franquias.Api.Data;
using Franquias.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Franquias.Api.Repositories;

public class CategoriaRepository : ICategoriaRepository
{
    private readonly AppDbContext _context;

    public CategoriaRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Categoria?> ObterPorIdAsync(int id)
    {
        return await _context.Categorias.FindAsync(id);
    }

    public async Task<List<Categoria>> ListarAsync(bool? ativo)
    {
        var query = _context.Categorias.AsQueryable();

        if (ativo.HasValue)
        {
            query = query.Where(c => c.Ativo == ativo.Value);
        }

        return await query.OrderBy(c => c.Nome).ToListAsync();
    }

    public async Task<bool> ExisteNomeAsync(string nome, int? idExcluido = null)
    {
        return await _context.Categorias
            .AnyAsync(c => c.Nome == nome && c.Id != idExcluido);
    }

    public async Task<bool> PossuiProdutosVinculadosAsync(int categoriaId)
    {
        return await _context.ProdutosServicos.AnyAsync(p => p.CategoriaId == categoriaId);
    }

    public async Task AdicionarAsync(Categoria categoria)
    {
        await _context.Categorias.AddAsync(categoria);
    }

    public void Remover(Categoria categoria)
    {
        _context.Categorias.Remove(categoria);
    }

    public async Task SalvarAlteracoesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
