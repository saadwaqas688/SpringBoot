using Gateway.DTOs;
using Shared.Common;

namespace Gateway.Services;

public interface IUserAccountGatewayService
{
    Task<ApiResponse<AuthResponseDto>> SignUpAsync(SignUpDto dto);
    Task<ApiResponse<AuthResponseDto>> SignInAsync(SignInDto dto);
    Task<ApiResponse<UserInfoDto>> UpdateProfileAsync(string token, UpdateProfileDto dto);
    Task<ApiResponse<UserInfoDto>> GetCurrentUserAsync(string token);
    Task<ApiResponse<PagedResponse<UserInfoDto>>> GetAllUsersAsync(int page, int pageSize, string? searchTerm = null);
    Task<ApiResponse<UserInfoDto>> CreateUserAsync(CreateUserDto dto);
    Task<ApiResponse<UserInfoDto>> UpdateUserAsync(string id, UpdateUserDto dto);
    Task<ApiResponse<bool>> DeleteUserAsync(string id);
    Task<ApiResponse<bool>> UpdateUserStatusAsync(string id, string status);
    Task<ApiResponse<AuthResponseDto>> RefreshTokenAsync(RefreshTokenDto dto);
    Task<ApiResponse<string>> ForgotPasswordAsync(ForgotPasswordDto dto);
    Task<ApiResponse<string>> ResetPasswordAsync(ResetPasswordDto dto);
}

