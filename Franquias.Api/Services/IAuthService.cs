using Franquias.Api.DTOs;

namespace Franquias.Api.Services;

public interface IAuthService
{
    Task<LoginResponseDto> LoginAsync(LoginRequestDto dto);
}
