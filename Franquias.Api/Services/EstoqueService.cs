using Franquias.Api.DTOs;
using Franquias.Api.Models;
using Franquias.Api.Models.Enums;
using Franquias.Api.Repositories;

namespace Franquias.Api.Services;

public class EstoqueService : IEstoqueService
{
    private readonly IEstoqueRepository _estoqueRepository;
    private readonly IProdutoServicoRepository _produtoRepository;
    private readonly IUnidadeFranqueadaRepository _unidadeRepository;

    public EstoqueService(
        IEstoqueRepository estoqueRepository,
        IProdutoServicoRepository produtoRepository,
        IUnidadeFranqueadaRepository unidadeRepository)
    {
        _estoqueRepository = estoqueRepository;
        _produtoRepository = produtoRepository;
        _unidadeRepository = unidadeRepository;
    }

    public async Task<List<EstoqueRespostaDto>> ListarPorUnidadeAsync(int unidadeId)
    {
        var estoques = await _estoqueRepository.ListarPorUnidadeAsync(unidadeId);
        return estoques.Select(MapearParaDto).ToList();
    }

    public async Task<EstoqueRespostaDto> ObterPorUnidadeEProdutoAsync(int unidadeId, int produtoId)
    {
        var estoque = await _estoqueRepository.ObterPorUnidadeEProdutoAsync(unidadeId, produtoId)
            ?? throw new KeyNotFoundException("Não há registro de estoque para este produto nesta unidade.");

        return MapearParaDto(estoque);
    }

    public async Task<List<EstoqueRespostaDto>> ListarCriticosPorUnidadeAsync(int unidadeId)
    {
        var estoques = await _estoqueRepository.ListarCriticosPorUnidadeAsync(unidadeId);
        return estoques.Select(MapearParaDto).ToList();
    }

    public async Task<MovimentacaoEstoqueRespostaDto> RegistrarMovimentacaoAsync(MovimentacaoEstoqueCriacaoDto dto, int usuarioId)
    {
        _ = await _unidadeRepository.ObterPorIdAsync(dto.UnidadeFranqueadaId)
            ?? throw new KeyNotFoundException("Unidade franqueada não encontrada.");

        var produto = await _produtoRepository.ObterPorIdAsync(dto.ProdutoServicoId)
            ?? throw new KeyNotFoundException("Produto/serviço não encontrado.");

        var estoque = await _estoqueRepository.ObterPorUnidadeEProdutoAsync(dto.UnidadeFranqueadaId, dto.ProdutoServicoId);

        if (estoque is null)
        {
            if (dto.Tipo == TipoMovimentacaoEstoque.Saida)
            {
                throw new InvalidOperationException("Saldo insuficiente para a movimentação de saída.");
            }

            estoque = new Estoque
            {
                UnidadeFranqueadaId = dto.UnidadeFranqueadaId,
                ProdutoServicoId = dto.ProdutoServicoId,
                ProdutoServico = produto,
                QuantidadeAtual = 0,
                QuantidadeMinima = 0
            };
        }

        if (dto.Tipo == TipoMovimentacaoEstoque.Saida && estoque.QuantidadeAtual < dto.Quantidade)
        {
            throw new InvalidOperationException("Saldo insuficiente para a movimentação de saída.");
        }

        estoque.QuantidadeAtual += dto.Tipo == TipoMovimentacaoEstoque.Entrada ? dto.Quantidade : -dto.Quantidade;

        var movimentacao = new MovimentacaoEstoque
        {
            Estoque = estoque,
            Tipo = dto.Tipo,
            Quantidade = dto.Quantidade,
            Observacao = dto.Observacao,
            DataMovimentacao = DateTime.UtcNow,
            UsuarioId = usuarioId
        };

        await _estoqueRepository.RegistrarMovimentacaoAsync(estoque, movimentacao);

        return MapearMovimentacaoParaDto(movimentacao, produto);
    }

    public async Task<EstoqueRespostaDto> AtualizarEstoqueMinimoAsync(int unidadeId, int produtoId, int quantidadeMinima)
    {
        var estoque = await _estoqueRepository.ObterPorUnidadeEProdutoAsync(unidadeId, produtoId);

        if (estoque is null)
        {
            _ = await _unidadeRepository.ObterPorIdAsync(unidadeId)
                ?? throw new KeyNotFoundException("Unidade franqueada não encontrada.");

            var produto = await _produtoRepository.ObterPorIdAsync(produtoId)
                ?? throw new KeyNotFoundException("Produto/serviço não encontrado.");

            estoque = new Estoque
            {
                UnidadeFranqueadaId = unidadeId,
                ProdutoServicoId = produtoId,
                ProdutoServico = produto,
                QuantidadeAtual = 0,
                QuantidadeMinima = quantidadeMinima
            };

            await _estoqueRepository.AdicionarAsync(estoque);
        }
        else
        {
            estoque.QuantidadeMinima = quantidadeMinima;
        }

        await _estoqueRepository.SalvarAlteracoesAsync();

        return MapearParaDto(estoque);
    }

    private static EstoqueRespostaDto MapearParaDto(Estoque estoque)
    {
        return new EstoqueRespostaDto
        {
            Id = estoque.Id,
            UnidadeFranqueadaId = estoque.UnidadeFranqueadaId,
            ProdutoServicoId = estoque.ProdutoServicoId,
            ProdutoNome = estoque.ProdutoServico.Nome,
            QuantidadeAtual = estoque.QuantidadeAtual,
            QuantidadeMinima = estoque.QuantidadeMinima,
            Critico = estoque.QuantidadeAtual < estoque.QuantidadeMinima
        };
    }

    private static MovimentacaoEstoqueRespostaDto MapearMovimentacaoParaDto(MovimentacaoEstoque movimentacao, ProdutoServico produto)
    {
        return new MovimentacaoEstoqueRespostaDto
        {
            Id = movimentacao.Id,
            UnidadeFranqueadaId = movimentacao.Estoque.UnidadeFranqueadaId,
            ProdutoServicoId = movimentacao.Estoque.ProdutoServicoId,
            ProdutoNome = produto.Nome,
            Tipo = movimentacao.Tipo,
            Quantidade = movimentacao.Quantidade,
            Observacao = movimentacao.Observacao,
            DataMovimentacao = movimentacao.DataMovimentacao,
            UsuarioId = movimentacao.UsuarioId,
            SaldoAtual = movimentacao.Estoque.QuantidadeAtual
        };
    }
}
