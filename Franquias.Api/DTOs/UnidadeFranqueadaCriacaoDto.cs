using System.ComponentModel.DataAnnotations;

namespace Franquias.Api.DTOs;

public class UnidadeFranqueadaCriacaoDto
{
    [Required(ErrorMessage = "A franqueadora é obrigatória.")]
    public int FranqueadoraId { get; set; }

    [Required(ErrorMessage = "O nome da unidade é obrigatório.")]
    [MaxLength(150)]
    public string Nome { get; set; } = string.Empty;

    [Required(ErrorMessage = "O CNPJ é obrigatório.")]
    [MaxLength(18)]
    public string Cnpj { get; set; } = string.Empty;

    [MaxLength(20)]
    public string Telefone { get; set; } = string.Empty;

    [MaxLength(200)]
    public string Logradouro { get; set; } = string.Empty;

    [MaxLength(100)]
    public string Cidade { get; set; } = string.Empty;

    [MaxLength(2)]
    public string Estado { get; set; } = string.Empty;

    [MaxLength(9)]
    public string Cep { get; set; } = string.Empty;

    [Required(ErrorMessage = "A data de início é obrigatória.")]
    public DateTime DataInicio { get; set; }

    [Range(0, 100, ErrorMessage = "O percentual de royalty deve estar entre 0 e 100.")]
    public decimal PercentualRoyalty { get; set; }

    public List<FranqueadoDto> Responsaveis { get; set; } = new();
}
