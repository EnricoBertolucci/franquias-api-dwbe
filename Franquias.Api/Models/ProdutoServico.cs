using Franquias.Api.Models.Enums;

namespace Franquias.Api.Models;

public class ProdutoServico
{
    public int Id { get; set; }
    public int CategoriaId { get; set; }
    public Categoria Categoria { get; set; } = null!;

    public string Nome { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public TipoProdutoServico Tipo { get; set; } = TipoProdutoServico.Produto;
    public decimal PrecoBase { get; set; }
    public bool Ativo { get; set; } = true;

    public ICollection<Estoque> Estoques { get; set; } = new List<Estoque>();
    public ICollection<ItemVenda> ItensVenda { get; set; } = new List<ItemVenda>();
    public ICollection<FornecedorProduto> Fornecedores { get; set; } = new List<FornecedorProduto>();
}
