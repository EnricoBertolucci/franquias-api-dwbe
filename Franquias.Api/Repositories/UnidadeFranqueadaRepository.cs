using Franquias.Api.Data;
using Franquias.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Franquias.Api.Repositories;

public class UnidadeFranqueadaRepository : IUnidadeFranqueadaRepository
{
    private readonly AppDbContext _context;

    public UnidadeFranqueadaRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<UnidadeFranqueada?> ObterPorIdAsync(int id)
    {
        return await _context.UnidadesFranqueadas
            .Include(u => u.Responsaveis)
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<(List<UnidadeFranqueada> Itens, int Total)> ListarAsync(
        string? nome,
        string? cidade,
        string? cnpj,
        string? responsavel,
        int pagina,
        int tamanhoPagina,
        string? ordenarPor,
        bool decrescente)
    {
        var query = _context.UnidadesFranqueadas
            .Include(u => u.Responsaveis)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(nome))
        {
            query = query.Where(u => u.Nome.Contains(nome));
        }

        if (!string.IsNullOrWhiteSpace(cidade))
        {
            query = query.Where(u => u.Cidade.Contains(cidade));
        }

        if (!string.IsNullOrWhiteSpace(cnpj))
        {
            query = query.Where(u => u.Cnpj.Contains(cnpj));
        }

        if (!string.IsNullOrWhiteSpace(responsavel))
        {
            query = query.Where(u => u.Responsaveis.Any(r => r.Nome.Contains(responsavel)));
        }

        query = ordenarPor?.ToLower() switch
        {
            "cidade" => decrescente ? query.OrderByDescending(u => u.Cidade) : query.OrderBy(u => u.Cidade),
            "datainicio" => decrescente ? query.OrderByDescending(u => u.DataInicio) : query.OrderBy(u => u.DataInicio),
            "cnpj" => decrescente ? query.OrderByDescending(u => u.Cnpj) : query.OrderBy(u => u.Cnpj),
            _ => decrescente ? query.OrderByDescending(u => u.Nome) : query.OrderBy(u => u.Nome)
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
        return await _context.UnidadesFranqueadas
            .AnyAsync(u => u.Cnpj == cnpj && u.Id != idExcluido);
    }

    public async Task AdicionarAsync(UnidadeFranqueada unidade)
    {
        await _context.UnidadesFranqueadas.AddAsync(unidade);
    }

    public async Task SalvarAlteracoesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
