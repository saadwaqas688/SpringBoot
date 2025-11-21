using LMS.API.DTOs;

namespace LMS.API.Services;

public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsync(RegisterDto dto);
    Task<AuthResponseDto> LoginAsync(LoginDto dto);
    string GenerateJwtToken(UserDto user);
}

