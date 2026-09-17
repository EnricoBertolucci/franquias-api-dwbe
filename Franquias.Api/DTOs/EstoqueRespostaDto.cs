namespace Franquias.Api.DTOs;

public class EstoqueRespostaDto
{
    public int Id { get; set; }
    public int UnidadeFranqueadaId { get; set; }
    public int ProdutoServicoId { get; set; }
    public string ProdutoNome { get; set; } = string.Empty;
    public int QuantidadeAtual { get; set; }
    public int QuantidadeMinima { get; set; }
    public bool Critico { get; set; }
}
