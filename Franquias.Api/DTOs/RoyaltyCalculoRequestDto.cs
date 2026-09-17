using System.ComponentModel.DataAnnotations;

namespace Franquias.Api.DTOs;

public class RoyaltyCalculoRequestDto
{
    [Required(ErrorMessage = "A unidade franqueada é obrigatória.")]
    public int UnidadeFranqueadaId { get; set; }

    [Range(2000, 2100, ErrorMessage = "O ano de referência é inválido.")]
    public int AnoReferencia { get; set; }

    [Range(1, 12, ErrorMessage = "O mês de referência deve estar entre 1 e 12.")]
    public int MesReferencia { get; set; }
}
