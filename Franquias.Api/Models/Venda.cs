using Franquias.Api.Models.Enums;

namespace Franquias.Api.Models;

public class Venda
{
    public int Id { get; set; }

    public int UnidadeFranqueadaId { get; set; }
    public UnidadeFranqueada UnidadeFranqueada { get; set; } = null!;

    public int UsuarioId { get; set; }
    public Usuario Usuario { get; set; } = null!;

    public DateTime DataVenda { get; set; } = DateTime.UtcNow;
    public decimal ValorTotal { get; set; }
    public StatusVenda Status { get; set; } = StatusVenda.Confirmada;

    public ICollection<ItemVenda> Itens { get; set; } = new List<ItemVenda>();
}
