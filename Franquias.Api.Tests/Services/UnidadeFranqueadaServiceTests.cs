using Franquias.Api.DTOs;
using Franquias.Api.Models;
using Franquias.Api.Repositories;
using Franquias.Api.Services;
using Moq;
using Xunit;

namespace Franquias.Api.Tests.Services;

public class UnidadeFranqueadaServiceTests
{
    [Fact]
    public async Task CriarAsync_ComCnpjJaCadastrado_DeveLancarInvalidOperationException()
    {
        var unidadeRepositorio = new Mock<IUnidadeFranqueadaRepository>();
        var franqueadoraRepositorio = new Mock<IFranqueadoraRepository>();

        franqueadoraRepositorio.Setup(r => r.ObterPorIdAsync(1)).ReturnsAsync(new Franqueadora { Id = 1 });
        unidadeRepositorio.Setup(r => r.ExisteCnpjAsync(It.IsAny<string>(), null)).ReturnsAsync(true);

        var servico = new UnidadeFranqueadaService(unidadeRepositorio.Object, franqueadoraRepositorio.Object);

        var dto = new UnidadeFranqueadaCriacaoDto
        {
            FranqueadoraId = 1,
            Nome = "Unidade Duplicada",
            Cnpj = "00.000.000/0001-00",
            DataInicio = DateTime.UtcNow
        };

        await Assert.ThrowsAsync<InvalidOperationException>(() => servico.CriarAsync(dto));

        unidadeRepositorio.Verify(r => r.AdicionarAsync(It.IsAny<UnidadeFranqueada>()), Times.Never);
    }

    [Fact]
    public async Task CriarAsync_ComCnpjNovo_DeveCadastrarUnidade()
    {
        var unidadeRepositorio = new Mock<IUnidadeFranqueadaRepository>();
        var franqueadoraRepositorio = new Mock<IFranqueadoraRepository>();

        franqueadoraRepositorio.Setup(r => r.ObterPorIdAsync(1)).ReturnsAsync(new Franqueadora { Id = 1 });
        unidadeRepositorio.Setup(r => r.ExisteCnpjAsync(It.IsAny<string>(), null)).ReturnsAsync(false);

        var servico = new UnidadeFranqueadaService(unidadeRepositorio.Object, franqueadoraRepositorio.Object);

        var dto = new UnidadeFranqueadaCriacaoDto
        {
            FranqueadoraId = 1,
            Nome = "Unidade Nova",
            Cnpj = "11.111.111/0001-11",
            DataInicio = DateTime.UtcNow
        };

        var resultado = await servico.CriarAsync(dto);

        Assert.Equal(dto.Cnpj, resultado.Cnpj);
        unidadeRepositorio.Verify(r => r.AdicionarAsync(It.IsAny<UnidadeFranqueada>()), Times.Once);
    }
}
