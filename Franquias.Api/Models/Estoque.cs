namespace Franquias.Api.Models;

public class Estoque
{
    public int Id { get; set; }

    public int UnidadeFranqueadaId { get; set; }
    public UnidadeFranqueada UnidadeFranqueada { get; set; } = null!;

    public int ProdutoServicoId { get; set; }
    public ProdutoServico ProdutoServico { get; set; } = null!;

    public int QuantidadeAtual { get; set; }
    public int QuantidadeMinima { get; set; }

    public ICollection<MovimentacaoEstoque> Movimentacoes { get; set; } = new List<MovimentacaoEstoque>();
}
