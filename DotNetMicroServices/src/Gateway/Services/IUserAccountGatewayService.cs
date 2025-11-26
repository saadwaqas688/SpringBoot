using Gateway.DTOs;
using Shared.Common;

namespace Gateway.Services;

public interface IUserAccountGatewayService
{
    Task<ApiResponse<AuthResponseDto>> SignUpAsync(SignUpDto dto);
    Task<ApiResponse<AuthResponseDto>> SignInAsync(SignInDto dto);
    Task<ApiResponse<UserInfoDto>> UpdateProfileAsync(string token, UpdateProfileDto dto);
    Task<ApiResponse<UserInfoDto>> GetCurrentUserAsync(string token);
}

