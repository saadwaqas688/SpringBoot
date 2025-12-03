using UserAccountService.DTOs;
using UserAccountService.Models;
using Shared.Common;

namespace UserAccountService.Services;

public interface IUserAccountService
{
    Task<UserAccount?> GetUserByEmailAsync(string email);
    Task<UserAccount?> GetUserByIdAsync(string id);
    Task<UserAccount> CreateUserAsync(UserAccount user);
    Task<UserAccount?> UpdateUserAsync(string id, UserAccount existingUser, UpdateProfileDto dto);
    Task<UserAccount?> UpdateUserAdminAsync(string id, UpdateUserDto dto);
    Task<bool> DeleteUserAsync(string id);
    Task<bool> UpdateUserStatusAsync(string id, string status);
    Task<PagedResponse<UserAccount>> GetAllUsersAsync(int page, int pageSize, string? searchTerm = null);
}

