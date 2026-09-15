using Franquias.Api.DTOs;
using Franquias.Api.Models;
using Franquias.Api.Repositories;

namespace Franquias.Api.Services;

public class UsuarioService : IUsuarioService
{
    private readonly IUsuarioRepository _usuarioRepository;

    public UsuarioService(IUsuarioRepository usuarioRepository)
    {
        _usuarioRepository = usuarioRepository;
    }

    public async Task<UsuarioRespostaDto> CriarAsync(UsuarioCriacaoDto dto)
    {
        if (await _usuarioRepository.ExisteEmailAsync(dto.Email))
        {
            throw new InvalidOperationException("Já existe um usuário cadastrado com este e-mail.");
        }

        var usuario = new Usuario
        {
            Nome = dto.Nome,
            Email = dto.Email,
            SenhaHash = BCrypt.Net.BCrypt.HashPassword(dto.Senha),
            Perfil = dto.Perfil,
            UnidadeFranqueadaId = dto.UnidadeFranqueadaId,
            Ativo = true,
            DataCriacao = DateTime.UtcNow
        };

        await _usuarioRepository.AdicionarAsync(usuario);
        await _usuarioRepository.SalvarAlteracoesAsync();

        return MapearParaDto(usuario);
    }

    public async Task<List<UsuarioRespostaDto>> ListarAsync()
    {
        var usuarios = await _usuarioRepository.ListarAsync();
        return usuarios.Select(MapearParaDto).ToList();
    }

    public async Task<UsuarioRespostaDto> ObterPorIdAsync(int id)
    {
        var usuario = await _usuarioRepository.ObterPorIdAsync(id)
            ?? throw new KeyNotFoundException("Usuário não encontrado.");

        return MapearParaDto(usuario);
    }

    public async Task<UsuarioRespostaDto> AtualizarAsync(int id, UsuarioAtualizacaoDto dto)
    {
        var usuario = await _usuarioRepository.ObterPorIdAsync(id)
            ?? throw new KeyNotFoundException("Usuário não encontrado.");

        if (await _usuarioRepository.ExisteEmailAsync(dto.Email, id))
        {
            throw new InvalidOperationException("Já existe um usuário cadastrado com este e-mail.");
        }

        usuario.Nome = dto.Nome;
        usuario.Email = dto.Email;
        usuario.Perfil = dto.Perfil;
        usuario.UnidadeFranqueadaId = dto.UnidadeFranqueadaId;

        await _usuarioRepository.SalvarAlteracoesAsync();

        return MapearParaDto(usuario);
    }

    public async Task InativarAsync(int id)
    {
        var usuario = await _usuarioRepository.ObterPorIdAsync(id)
            ?? throw new KeyNotFoundException("Usuário não encontrado.");

        usuario.Ativo = false;
        await _usuarioRepository.SalvarAlteracoesAsync();
    }

    public async Task AtivarAsync(int id)
    {
        var usuario = await _usuarioRepository.ObterPorIdAsync(id)
            ?? throw new KeyNotFoundException("Usuário não encontrado.");

        usuario.Ativo = true;
        await _usuarioRepository.SalvarAlteracoesAsync();
    }

    private static UsuarioRespostaDto MapearParaDto(Usuario usuario)
    {
        return new UsuarioRespostaDto
        {
            Id = usuario.Id,
            Nome = usuario.Nome,
            Email = usuario.Email,
            Perfil = usuario.Perfil,
            Ativo = usuario.Ativo,
            DataCriacao = usuario.DataCriacao,
            UnidadeFranqueadaId = usuario.UnidadeFranqueadaId
        };
    }
}
