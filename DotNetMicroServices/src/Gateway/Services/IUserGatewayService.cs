using Shared.Models;
using Shared.DTOs;
using Shared.Common;

namespace Gateway.Services;

public interface IUserGatewayService
{
    Task<ApiResponse<List<User>>> GetAllUsersAsync();
    Task<ApiResponse<User>> GetUserByIdAsync(Guid id);
    Task<ApiResponse<User>> GetUserByEmailAsync(string email);
    Task<ApiResponse<User>> CreateUserAsync(CreateUserDto dto);
    Task<ApiResponse<User>> UpdateUserAsync(Guid id, UpdateUserDto dto);
    Task<ApiResponse<bool>> DeleteUserAsync(Guid id);
}

