namespace Franquias.Api.DTOs;

public class FornecedorRespostaDto
{
    public int Id { get; set; }
    public string RazaoSocial { get; set; } = string.Empty;
    public string Cnpj { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Telefone { get; set; } = string.Empty;
    public bool Ativo { get; set; }
    public List<ProdutoVinculadoDto> ProdutosVinculados { get; set; } = new();
}

public class ProdutoVinculadoDto
{
    public int ProdutoServicoId { get; set; }
    public string Nome { get; set; } = string.Empty;
}
