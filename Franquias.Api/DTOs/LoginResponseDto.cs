namespace Franquias.Api.DTOs;

public class LoginResponseDto
{
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiraEm { get; set; }
    public UsuarioRespostaDto Usuario { get; set; } = null!;
}
