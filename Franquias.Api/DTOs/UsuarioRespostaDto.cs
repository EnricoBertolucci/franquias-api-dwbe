using Franquias.Api.Models.Enums;

namespace Franquias.Api.DTOs;

public class UsuarioRespostaDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public PerfilUsuario Perfil { get; set; }
    public bool Ativo { get; set; }
    public DateTime DataCriacao { get; set; }
    public int? UnidadeFranqueadaId { get; set; }
}
