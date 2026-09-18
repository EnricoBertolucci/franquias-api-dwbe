namespace Franquias.Api.DTOs;

public class RankingUnidadeDto
{
    public int Posicao { get; set; }
    public int UnidadeFranqueadaId { get; set; }
    public string UnidadeFranqueadaNome { get; set; } = string.Empty;
    public int QuantidadeVendas { get; set; }
    public decimal ValorTotal { get; set; }
}
