using Franquias.Api.Models.Enums;

namespace Franquias.Api.Models;

public class MovimentacaoEstoque
{
    public int Id { get; set; }

    public int EstoqueId { get; set; }
    public Estoque Estoque { get; set; } = null!;

    public TipoMovimentacaoEstoque Tipo { get; set; }
    public int Quantidade { get; set; }
    public string Observacao { get; set; } = string.Empty;
    public DateTime DataMovimentacao { get; set; } = DateTime.UtcNow;

    public int? VendaId { get; set; }
    public Venda? Venda { get; set; }
}
