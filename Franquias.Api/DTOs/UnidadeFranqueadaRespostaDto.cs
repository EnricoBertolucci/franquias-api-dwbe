using Franquias.Api.Models.Enums;

namespace Franquias.Api.DTOs;

public class UnidadeFranqueadaRespostaDto
{
    public int Id { get; set; }
    public int FranqueadoraId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Cnpj { get; set; } = string.Empty;
    public string Telefone { get; set; } = string.Empty;
    public string Logradouro { get; set; } = string.Empty;
    public string Cidade { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public string Cep { get; set; } = string.Empty;
    public DateTime DataInicio { get; set; }
    public SituacaoUnidade Situacao { get; set; }
    public decimal PercentualRoyalty { get; set; }
    public List<FranqueadoDto> Responsaveis { get; set; } = new();
}
