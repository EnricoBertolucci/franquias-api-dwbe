using Franquias.Api.Models.Enums;

namespace Franquias.Api.DTOs;

public class VendaRespostaDto
{
    public int Id { get; set; }
    public int UnidadeFranqueadaId { get; set; }
    public int UsuarioId { get; set; }
    public DateTime DataVenda { get; set; }
    public decimal ValorTotal { get; set; }
    public StatusVenda Status { get; set; }
    public List<ItemVendaRespostaDto> Itens { get; set; } = new();
}
