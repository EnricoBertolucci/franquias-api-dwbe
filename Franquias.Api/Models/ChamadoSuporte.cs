using Franquias.Api.Models.Enums;

namespace Franquias.Api.Models;

public class ChamadoSuporte
{
    public int Id { get; set; }

    public int UnidadeFranqueadaId { get; set; }
    public UnidadeFranqueada UnidadeFranqueada { get; set; } = null!;

    public int UsuarioId { get; set; }
    public Usuario Usuario { get; set; } = null!;

    public string Categoria { get; set; } = string.Empty;
    public PrioridadeChamado Prioridade { get; set; } = PrioridadeChamado.Media;
    public string Descricao { get; set; } = string.Empty;
    public StatusChamado Status { get; set; } = StatusChamado.Aberto;

    public DateTime DataAbertura { get; set; } = DateTime.UtcNow;
    public DateTime? DataAtualizacao { get; set; }
    public DateTime? DataEncerramento { get; set; }
}
