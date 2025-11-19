using DotNetMessaging.API.Models;

namespace DotNetMessaging.API.Repositories;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByUsernameAsync(string username);
    Task<IEnumerable<User>> SearchUsersAsync(string searchTerm, string excludeUserId);
}

