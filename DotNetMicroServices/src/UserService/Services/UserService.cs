using UserService.Models;
using Shared.Utils;

namespace UserService.Services;

public class UserService : IUserService
{
    private static readonly List<User> _users = new();
    private static readonly object _lock = new();

    public Task<List<User>> GetAllUsersAsync()
    {
        lock (_lock)
        {
            return Task.FromResult(new List<User>(_users));
        }
    }

    public Task<User?> GetUserByIdAsync(Guid id)
    {
        lock (_lock)
        {
            var user = _users.FirstOrDefault(u => u.Id == id);
            return Task.FromResult(user);
        }
    }

    public Task<User?> GetUserByEmailAsync(string email)
    {
        lock (_lock)
        {
            var user = _users.FirstOrDefault(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
            return Task.FromResult(user);
        }
    }

    public Task<User> CreateUserAsync(User user)
    {
        lock (_lock)
        {
            user.Id = Guid.NewGuid();
            user.CreatedAt = DateTimeHelper.GetUtcNow();
            user.UpdatedAt = DateTimeHelper.GetUtcNow();
            _users.Add(user);
            return Task.FromResult(user);
        }
    }

    public Task<User?> UpdateUserAsync(Guid id, User updatedUser)
    {
        lock (_lock)
        {
            var user = _users.FirstOrDefault(u => u.Id == id);
            if (user == null)
                return Task.FromResult<User?>(null);

            user.Username = updatedUser.Username;
            user.Email = updatedUser.Email;
            user.FirstName = updatedUser.FirstName;
            user.LastName = updatedUser.LastName;
            user.UpdatedAt = DateTimeHelper.GetUtcNow();

            return Task.FromResult<User?>(user);
        }
    }

    public Task<bool> DeleteUserAsync(Guid id)
    {
        lock (_lock)
        {
            var user = _users.FirstOrDefault(u => u.Id == id);
            if (user == null)
                return Task.FromResult(false);

            _users.Remove(user);
            return Task.FromResult(true);
        }
    }
}

