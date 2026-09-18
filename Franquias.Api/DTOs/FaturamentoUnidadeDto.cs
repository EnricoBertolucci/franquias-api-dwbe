namespace Franquias.Api.DTOs;

public class FaturamentoUnidadeDto
{
    public int UnidadeFranqueadaId { get; set; }
    public string UnidadeFranqueadaNome { get; set; } = string.Empty;
    public DateTime PeriodoInicio { get; set; }
    public DateTime PeriodoFim { get; set; }
    public int QuantidadeVendas { get; set; }
    public decimal ValorTotal { get; set; }
}
