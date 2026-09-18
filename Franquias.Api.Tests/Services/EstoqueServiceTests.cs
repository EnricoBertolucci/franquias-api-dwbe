using Franquias.Api.DTOs;
using Franquias.Api.Models;
using Franquias.Api.Models.Enums;
using Franquias.Api.Repositories;
using Franquias.Api.Services;
using Moq;
using Xunit;

namespace Franquias.Api.Tests.Services;

public class EstoqueServiceTests
{
    [Fact]
    public async Task RegistrarMovimentacaoAsync_SaidaMaiorQueSaldo_DeveLancarInvalidOperationException()
    {
        var estoqueRepositorio = new Mock<IEstoqueRepository>();
        var produtoRepositorio = new Mock<IProdutoServicoRepository>();
        var unidadeRepositorio = new Mock<IUnidadeFranqueadaRepository>();

        unidadeRepositorio.Setup(r => r.ObterPorIdAsync(1)).ReturnsAsync(new UnidadeFranqueada { Id = 1 });
        produtoRepositorio.Setup(r => r.ObterPorIdAsync(1)).ReturnsAsync(new ProdutoServico { Id = 1, Nome = "Produto Teste" });
        estoqueRepositorio.Setup(r => r.ObterPorUnidadeEProdutoAsync(1, 1))
            .ReturnsAsync(new Estoque { UnidadeFranqueadaId = 1, ProdutoServicoId = 1, QuantidadeAtual = 5, QuantidadeMinima = 0 });

        var servico = new EstoqueService(estoqueRepositorio.Object, produtoRepositorio.Object, unidadeRepositorio.Object);

        var dto = new MovimentacaoEstoqueCriacaoDto
        {
            UnidadeFranqueadaId = 1,
            ProdutoServicoId = 1,
            Tipo = TipoMovimentacaoEstoque.Saida,
            Quantidade = 10
        };

        await Assert.ThrowsAsync<InvalidOperationException>(() => servico.RegistrarMovimentacaoAsync(dto, usuarioId: 1));

        estoqueRepositorio.Verify(
            r => r.RegistrarMovimentacaoAsync(It.IsAny<Estoque>(), It.IsAny<MovimentacaoEstoque>()),
            Times.Never);
    }

    [Fact]
    public async Task RegistrarMovimentacaoAsync_SaidaSemRegistroDeEstoque_DeveLancarInvalidOperationException()
    {
        var estoqueRepositorio = new Mock<IEstoqueRepository>();
        var produtoRepositorio = new Mock<IProdutoServicoRepository>();
        var unidadeRepositorio = new Mock<IUnidadeFranqueadaRepository>();

        unidadeRepositorio.Setup(r => r.ObterPorIdAsync(1)).ReturnsAsync(new UnidadeFranqueada { Id = 1 });
        produtoRepositorio.Setup(r => r.ObterPorIdAsync(1)).ReturnsAsync(new ProdutoServico { Id = 1, Nome = "Produto Teste" });
        estoqueRepositorio.Setup(r => r.ObterPorUnidadeEProdutoAsync(1, 1)).ReturnsAsync((Estoque?)null);

        var servico = new EstoqueService(estoqueRepositorio.Object, produtoRepositorio.Object, unidadeRepositorio.Object);

        var dto = new MovimentacaoEstoqueCriacaoDto
        {
            UnidadeFranqueadaId = 1,
            ProdutoServicoId = 1,
            Tipo = TipoMovimentacaoEstoque.Saida,
            Quantidade = 1
        };

        await Assert.ThrowsAsync<InvalidOperationException>(() => servico.RegistrarMovimentacaoAsync(dto, usuarioId: 1));
    }

    [Fact]
    public async Task RegistrarMovimentacaoAsync_EntradaValida_DeveIncrementarSaldo()
    {
        var estoqueRepositorio = new Mock<IEstoqueRepository>();
        var produtoRepositorio = new Mock<IProdutoServicoRepository>();
        var unidadeRepositorio = new Mock<IUnidadeFranqueadaRepository>();

        unidadeRepositorio.Setup(r => r.ObterPorIdAsync(1)).ReturnsAsync(new UnidadeFranqueada { Id = 1 });
        produtoRepositorio.Setup(r => r.ObterPorIdAsync(1)).ReturnsAsync(new ProdutoServico { Id = 1, Nome = "Produto Teste" });

        var estoque = new Estoque { UnidadeFranqueadaId = 1, ProdutoServicoId = 1, QuantidadeAtual = 5, QuantidadeMinima = 0 };
        estoqueRepositorio.Setup(r => r.ObterPorUnidadeEProdutoAsync(1, 1)).ReturnsAsync(estoque);

        var servico = new EstoqueService(estoqueRepositorio.Object, produtoRepositorio.Object, unidadeRepositorio.Object);

        var dto = new MovimentacaoEstoqueCriacaoDto
        {
            UnidadeFranqueadaId = 1,
            ProdutoServicoId = 1,
            Tipo = TipoMovimentacaoEstoque.Entrada,
            Quantidade = 10
        };

        var resultado = await servico.RegistrarMovimentacaoAsync(dto, usuarioId: 1);

        Assert.Equal(15, resultado.SaldoAtual);
        Assert.Equal(15, estoque.QuantidadeAtual);
    }
}
