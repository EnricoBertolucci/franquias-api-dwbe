using Franquias.Api.Data;
using Franquias.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Franquias.Api.Repositories;

public class FornecedorRepository : IFornecedorRepository
{
    private readonly AppDbContext _context;

    public FornecedorRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Fornecedor?> ObterPorIdAsync(int id)
    {
        return await _context.Fornecedores
            .Include(f => f.Produtos)
                .ThenInclude(fp => fp.ProdutoServico)
            .FirstOrDefaultAsync(f => f.Id == id);
    }

    public async Task<(List<Fornecedor> Itens, int Total)> ListarAsync(
        string? nome,
        string? cnpj,
        bool? ativo,
        int pagina,
        int tamanhoPagina,
        string? ordenarPor,
        bool decrescente)
    {
        var query = _context.Fornecedores
            .Include(f => f.Produtos)
                .ThenInclude(fp => fp.ProdutoServico)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(nome))
        {
            query = query.Where(f => f.RazaoSocial.Contains(nome));
        }

        if (!string.IsNullOrWhiteSpace(cnpj))
        {
            query = query.Where(f => f.Cnpj.Contains(cnpj));
        }

        if (ativo.HasValue)
        {
            query = query.Where(f => f.Ativo == ativo.Value);
        }

        query = ordenarPor?.ToLower() switch
        {
            "cnpj" => decrescente ? query.OrderByDescending(f => f.Cnpj) : query.OrderBy(f => f.Cnpj),
            _ => decrescente ? query.OrderByDescending(f => f.RazaoSocial) : query.OrderBy(f => f.RazaoSocial)
        };

        var total = await query.CountAsync();

        var itens = await query
            .Skip((pagina - 1) * tamanhoPagina)
            .Take(tamanhoPagina)
            .ToListAsync();

        return (itens, total);
    }

    public async Task<bool> ExisteCnpjAsync(string cnpj, int? idExcluido = null)
    {
        return await _context.Fornecedores
            .AnyAsync(f => f.Cnpj == cnpj && f.Id != idExcluido);
    }

    public async Task AdicionarAsync(Fornecedor fornecedor)
    {
        await _context.Fornecedores.AddAsync(fornecedor);
    }

    public async Task<bool> ExisteAssociacaoAsync(int fornecedorId, int produtoId)
    {
        return await _context.FornecedoresProdutos
            .AnyAsync(fp => fp.FornecedorId == fornecedorId && fp.ProdutoServicoId == produtoId);
    }

    public async Task AdicionarAssociacaoAsync(int fornecedorId, int produtoId)
    {
        await _context.FornecedoresProdutos.AddAsync(new FornecedorProduto
        {
            FornecedorId = fornecedorId,
            ProdutoServicoId = produtoId
        });
    }

    public async Task<bool> RemoverAssociacaoAsync(int fornecedorId, int produtoId)
    {
        var associacao = await _context.FornecedoresProdutos
            .FirstOrDefaultAsync(fp => fp.FornecedorId == fornecedorId && fp.ProdutoServicoId == produtoId);

        if (associacao is null)
        {
            return false;
        }

        _context.FornecedoresProdutos.Remove(associacao);
        return true;
    }

    public async Task SalvarAlteracoesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
