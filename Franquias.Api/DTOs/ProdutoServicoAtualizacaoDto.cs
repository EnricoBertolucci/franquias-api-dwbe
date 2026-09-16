using System.ComponentModel.DataAnnotations;
using Franquias.Api.Models.Enums;

namespace Franquias.Api.DTOs;

public class ProdutoServicoAtualizacaoDto
{
    [Required(ErrorMessage = "A categoria é obrigatória.")]
    public int CategoriaId { get; set; }

    [Required(ErrorMessage = "O nome do produto/serviço é obrigatório.")]
    [MaxLength(150)]
    public string Nome { get; set; } = string.Empty;

    [MaxLength(500)]
    public string Descricao { get; set; } = string.Empty;

    [Required(ErrorMessage = "O tipo é obrigatório.")]
    public TipoProdutoServico Tipo { get; set; } = TipoProdutoServico.Produto;

    [Range(0.01, double.MaxValue, ErrorMessage = "O preço base deve ser maior que zero.")]
    public decimal PrecoBase { get; set; }
}
