using Franquias.Api.Models.Enums;

namespace Franquias.Api.Models;

public class UnidadeFranqueada
{
    public int Id { get; set; }
    public int FranqueadoraId { get; set; }
    public Franqueadora Franqueadora { get; set; } = null!;

    public string Nome { get; set; } = string.Empty;
    public string Cnpj { get; set; } = string.Empty;
    public string Telefone { get; set; } = string.Empty;
    public string Logradouro { get; set; } = string.Empty;
    public string Cidade { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public string Cep { get; set; } = string.Empty;

    public DateTime DataInicio { get; set; }
    public SituacaoUnidade Situacao { get; set; } = SituacaoUnidade.Ativa;
    public decimal PercentualRoyalty { get; set; }

    public ICollection<Franqueado> Responsaveis { get; set; } = new List<Franqueado>();
    public ICollection<Estoque> Estoques { get; set; } = new List<Estoque>();
    public ICollection<Venda> Vendas { get; set; } = new List<Venda>();
    public ICollection<Royalty> Royalties { get; set; } = new List<Royalty>();
    public ICollection<ChamadoSuporte> Chamados { get; set; } = new List<ChamadoSuporte>();
}
