using UserAccountService.DTOs;
using UserAccountService.Models;

namespace UserAccountService.Services;

public interface IUserAccountService
{
    Task<UserAccount?> GetUserByEmailAsync(string email);
    Task<UserAccount?> GetUserByIdAsync(Guid id);
    Task<UserAccount> CreateUserAsync(UserAccount user);
    Task<UserAccount?> UpdateUserAsync(Guid id, UserAccount existingUser, UpdateProfileDto dto);
}

