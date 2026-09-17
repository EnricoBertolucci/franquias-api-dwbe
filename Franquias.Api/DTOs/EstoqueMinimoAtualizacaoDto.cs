using System.ComponentModel.DataAnnotations;

namespace Franquias.Api.DTOs;

public class EstoqueMinimoAtualizacaoDto
{
    [Range(0, int.MaxValue, ErrorMessage = "A quantidade mínima não pode ser negativa.")]
    public int QuantidadeMinima { get; set; }
}
