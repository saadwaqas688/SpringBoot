using UserAccountService.Application.DTOs;
using UserAccountService.Models;
using Shared.Core.Common;

namespace UserAccountService.Application.Services;

public interface IUserAccountService
{
    Task<UserAccount?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<UserAccount?> GetUserByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<UserAccount> CreateUserAsync(UserAccount user, CancellationToken cancellationToken = default);
    Task<UserAccount?> UpdateUserAsync(string id, UserAccount existingUser, UpdateProfileDto dto, CancellationToken cancellationToken = default);
    Task<UserAccount?> UpdateUserAdminAsync(string id, UpdateUserDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteUserAsync(string id, CancellationToken cancellationToken = default);
    Task<UserAccount?> UpdateUserStatusAsync(string id, string status, CancellationToken cancellationToken = default);
    Task<PagedResponse<UserAccount>> GetAllUsersAsync(int page, int pageSize, string? searchTerm = null, CancellationToken cancellationToken = default);
}

