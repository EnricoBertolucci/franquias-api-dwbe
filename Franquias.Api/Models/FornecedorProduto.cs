namespace Franquias.Api.Models;

public class FornecedorProduto
{
    public int FornecedorId { get; set; }
    public Fornecedor Fornecedor { get; set; } = null!;

    public int ProdutoServicoId { get; set; }
    public ProdutoServico ProdutoServico { get; set; } = null!;
}
