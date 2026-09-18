using System.ComponentModel.DataAnnotations;
using Franquias.Api.Models.Enums;

namespace Franquias.Api.DTOs;

public class ChamadoCriacaoDto
{
    [Required(ErrorMessage = "A unidade franqueada é obrigatória.")]
    public int UnidadeFranqueadaId { get; set; }

    [Required(ErrorMessage = "A categoria é obrigatória.")]
    [MaxLength(80)]
    public string Categoria { get; set; } = string.Empty;

    [Required(ErrorMessage = "A prioridade é obrigatória.")]
    public PrioridadeChamado Prioridade { get; set; }

    [Required(ErrorMessage = "A descrição é obrigatória.")]
    [MaxLength(1000)]
    public string Descricao { get; set; } = string.Empty;
}
