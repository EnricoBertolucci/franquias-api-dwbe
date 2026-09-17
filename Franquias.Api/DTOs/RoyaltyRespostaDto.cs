using Franquias.Api.Models.Enums;

namespace Franquias.Api.DTOs;

public class RoyaltyRespostaDto
{
    public int Id { get; set; }
    public int UnidadeFranqueadaId { get; set; }
    public string UnidadeFranqueadaNome { get; set; } = string.Empty;
    public int AnoReferencia { get; set; }
    public int MesReferencia { get; set; }
    public decimal FaturamentoBase { get; set; }
    public decimal PercentualAplicado { get; set; }
    public decimal ValorCalculado { get; set; }
    public StatusPagamento StatusPagamento { get; set; }
    public DateTime DataVencimento { get; set; }
    public DateTime? DataPagamento { get; set; }
}
