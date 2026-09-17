using System.ComponentModel.DataAnnotations;
using Franquias.Api.Models.Enums;

namespace Franquias.Api.DTOs;

public class MovimentacaoEstoqueCriacaoDto
{
    [Required(ErrorMessage = "A unidade franqueada é obrigatória.")]
    public int UnidadeFranqueadaId { get; set; }

    [Required(ErrorMessage = "O produto/serviço é obrigatório.")]
    public int ProdutoServicoId { get; set; }

    [Required(ErrorMessage = "O tipo de movimentação é obrigatório.")]
    public TipoMovimentacaoEstoque Tipo { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "A quantidade deve ser maior que zero.")]
    public int Quantidade { get; set; }

    [MaxLength(300)]
    public string Observacao { get; set; } = string.Empty;
}
