using System.ComponentModel.DataAnnotations;

namespace Franquias.Api.DTOs;

public class FranqueadoraDto
{
    public int Id { get; set; }

    [Required(ErrorMessage = "A razão social é obrigatória.")]
    [MaxLength(150)]
    public string RazaoSocial { get; set; } = string.Empty;

    [Required(ErrorMessage = "O nome fantasia é obrigatório.")]
    [MaxLength(150)]
    public string NomeFantasia { get; set; } = string.Empty;

    [Required(ErrorMessage = "O CNPJ é obrigatório.")]
    [MaxLength(18)]
    public string Cnpj { get; set; } = string.Empty;

    [Required(ErrorMessage = "O e-mail é obrigatório.")]
    [EmailAddress(ErrorMessage = "E-mail inválido.")]
    [MaxLength(150)]
    public string Email { get; set; } = string.Empty;

    [MaxLength(20)]
    public string Telefone { get; set; } = string.Empty;

    public bool Ativo { get; set; }

    public DateTime DataCriacao { get; set; }
}
