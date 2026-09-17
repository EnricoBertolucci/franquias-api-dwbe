using Franquias.Api.Models.Enums;

namespace Franquias.Api.DTOs;

public class MovimentacaoEstoqueRespostaDto
{
    public int Id { get; set; }
    public int UnidadeFranqueadaId { get; set; }
    public int ProdutoServicoId { get; set; }
    public string ProdutoNome { get; set; } = string.Empty;
    public TipoMovimentacaoEstoque Tipo { get; set; }
    public int Quantidade { get; set; }
    public string Observacao { get; set; } = string.Empty;
    public DateTime DataMovimentacao { get; set; }
    public int UsuarioId { get; set; }
    public int SaldoAtual { get; set; }
}
