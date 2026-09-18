namespace Franquias.Api.DTOs;

public class ProdutoMaisVendidoDto
{
    public int ProdutoServicoId { get; set; }
    public string ProdutoServicoNome { get; set; } = string.Empty;
    public int QuantidadeVendida { get; set; }
    public decimal ValorTotalVendido { get; set; }
}
