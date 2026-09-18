namespace Franquias.Api.DTOs;

public class RoyaltyPorUnidadeDto
{
    public int UnidadeFranqueadaId { get; set; }
    public string UnidadeFranqueadaNome { get; set; } = string.Empty;
    public int QuantidadeRoyalties { get; set; }
    public decimal ValorTotal { get; set; }
}
