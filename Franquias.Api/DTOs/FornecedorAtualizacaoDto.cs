using System.ComponentModel.DataAnnotations;

namespace Franquias.Api.DTOs;

public class FornecedorAtualizacaoDto
{
    [Required(ErrorMessage = "A razão social é obrigatória.")]
    [MaxLength(150)]
    public string RazaoSocial { get; set; } = string.Empty;

    [Required(ErrorMessage = "O CNPJ é obrigatório.")]
    [MaxLength(18)]
    public string Cnpj { get; set; } = string.Empty;

    [MaxLength(150)]
    [EmailAddress(ErrorMessage = "E-mail inválido.")]
    public string Email { get; set; } = string.Empty;

    [MaxLength(20)]
    public string Telefone { get; set; } = string.Empty;
}
