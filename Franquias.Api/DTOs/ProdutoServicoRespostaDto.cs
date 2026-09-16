using Franquias.Api.Models.Enums;

namespace Franquias.Api.DTOs;

public class ProdutoServicoRespostaDto
{
    public int Id { get; set; }
    public int CategoriaId { get; set; }
    public string CategoriaNome { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public TipoProdutoServico Tipo { get; set; }
    public decimal PrecoBase { get; set; }
    public bool Ativo { get; set; }
}
