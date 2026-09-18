using Franquias.Api.Models.Enums;

namespace Franquias.Api.DTOs;

public class ChamadosPorStatusDto
{
    public StatusChamado Status { get; set; }
    public int Quantidade { get; set; }
}
