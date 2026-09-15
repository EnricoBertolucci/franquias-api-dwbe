using Franquias.Api.Models.Enums;

namespace Franquias.Api.Models;

public class Usuario
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string SenhaHash { get; set; } = string.Empty;
    public PerfilUsuario Perfil { get; set; }
    public bool Ativo { get; set; } = true;
    public DateTime DataCriacao { get; set; } = DateTime.UtcNow;

    public int? UnidadeFranqueadaId { get; set; }
    public UnidadeFranqueada? UnidadeFranqueada { get; set; }
}
