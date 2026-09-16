using Franquias.Api.Data;
using Franquias.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Franquias.Api.Repositories;

public class ProdutoServicoRepository : IProdutoServicoRepository
{
    private readonly AppDbContext _context;

    public ProdutoServicoRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<ProdutoServico?> ObterPorIdAsync(int id)
    {
        return await _context.ProdutosServicos
            .Include(p => p.Categoria)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<(List<ProdutoServico> Itens, int Total)> ListarAsync(
        string? nome,
        int? categoriaId,
        bool? ativo,
        int pagina,
        int tamanhoPagina,
        string? ordenarPor,
        bool decrescente)
    {
        var query = _context.ProdutosServicos
            .Include(p => p.Categoria)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(nome))
        {
            query = query.Where(p => p.Nome.Contains(nome));
        }

        if (categoriaId.HasValue)
        {
            query = query.Where(p => p.CategoriaId == categoriaId.Value);
        }

        if (ativo.HasValue)
        {
            query = query.Where(p => p.Ativo == ativo.Value);
        }

        query = ordenarPor?.ToLower() switch
        {
            "preco" or "precobase" => decrescente ? query.OrderByDescending(p => p.PrecoBase) : query.OrderBy(p => p.PrecoBase),
            "categoria" => decrescente ? query.OrderByDescending(p => p.Categoria.Nome) : query.OrderBy(p => p.Categoria.Nome),
            _ => decrescente ? query.OrderByDescending(p => p.Nome) : query.OrderBy(p => p.Nome)
        };

        var total = await query.CountAsync();

        var itens = await query
            .Skip((pagina - 1) * tamanhoPagina)
            .Take(tamanhoPagina)
            .ToListAsync();

        return (itens, total);
    }

    public async Task AdicionarAsync(ProdutoServico produto)
    {
        await _context.ProdutosServicos.AddAsync(produto);
    }

    public async Task SalvarAlteracoesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
