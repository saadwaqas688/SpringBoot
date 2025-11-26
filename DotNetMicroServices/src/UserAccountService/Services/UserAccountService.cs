using MongoDB.Driver;
using UserAccountService.Data;
using UserAccountService.DTOs;
using UserAccountService.Models;
using Shared.Utils;

namespace UserAccountService.Services;

public class UserAccountService : IUserAccountService
{
    private readonly MongoDbContext _context;

    public UserAccountService(MongoDbContext context)
    {
        _context = context;
    }

    public async Task<UserAccount?> GetUserByEmailAsync(string email)
    {
        var filter = Builders<UserAccount>.Filter.Eq(u => u.Email, email.ToLower());
        return await _context.UserAccounts.Find(filter).FirstOrDefaultAsync();
    }

    public async Task<UserAccount?> GetUserByIdAsync(Guid id)
    {
        var filter = Builders<UserAccount>.Filter.Eq(u => u.Id, id);
        return await _context.UserAccounts.Find(filter).FirstOrDefaultAsync();
    }

    public async Task<UserAccount> CreateUserAsync(UserAccount user)
    {
        user.Id = Guid.NewGuid();
        user.CreatedAt = DateTimeHelper.GetUtcNow();
        user.UpdatedAt = DateTimeHelper.GetUtcNow();
        user.Role = "user"; // Default role
        user.Email = user.Email.ToLower(); // Store email in lowercase

        await _context.UserAccounts.InsertOneAsync(user);
        return user;
    }

    public async Task<UserAccount?> UpdateUserAsync(Guid id, UserAccount existingUser, UpdateProfileDto dto)
    {
        var filter = Builders<UserAccount>.Filter.Eq(u => u.Id, id);
        var user = await _context.UserAccounts.Find(filter).FirstOrDefaultAsync();
        
        if (user == null)
            return null;

        var updateDefinition = Builders<UserAccount>.Update
            .Set(u => u.UpdatedAt, DateTimeHelper.GetUtcNow());

        // Update Name if provided in DTO (not null)
        if (dto.Name != null)
            updateDefinition = updateDefinition.Set(u => u.Name, dto.Name);
        
        // Update Image if provided in DTO (not null - can be empty string to clear it)
        if (dto.Image != null)
            updateDefinition = updateDefinition.Set(u => u.Image, dto.Image);

        await _context.UserAccounts.UpdateOneAsync(filter, updateDefinition);
        
        // Return updated user
        return await _context.UserAccounts.Find(filter).FirstOrDefaultAsync();
    }
}

