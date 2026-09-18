using Franquias.Api.DTOs;
using Franquias.Api.Models;
using Franquias.Api.Models.Enums;
using Franquias.Api.Repositories;
using Franquias.Api.Services;
using Moq;
using Xunit;

namespace Franquias.Api.Tests.Services;

public class RoyaltyServiceTests
{
    [Fact]
    public async Task CalcularAsync_UnidadeSemRoyaltyAnterior_DeveCalcularValorComBaseNoPercentual()
    {
        var royaltyRepositorio = new Mock<IRoyaltyRepository>();
        var unidadeRepositorio = new Mock<IUnidadeFranqueadaRepository>();

        unidadeRepositorio.Setup(r => r.ObterPorIdAsync(1))
            .ReturnsAsync(new UnidadeFranqueada { Id = 1, PercentualRoyalty = 10m });

        royaltyRepositorio.Setup(r => r.ObterFaturamentoUnidadeAsync(1, It.IsAny<DateTime>(), It.IsAny<DateTime>()))
            .ReturnsAsync(1000m);
        royaltyRepositorio.Setup(r => r.ObterPorUnidadeEPeriodoAsync(1, 2026, 9))
            .ReturnsAsync((Royalty?)null);

        var servico = new RoyaltyService(royaltyRepositorio.Object, unidadeRepositorio.Object);

        var dto = new RoyaltyCalculoRequestDto { UnidadeFranqueadaId = 1, AnoReferencia = 2026, MesReferencia = 9 };

        var resultado = await servico.CalcularAsync(dto);

        Assert.Equal(1000m, resultado.FaturamentoBase);
        Assert.Equal(10m, resultado.PercentualAplicado);
        Assert.Equal(100m, resultado.ValorCalculado);
        royaltyRepositorio.Verify(r => r.AdicionarAsync(It.IsAny<Royalty>()), Times.Once);
    }

    [Fact]
    public async Task CalcularAsync_RoyaltyJaPago_DeveLancarInvalidOperationException()
    {
        var royaltyRepositorio = new Mock<IRoyaltyRepository>();
        var unidadeRepositorio = new Mock<IUnidadeFranqueadaRepository>();

        var unidade = new UnidadeFranqueada { Id = 1, PercentualRoyalty = 10m };
        unidadeRepositorio.Setup(r => r.ObterPorIdAsync(1)).ReturnsAsync(unidade);

        royaltyRepositorio.Setup(r => r.ObterFaturamentoUnidadeAsync(1, It.IsAny<DateTime>(), It.IsAny<DateTime>()))
            .ReturnsAsync(1000m);
        royaltyRepositorio.Setup(r => r.ObterPorUnidadeEPeriodoAsync(1, 2026, 9))
            .ReturnsAsync(new Royalty
            {
                UnidadeFranqueadaId = 1,
                UnidadeFranqueada = unidade,
                AnoReferencia = 2026,
                MesReferencia = 9,
                StatusPagamento = StatusPagamento.Pago
            });

        var servico = new RoyaltyService(royaltyRepositorio.Object, unidadeRepositorio.Object);

        var dto = new RoyaltyCalculoRequestDto { UnidadeFranqueadaId = 1, AnoReferencia = 2026, MesReferencia = 9 };

        await Assert.ThrowsAsync<InvalidOperationException>(() => servico.CalcularAsync(dto));
    }
}
