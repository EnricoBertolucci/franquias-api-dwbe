using System.ComponentModel.DataAnnotations;

namespace Franquias.Api.DTOs;

public class ItemVendaCriacaoDto
{
    [Required(ErrorMessage = "O produto/serviço é obrigatório.")]
    public int ProdutoServicoId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "A quantidade deve ser maior que zero.")]
    public int Quantidade { get; set; }
}
