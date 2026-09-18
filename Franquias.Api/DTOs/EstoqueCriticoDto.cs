namespace Franquias.Api.DTOs;

public class EstoqueCriticoDto
{
    public int UnidadeFranqueadaId { get; set; }
    public string UnidadeFranqueadaNome { get; set; } = string.Empty;
    public int ProdutoServicoId { get; set; }
    public string ProdutoServicoNome { get; set; } = string.Empty;
    public int QuantidadeAtual { get; set; }
    public int QuantidadeMinima { get; set; }
}
