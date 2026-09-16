using System.ComponentModel.DataAnnotations;

namespace Franquias.Api.DTOs;

public class FranqueadoDto
{
    public int Id { get; set; }

    [Required(ErrorMessage = "O nome do responsável é obrigatório.")]
    [MaxLength(150)]
    public string Nome { get; set; } = string.Empty;

    [Required(ErrorMessage = "O CPF do responsável é obrigatório.")]
    [MaxLength(14)]
    public string Cpf { get; set; } = string.Empty;

    [EmailAddress(ErrorMessage = "E-mail inválido.")]
    [MaxLength(150)]
    public string Email { get; set; } = string.Empty;

    [MaxLength(20)]
    public string Telefone { get; set; } = string.Empty;

    [MaxLength(80)]
    public string Cargo { get; set; } = string.Empty;

    public bool ResponsavelPrincipal { get; set; }
}
