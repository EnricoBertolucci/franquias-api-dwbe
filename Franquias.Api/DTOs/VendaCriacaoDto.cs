using System.ComponentModel.DataAnnotations;

namespace Franquias.Api.DTOs;

public class VendaCriacaoDto
{
    [Required(ErrorMessage = "A unidade franqueada é obrigatória.")]
    public int UnidadeFranqueadaId { get; set; }

    [Required(ErrorMessage = "A venda deve possuir ao menos um item.")]
    [MinLength(1, ErrorMessage = "A venda deve possuir ao menos um item.")]
    public List<ItemVendaCriacaoDto> Itens { get; set; } = new();
}
