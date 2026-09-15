namespace Franquias.Api.Models;

public class Franqueadora
{
    public int Id { get; set; }
    public string RazaoSocial { get; set; } = string.Empty;
    public string NomeFantasia { get; set; } = string.Empty;
    public string Cnpj { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Telefone { get; set; } = string.Empty;
    public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
    public bool Ativo { get; set; } = true;

    public ICollection<UnidadeFranqueada> Unidades { get; set; } = new List<UnidadeFranqueada>();
}
