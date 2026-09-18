using Franquias.Api.DTOs;
using Franquias.Api.Models;
using Franquias.Api.Models.Enums;
using Franquias.Api.Repositories;
using Franquias.Api.Services;
using Moq;
using Xunit;

namespace Franquias.Api.Tests.Services;

public class UsuarioServiceTests
{
    [Fact]
    public async Task CriarAsync_ComEmailJaCadastrado_DeveLancarInvalidOperationException()
    {
        var repositorio = new Mock<IUsuarioRepository>();
        repositorio.Setup(r => r.ExisteEmailAsync(It.IsAny<string>(), null)).ReturnsAsync(true);

        var servico = new UsuarioService(repositorio.Object);

        var dto = new UsuarioCriacaoDto
        {
            Nome = "Usuário Teste",
            Email = "duplicado@teste.com",
            Senha = "senha123",
            Perfil = PerfilUsuario.Operador
        };

        await Assert.ThrowsAsync<InvalidOperationException>(() => servico.CriarAsync(dto));

        repositorio.Verify(r => r.AdicionarAsync(It.IsAny<Usuario>()), Times.Never);
    }

    [Fact]
    public async Task CriarAsync_ComEmailNovo_DeveCadastrarUsuario()
    {
        var repositorio = new Mock<IUsuarioRepository>();
        repositorio.Setup(r => r.ExisteEmailAsync(It.IsAny<string>(), null)).ReturnsAsync(false);

        var servico = new UsuarioService(repositorio.Object);

        var dto = new UsuarioCriacaoDto
        {
            Nome = "Usuário Novo",
            Email = "novo@teste.com",
            Senha = "senha123",
            Perfil = PerfilUsuario.Operador
        };

        var resultado = await servico.CriarAsync(dto);

        Assert.Equal(dto.Email, resultado.Email);
        repositorio.Verify(r => r.AdicionarAsync(It.IsAny<Usuario>()), Times.Once);
        repositorio.Verify(r => r.SalvarAlteracoesAsync(), Times.Once);
    }
}
