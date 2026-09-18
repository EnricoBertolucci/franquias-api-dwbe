using System.ComponentModel.DataAnnotations;
using Franquias.Api.Models.Enums;

namespace Franquias.Api.DTOs;

public class ChamadoAtualizacaoDto
{
    public StatusChamado? Status { get; set; }
    public PrioridadeChamado? Prioridade { get; set; }

    [MaxLength(1000)]
    public string? Descricao { get; set; }
}
