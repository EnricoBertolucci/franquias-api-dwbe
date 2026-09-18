using Franquias.Api.Models.Enums;

namespace Franquias.Api.DTOs;

public class ChamadoRespostaDto
{
    public int Id { get; set; }
    public int UnidadeFranqueadaId { get; set; }
    public string UnidadeFranqueadaNome { get; set; } = string.Empty;
    public int UsuarioId { get; set; }
    public string UsuarioNome { get; set; } = string.Empty;
    public string Categoria { get; set; } = string.Empty;
    public PrioridadeChamado Prioridade { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public StatusChamado Status { get; set; }
    public DateTime DataAbertura { get; set; }
    public DateTime? DataAtualizacao { get; set; }
    public DateTime? DataEncerramento { get; set; }
}
