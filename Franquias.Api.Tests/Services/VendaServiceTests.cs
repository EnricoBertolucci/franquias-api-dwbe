using Franquias.Api.DTOs;
using Franquias.Api.Models;
using Franquias.Api.Models.Enums;
using Franquias.Api.Repositories;
using Franquias.Api.Services;
using Moq;
using Xunit;

namespace Franquias.Api.Tests.Services;

public class VendaServiceTests
{
    [Fact]
    public async Task CriarAsync_SemItens_DeveLancarArgumentException()
    {
        var servico = CriarServico(
            out _, out _, out _, out _);

        var dto = new VendaCriacaoDto
        {
            UnidadeFranqueadaId = 1,
            Itens = new List<ItemVendaCriacaoDto>()
        };

        await Assert.ThrowsAsync<ArgumentException>(() => servico.CriarAsync(dto, usuarioId: 1));
    }

    [Fact]
    public async Task CriarAsync_UnidadeInativa_DeveLancarInvalidOperationException()
    {
        var servico = CriarServico(
            out var unidadeRepositorio, out _, out _, out _);

        unidadeRepositorio.Setup(r => r.ObterPorIdAsync(1))
            .ReturnsAsync(new UnidadeFranqueada { Id = 1, Situacao = SituacaoUnidade.Inativa });

        var dto = new VendaCriacaoDto
        {
            UnidadeFranqueadaId = 1,
            Itens = new List<ItemVendaCriacaoDto> { new() { ProdutoServicoId = 1, Quantidade = 1 } }
        };

        await Assert.ThrowsAsync<InvalidOperationException>(() => servico.CriarAsync(dto, usuarioId: 1));
    }

    [Fact]
    public async Task CriarAsync_EstoqueInsuficiente_DeveLancarInvalidOperationException()
    {
        var servico = CriarServico(
            out var unidadeRepositorio, out var produtoRepositorio, out var estoqueRepositorio, out _);

        unidadeRepositorio.Setup(r => r.ObterPorIdAsync(1))
            .ReturnsAsync(new UnidadeFranqueada { Id = 1, Situacao = SituacaoUnidade.Ativa });
        produtoRepositorio.Setup(r => r.ObterPorIdAsync(1))
            .ReturnsAsync(new ProdutoServico { Id = 1, Nome = "Produto Teste", PrecoBase = 10m });
        estoqueRepositorio.Setup(r => r.ObterPorUnidadeEProdutoAsync(1, 1))
            .ReturnsAsync(new Estoque { UnidadeFranqueadaId = 1, ProdutoServicoId = 1, QuantidadeAtual = 2 });

        var dto = new VendaCriacaoDto
        {
            UnidadeFranqueadaId = 1,
            Itens = new List<ItemVendaCriacaoDto> { new() { ProdutoServicoId = 1, Quantidade = 5 } }
        };

        await Assert.ThrowsAsync<InvalidOperationException>(() => servico.CriarAsync(dto, usuarioId: 1));
    }

    [Fact]
    public async Task CriarAsync_ComItensValidos_DeveCalcularTotalEBaixarEstoque()
    {
        var servico = CriarServico(
            out var unidadeRepositorio, out var produtoRepositorio, out var estoqueRepositorio, out var vendaRepositorio);

        unidadeRepositorio.Setup(r => r.ObterPorIdAsync(1))
            .ReturnsAsync(new UnidadeFranqueada { Id = 1, Situacao = SituacaoUnidade.Ativa });

        produtoRepositorio.Setup(r => r.ObterPorIdAsync(1))
            .ReturnsAsync(new ProdutoServico { Id = 1, Nome = "Produto A", PrecoBase = 10m });
        produtoRepositorio.Setup(r => r.ObterPorIdAsync(2))
            .ReturnsAsync(new ProdutoServico { Id = 2, Nome = "Produto B", PrecoBase = 25m });

        var estoqueA = new Estoque { UnidadeFranqueadaId = 1, ProdutoServicoId = 1, QuantidadeAtual = 10 };
        var estoqueB = new Estoque { UnidadeFranqueadaId = 1, ProdutoServicoId = 2, QuantidadeAtual = 10 };
        estoqueRepositorio.Setup(r => r.ObterPorUnidadeEProdutoAsync(1, 1)).ReturnsAsync(estoqueA);
        estoqueRepositorio.Setup(r => r.ObterPorUnidadeEProdutoAsync(1, 2)).ReturnsAsync(estoqueB);

        var dto = new VendaCriacaoDto
        {
            UnidadeFranqueadaId = 1,
            Itens = new List<ItemVendaCriacaoDto>
            {
                new() { ProdutoServicoId = 1, Quantidade = 3 }, // 3 * 10 = 30
                new() { ProdutoServicoId = 2, Quantidade = 2 }  // 2 * 25 = 50
            }
        };

        var resultado = await servico.CriarAsync(dto, usuarioId: 1);

        Assert.Equal(80m, resultado.ValorTotal);
        Assert.Equal(7, estoqueA.QuantidadeAtual);
        Assert.Equal(8, estoqueB.QuantidadeAtual);
        vendaRepositorio.Verify(
            r => r.RegistrarVendaAsync(It.IsAny<Venda>(), It.Is<List<MovimentacaoEstoque>>(l => l.Count == 2)),
            Times.Once);
    }

    private static VendaService CriarServico(
        out Mock<IUnidadeFranqueadaRepository> unidadeRepositorio,
        out Mock<IProdutoServicoRepository> produtoRepositorio,
        out Mock<IEstoqueRepository> estoqueRepositorio,
        out Mock<IVendaRepository> vendaRepositorio)
    {
        vendaRepositorio = new Mock<IVendaRepository>();
        unidadeRepositorio = new Mock<IUnidadeFranqueadaRepository>();
        produtoRepositorio = new Mock<IProdutoServicoRepository>();
        estoqueRepositorio = new Mock<IEstoqueRepository>();

        return new VendaService(
            vendaRepositorio.Object,
            unidadeRepositorio.Object,
            produtoRepositorio.Object,
            estoqueRepositorio.Object);
    }
}
