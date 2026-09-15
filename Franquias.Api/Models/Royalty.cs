using Franquias.Api.Models.Enums;

namespace Franquias.Api.Models;

public class Royalty
{
    public int Id { get; set; }

    public int UnidadeFranqueadaId { get; set; }
    public UnidadeFranqueada UnidadeFranqueada { get; set; } = null!;

    public int AnoReferencia { get; set; }
    public int MesReferencia { get; set; }

    public decimal FaturamentoBase { get; set; }
    public decimal PercentualAplicado { get; set; }
    public decimal ValorCalculado { get; set; }

    public StatusPagamento StatusPagamento { get; set; } = StatusPagamento.Pendente;
    public DateTime DataVencimento { get; set; }
    public DateTime? DataPagamento { get; set; }
}
