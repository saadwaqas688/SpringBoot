using DotNetMessaging.API.DTOs;
using DotNetMessaging.API.Models;
using DotNetMessaging.API.Repositories;

namespace DotNetMessaging.API.Services;

public interface IUserService
{
    Task<UserDto?> GetUserByIdAsync(string userId);
    Task<UserDto?> UpdateUserStatusAsync(string userId, string? status);
    Task<UserDto?> UpdateUserProfileAsync(string userId, string? username, string? profilePictureUrl);
    Task<bool> UpdateOnlineStatusAsync(string userId, bool isOnline);
}

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UserDto?> GetUserByIdAsync(string userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        return user != null ? MapToDto(user) : null;
    }

    public async Task<UserDto?> UpdateUserStatusAsync(string userId, string? status)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null) return null;

        user.Status = status;
        await _userRepository.UpdateAsync(userId, user);
        return MapToDto(user);
    }

    public async Task<UserDto?> UpdateUserProfileAsync(string userId, string? username, string? profilePictureUrl)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null) return null;

        if (!string.IsNullOrEmpty(username) && username != user.Username)
        {
            var existingUser = await _userRepository.GetByUsernameAsync(username);
            if (existingUser != null)
                throw new InvalidOperationException("Username already exists");
            
            user.Username = username;
        }

        if (profilePictureUrl != null)
        {
            user.ProfilePictureUrl = profilePictureUrl;
        }

        await _userRepository.UpdateAsync(userId, user);
        return MapToDto(user);
    }

    public async Task<bool> UpdateOnlineStatusAsync(string userId, bool isOnline)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null) return false;

        user.IsOnline = isOnline;
        user.LastSeen = DateTime.UtcNow;
        await _userRepository.UpdateAsync(userId, user);
        return true;
    }

    private UserDto MapToDto(User user)
    {
        return new UserDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            ProfilePictureUrl = user.ProfilePictureUrl,
            Status = user.Status,
            IsOnline = user.IsOnline,
            LastSeen = user.LastSeen
        };
    }
}

