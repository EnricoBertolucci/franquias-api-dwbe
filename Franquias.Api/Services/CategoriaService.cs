using Franquias.Api.DTOs;
using Franquias.Api.Models;
using Franquias.Api.Repositories;

namespace Franquias.Api.Services;

public class CategoriaService : ICategoriaService
{
    private readonly ICategoriaRepository _categoriaRepository;

    public CategoriaService(ICategoriaRepository categoriaRepository)
    {
        _categoriaRepository = categoriaRepository;
    }

    public async Task<List<CategoriaDto>> ListarAsync(bool? ativo)
    {
        var categorias = await _categoriaRepository.ListarAsync(ativo);
        return categorias.Select(MapearParaDto).ToList();
    }

    public async Task<CategoriaDto> ObterPorIdAsync(int id)
    {
        var categoria = await _categoriaRepository.ObterPorIdAsync(id)
            ?? throw new KeyNotFoundException("Categoria não encontrada.");

        return MapearParaDto(categoria);
    }

    public async Task<CategoriaDto> CriarAsync(CategoriaDto dto)
    {
        if (await _categoriaRepository.ExisteNomeAsync(dto.Nome))
        {
            throw new InvalidOperationException("Já existe uma categoria cadastrada com este nome.");
        }

        var categoria = new Categoria
        {
            Nome = dto.Nome,
            Descricao = dto.Descricao,
            Ativo = true
        };

        await _categoriaRepository.AdicionarAsync(categoria);
        await _categoriaRepository.SalvarAlteracoesAsync();

        return MapearParaDto(categoria);
    }

    public async Task<CategoriaDto> AtualizarAsync(int id, CategoriaDto dto)
    {
        var categoria = await _categoriaRepository.ObterPorIdAsync(id)
            ?? throw new KeyNotFoundException("Categoria não encontrada.");

        if (await _categoriaRepository.ExisteNomeAsync(dto.Nome, id))
        {
            throw new InvalidOperationException("Já existe uma categoria cadastrada com este nome.");
        }

        categoria.Nome = dto.Nome;
        categoria.Descricao = dto.Descricao;
        categoria.Ativo = dto.Ativo;

        await _categoriaRepository.SalvarAlteracoesAsync();

        return MapearParaDto(categoria);
    }

    public async Task RemoverAsync(int id)
    {
        var categoria = await _categoriaRepository.ObterPorIdAsync(id)
            ?? throw new KeyNotFoundException("Categoria não encontrada.");

        if (await _categoriaRepository.PossuiProdutosVinculadosAsync(id))
        {
            throw new InvalidOperationException("Não é possível remover uma categoria que possui produtos/serviços vinculados.");
        }

        _categoriaRepository.Remover(categoria);
        await _categoriaRepository.SalvarAlteracoesAsync();
    }

    private static CategoriaDto MapearParaDto(Categoria categoria)
    {
        return new CategoriaDto
        {
            Id = categoria.Id,
            Nome = categoria.Nome,
            Descricao = categoria.Descricao,
            Ativo = categoria.Ativo
        };
    }
}
