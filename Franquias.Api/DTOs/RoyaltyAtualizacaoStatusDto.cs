using System.ComponentModel.DataAnnotations;
using Franquias.Api.Models.Enums;

namespace Franquias.Api.DTOs;

public class RoyaltyAtualizacaoStatusDto
{
    [Required(ErrorMessage = "O status de pagamento é obrigatório.")]
    public StatusPagamento StatusPagamento { get; set; }

    public DateTime? DataPagamento { get; set; }
}
