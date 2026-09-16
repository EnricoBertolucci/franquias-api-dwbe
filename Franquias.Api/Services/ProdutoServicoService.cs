using Franquias.Api.DTOs;
using Franquias.Api.Models;
using Franquias.Api.Repositories;

namespace Franquias.Api.Services;

public class ProdutoServicoService : IProdutoServicoService
{
    private readonly IProdutoServicoRepository _produtoRepository;
    private readonly ICategoriaRepository _categoriaRepository;

    public ProdutoServicoService(
        IProdutoServicoRepository produtoRepository,
        ICategoriaRepository categoriaRepository)
    {
        _produtoRepository = produtoRepository;
        _categoriaRepository = categoriaRepository;
    }

    public async Task<ResultadoPaginadoDto<ProdutoServicoRespostaDto>> ListarAsync(
        string? nome,
        int? categoriaId,
        bool? ativo,
        int pagina,
        int tamanhoPagina,
        string? ordenarPor,
        bool decrescente)
    {
        pagina = pagina < 1 ? 1 : pagina;
        tamanhoPagina = tamanhoPagina is < 1 or > 100 ? 10 : tamanhoPagina;

        var (itens, total) = await _produtoRepository.ListarAsync(
            nome, categoriaId, ativo, pagina, tamanhoPagina, ordenarPor, decrescente);

        return new ResultadoPaginadoDto<ProdutoServicoRespostaDto>
        {
            Itens = itens.Select(MapearParaDto).ToList(),
            PaginaAtual = pagina,
            TamanhoPagina = tamanhoPagina,
            TotalRegistros = total
        };
    }

    public async Task<ProdutoServicoRespostaDto> ObterPorIdAsync(int id)
    {
        var produto = await _produtoRepository.ObterPorIdAsync(id)
            ?? throw new KeyNotFoundException("Produto/serviço não encontrado.");

        return MapearParaDto(produto);
    }

    public async Task<ProdutoServicoRespostaDto> CriarAsync(ProdutoServicoCriacaoDto dto)
    {
        var categoria = await _categoriaRepository.ObterPorIdAsync(dto.CategoriaId)
            ?? throw new KeyNotFoundException("Categoria não encontrada.");

        var produto = new ProdutoServico
        {
            CategoriaId = dto.CategoriaId,
            Categoria = categoria,
            Nome = dto.Nome,
            Descricao = dto.Descricao,
            Tipo = dto.Tipo,
            PrecoBase = dto.PrecoBase,
            Ativo = true
        };

        await _produtoRepository.AdicionarAsync(produto);
        await _produtoRepository.SalvarAlteracoesAsync();

        return MapearParaDto(produto);
    }

    public async Task<ProdutoServicoRespostaDto> AtualizarAsync(int id, ProdutoServicoAtualizacaoDto dto)
    {
        var produto = await _produtoRepository.ObterPorIdAsync(id)
            ?? throw new KeyNotFoundException("Produto/serviço não encontrado.");

        var categoria = await _categoriaRepository.ObterPorIdAsync(dto.CategoriaId)
            ?? throw new KeyNotFoundException("Categoria não encontrada.");

        produto.CategoriaId = dto.CategoriaId;
        produto.Categoria = categoria;
        produto.Nome = dto.Nome;
        produto.Descricao = dto.Descricao;
        produto.Tipo = dto.Tipo;
        produto.PrecoBase = dto.PrecoBase;

        await _produtoRepository.SalvarAlteracoesAsync();

        return MapearParaDto(produto);
    }

    public async Task InativarAsync(int id)
    {
        var produto = await _produtoRepository.ObterPorIdAsync(id)
            ?? throw new KeyNotFoundException("Produto/serviço não encontrado.");

        produto.Ativo = false;
        await _produtoRepository.SalvarAlteracoesAsync();
    }

    public async Task AtivarAsync(int id)
    {
        var produto = await _produtoRepository.ObterPorIdAsync(id)
            ?? throw new KeyNotFoundException("Produto/serviço não encontrado.");

        produto.Ativo = true;
        await _produtoRepository.SalvarAlteracoesAsync();
    }

    private static ProdutoServicoRespostaDto MapearParaDto(ProdutoServico produto)
    {
        return new ProdutoServicoRespostaDto
        {
            Id = produto.Id,
            CategoriaId = produto.CategoriaId,
            CategoriaNome = produto.Categoria.Nome,
            Nome = produto.Nome,
            Descricao = produto.Descricao,
            Tipo = produto.Tipo,
            PrecoBase = produto.PrecoBase,
            Ativo = produto.Ativo
        };
    }
}
