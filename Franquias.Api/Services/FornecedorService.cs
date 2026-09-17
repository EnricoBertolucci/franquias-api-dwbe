using Franquias.Api.DTOs;
using Franquias.Api.Models;
using Franquias.Api.Repositories;

namespace Franquias.Api.Services;

public class FornecedorService : IFornecedorService
{
    private readonly IFornecedorRepository _fornecedorRepository;
    private readonly IProdutoServicoRepository _produtoRepository;

    public FornecedorService(
        IFornecedorRepository fornecedorRepository,
        IProdutoServicoRepository produtoRepository)
    {
        _fornecedorRepository = fornecedorRepository;
        _produtoRepository = produtoRepository;
    }

    public async Task<ResultadoPaginadoDto<FornecedorRespostaDto>> ListarAsync(
        string? nome,
        string? cnpj,
        bool? ativo,
        int pagina,
        int tamanhoPagina,
        string? ordenarPor,
        bool decrescente)
    {
        pagina = pagina < 1 ? 1 : pagina;
        tamanhoPagina = tamanhoPagina is < 1 or > 100 ? 10 : tamanhoPagina;

        var (itens, total) = await _fornecedorRepository.ListarAsync(
            nome, cnpj, ativo, pagina, tamanhoPagina, ordenarPor, decrescente);

        return new ResultadoPaginadoDto<FornecedorRespostaDto>
        {
            Itens = itens.Select(MapearParaDto).ToList(),
            PaginaAtual = pagina,
            TamanhoPagina = tamanhoPagina,
            TotalRegistros = total
        };
    }

    public async Task<FornecedorRespostaDto> ObterPorIdAsync(int id)
    {
        var fornecedor = await _fornecedorRepository.ObterPorIdAsync(id)
            ?? throw new KeyNotFoundException("Fornecedor não encontrado.");

        return MapearParaDto(fornecedor);
    }

    public async Task<FornecedorRespostaDto> CriarAsync(FornecedorCriacaoDto dto)
    {
        if (await _fornecedorRepository.ExisteCnpjAsync(dto.Cnpj))
        {
            throw new InvalidOperationException("Já existe um fornecedor cadastrado com este CNPJ.");
        }

        var fornecedor = new Fornecedor
        {
            RazaoSocial = dto.RazaoSocial,
            Cnpj = dto.Cnpj,
            Email = dto.Email,
            Telefone = dto.Telefone,
            Ativo = true
        };

        await _fornecedorRepository.AdicionarAsync(fornecedor);
        await _fornecedorRepository.SalvarAlteracoesAsync();

        return MapearParaDto(fornecedor);
    }

    public async Task<FornecedorRespostaDto> AtualizarAsync(int id, FornecedorAtualizacaoDto dto)
    {
        var fornecedor = await _fornecedorRepository.ObterPorIdAsync(id)
            ?? throw new KeyNotFoundException("Fornecedor não encontrado.");

        if (await _fornecedorRepository.ExisteCnpjAsync(dto.Cnpj, id))
        {
            throw new InvalidOperationException("Já existe um fornecedor cadastrado com este CNPJ.");
        }

        fornecedor.RazaoSocial = dto.RazaoSocial;
        fornecedor.Cnpj = dto.Cnpj;
        fornecedor.Email = dto.Email;
        fornecedor.Telefone = dto.Telefone;

        await _fornecedorRepository.SalvarAlteracoesAsync();

        return MapearParaDto(fornecedor);
    }

    public async Task InativarAsync(int id)
    {
        var fornecedor = await _fornecedorRepository.ObterPorIdAsync(id)
            ?? throw new KeyNotFoundException("Fornecedor não encontrado.");

        fornecedor.Ativo = false;
        await _fornecedorRepository.SalvarAlteracoesAsync();
    }

    public async Task AtivarAsync(int id)
    {
        var fornecedor = await _fornecedorRepository.ObterPorIdAsync(id)
            ?? throw new KeyNotFoundException("Fornecedor não encontrado.");

        fornecedor.Ativo = true;
        await _fornecedorRepository.SalvarAlteracoesAsync();
    }

    public async Task AssociarProdutoAsync(int fornecedorId, int produtoId)
    {
        _ = await _fornecedorRepository.ObterPorIdAsync(fornecedorId)
            ?? throw new KeyNotFoundException("Fornecedor não encontrado.");

        _ = await _produtoRepository.ObterPorIdAsync(produtoId)
            ?? throw new KeyNotFoundException("Produto/serviço não encontrado.");

        if (await _fornecedorRepository.ExisteAssociacaoAsync(fornecedorId, produtoId))
        {
            throw new InvalidOperationException("Este produto/serviço já está associado a este fornecedor.");
        }

        await _fornecedorRepository.AdicionarAssociacaoAsync(fornecedorId, produtoId);
        await _fornecedorRepository.SalvarAlteracoesAsync();
    }

    public async Task DesassociarProdutoAsync(int fornecedorId, int produtoId)
    {
        var removido = await _fornecedorRepository.RemoverAssociacaoAsync(fornecedorId, produtoId);

        if (!removido)
        {
            throw new KeyNotFoundException("Associação entre fornecedor e produto/serviço não encontrada.");
        }

        await _fornecedorRepository.SalvarAlteracoesAsync();
    }

    private static FornecedorRespostaDto MapearParaDto(Fornecedor fornecedor)
    {
        return new FornecedorRespostaDto
        {
            Id = fornecedor.Id,
            RazaoSocial = fornecedor.RazaoSocial,
            Cnpj = fornecedor.Cnpj,
            Email = fornecedor.Email,
            Telefone = fornecedor.Telefone,
            Ativo = fornecedor.Ativo,
            ProdutosVinculados = fornecedor.Produtos
                .Select(fp => new ProdutoVinculadoDto
                {
                    ProdutoServicoId = fp.ProdutoServicoId,
                    Nome = fp.ProdutoServico.Nome
                })
                .ToList()
        };
    }
}
