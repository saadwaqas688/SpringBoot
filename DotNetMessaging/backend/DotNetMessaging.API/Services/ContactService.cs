using DotNetMessaging.API.DTOs;
using DotNetMessaging.API.Models;
using DotNetMessaging.API.Repositories;

namespace DotNetMessaging.API.Services;

public interface IContactService
{
    Task<List<UserDto>> GetUserContactsAsync(string userId);
    Task<bool> AddContactAsync(string userId, string contactUserId, string? displayName = null);
    Task<bool> RemoveContactAsync(string userId, string contactUserId);
    Task<List<UserDto>> SearchUsersAsync(string userId, string searchTerm);
}

public class ContactService : IContactService
{
    private readonly IContactRepository _contactRepository;
    private readonly IUserRepository _userRepository;

    public ContactService(IContactRepository contactRepository, IUserRepository userRepository)
    {
        _contactRepository = contactRepository;
        _userRepository = userRepository;
    }

    public async Task<List<UserDto>> GetUserContactsAsync(string userId)
    {
        var contacts = await _contactRepository.GetUserContactsAsync(userId);
        var userDtos = new List<UserDto>();

        foreach (var contact in contacts)
        {
            var user = await _userRepository.GetByIdAsync(contact.ContactUserId);
            if (user != null)
            {
                userDtos.Add(MapUserToDto(user));
            }
        }

        return userDtos;
    }

    public async Task<bool> AddContactAsync(string userId, string contactUserId, string? displayName = null)
    {
        if (userId == contactUserId)
            return false;

        var existing = await _contactRepository.GetContactAsync(userId, contactUserId);
        if (existing != null)
            return false;

        var contact = new Contact
        {
            UserId = userId,
            ContactUserId = contactUserId,
            DisplayName = displayName,
            CreatedAt = DateTime.UtcNow
        };

        await _contactRepository.CreateAsync(contact);
        return true;
    }

    public async Task<bool> RemoveContactAsync(string userId, string contactUserId)
    {
        var contact = await _contactRepository.GetContactAsync(userId, contactUserId);
        if (contact != null)
        {
            await _contactRepository.DeleteAsync(contact.Id);
            return true;
        }

        return false;
    }

    public async Task<List<UserDto>> SearchUsersAsync(string userId, string searchTerm)
    {
        var users = await _userRepository.SearchUsersAsync(searchTerm, userId);
        return users.Select(MapUserToDto).ToList();
    }

    private UserDto MapUserToDto(User user)
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

