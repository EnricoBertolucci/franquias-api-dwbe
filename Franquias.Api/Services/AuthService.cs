using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Franquias.Api.DTOs;
using Franquias.Api.Models;
using Franquias.Api.Repositories;
using Microsoft.IdentityModel.Tokens;

namespace Franquias.Api.Services;

public class AuthService : IAuthService
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IConfiguration _configuration;

    public AuthService(IUsuarioRepository usuarioRepository, IConfiguration configuration)
    {
        _usuarioRepository = usuarioRepository;
        _configuration = configuration;
    }

    public async Task<LoginResponseDto> LoginAsync(LoginRequestDto dto)
    {
        var usuario = await _usuarioRepository.ObterPorEmailAsync(dto.Email);

        if (usuario is null || !usuario.Ativo || !BCrypt.Net.BCrypt.Verify(dto.Senha, usuario.SenhaHash))
        {
            throw new UnauthorizedAccessException("E-mail ou senha inválidos.");
        }

        var (token, expiraEm) = GerarToken(usuario);

        return new LoginResponseDto
        {
            Token = token,
            ExpiraEm = expiraEm,
            Usuario = new UsuarioRespostaDto
            {
                Id = usuario.Id,
                Nome = usuario.Nome,
                Email = usuario.Email,
                Perfil = usuario.Perfil,
                Ativo = usuario.Ativo,
                DataCriacao = usuario.DataCriacao,
                UnidadeFranqueadaId = usuario.UnidadeFranqueadaId
            }
        };
    }

    private (string Token, DateTime ExpiraEm) GerarToken(Usuario usuario)
    {
        var jwtSettings = _configuration.GetSection("Jwt");
        var chave = jwtSettings["Key"]!;
        var expirationMinutes = int.Parse(jwtSettings["ExpirationMinutes"]!);
        var expiraEm = DateTime.UtcNow.AddMinutes(expirationMinutes);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, usuario.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, usuario.Email),
            new(ClaimTypes.Name, usuario.Nome),
            new(ClaimTypes.Role, usuario.Perfil.ToString())
        };

        if (usuario.UnidadeFranqueadaId.HasValue)
        {
            claims.Add(new Claim("unidadeFranqueadaId", usuario.UnidadeFranqueadaId.Value.ToString()));
        }

        var credenciais = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(chave)),
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: expiraEm,
            signingCredentials: credenciais);

        return (new JwtSecurityTokenHandler().WriteToken(token), expiraEm);
    }
}
