using Franquias.Api.Data;
using Franquias.Api.Models;
using Franquias.Api.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace Franquias.Api.Repositories;

public class ChamadoRepository : IChamadoRepository
{
    private readonly AppDbContext _context;

    public ChamadoRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<ChamadoSuporte?> ObterPorIdAsync(int id)
    {
        return await _context.ChamadosSuporte
            .Include(c => c.UnidadeFranqueada)
            .Include(c => c.Usuario)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<(List<ChamadoSuporte> Itens, int Total)> ListarAsync(
        int? unidadeId,
        StatusChamado? status,
        PrioridadeChamado? prioridade,
        int pagina,
        int tamanhoPagina,
        string? ordenarPor,
        bool decrescente)
    {
        var query = _context.ChamadosSuporte
            .Include(c => c.UnidadeFranqueada)
            .Include(c => c.Usuario)
            .AsQueryable();

        if (unidadeId.HasValue)
        {
            query = query.Where(c => c.UnidadeFranqueadaId == unidadeId.Value);
        }

        if (status.HasValue)
        {
            query = query.Where(c => c.Status == status.Value);
        }

        if (prioridade.HasValue)
        {
            query = query.Where(c => c.Prioridade == prioridade.Value);
        }

        query = ordenarPor?.ToLower() switch
        {
            "prioridade" => decrescente ? query.OrderByDescending(c => c.Prioridade) : query.OrderBy(c => c.Prioridade),
            "status" => decrescente ? query.OrderByDescending(c => c.Status) : query.OrderBy(c => c.Status),
            _ => decrescente ? query.OrderByDescending(c => c.DataAbertura) : query.OrderBy(c => c.DataAbertura)
        };

        var total = await query.CountAsync();

        var itens = await query
            .Skip((pagina - 1) * tamanhoPagina)
            .Take(tamanhoPagina)
            .ToListAsync();

        return (itens, total);
    }

    public async Task AdicionarAsync(ChamadoSuporte chamado)
    {
        await _context.ChamadosSuporte.AddAsync(chamado);
    }

    public async Task SalvarAlteracoesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
