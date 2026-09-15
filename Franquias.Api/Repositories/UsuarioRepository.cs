using Franquias.Api.Data;
using Franquias.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Franquias.Api.Repositories;

public class UsuarioRepository : IUsuarioRepository
{
    private readonly AppDbContext _context;

    public UsuarioRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Usuario?> ObterPorIdAsync(int id)
    {
        return await _context.Usuarios.FindAsync(id);
    }

    public async Task<Usuario?> ObterPorEmailAsync(string email)
    {
        return await _context.Usuarios
            .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
    }

    public async Task<List<Usuario>> ListarAsync()
    {
        return await _context.Usuarios
            .OrderBy(u => u.Nome)
            .ToListAsync();
    }

    public async Task<bool> ExisteEmailAsync(string email, int? idExcluido = null)
    {
        return await _context.Usuarios
            .AnyAsync(u => u.Email.ToLower() == email.ToLower() && u.Id != idExcluido);
    }

    public async Task AdicionarAsync(Usuario usuario)
    {
        await _context.Usuarios.AddAsync(usuario);
    }

    public async Task SalvarAlteracoesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
