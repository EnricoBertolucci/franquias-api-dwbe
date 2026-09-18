namespace Franquias.Api.DTOs;

public class RoyaltiesRelatorioDto
{
    public DateTime PeriodoInicio { get; set; }
    public DateTime PeriodoFim { get; set; }
    public decimal ValorTotal { get; set; }
    public List<RoyaltyPorUnidadeDto> Itens { get; set; } = new();
}
