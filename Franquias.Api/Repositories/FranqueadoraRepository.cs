using Franquias.Api.Data;
using Franquias.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Franquias.Api.Repositories;

public class FranqueadoraRepository : IFranqueadoraRepository
{
    private readonly AppDbContext _context;

    public FranqueadoraRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Franqueadora?> ObterPorIdAsync(int id)
    {
        return await _context.Franqueadoras.FindAsync(id);
    }

    public async Task<List<Franqueadora>> ListarAsync()
    {
        return await _context.Franqueadoras
            .OrderBy(f => f.NomeFantasia)
            .ToListAsync();
    }

    public async Task<bool> ExisteCnpjAsync(string cnpj, int? idExcluido = null)
    {
        return await _context.Franqueadoras
            .AnyAsync(f => f.Cnpj == cnpj && f.Id != idExcluido);
    }

    public async Task AdicionarAsync(Franqueadora franqueadora)
    {
        await _context.Franqueadoras.AddAsync(franqueadora);
    }

    public async Task SalvarAlteracoesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
