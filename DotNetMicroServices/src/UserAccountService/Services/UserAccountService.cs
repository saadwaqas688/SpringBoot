using MongoDB.Driver;
using MongoDB.Bson;
using UserAccountService.Data;
using UserAccountService.DTOs;
using UserAccountService.Models;
using Shared.Utils;
using Shared.Common;

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

    public async Task<UserAccount?> GetUserByIdAsync(string id)
    {
        var filter = Builders<UserAccount>.Filter.Eq(u => u.Id, id);
        return await _context.UserAccounts.Find(filter).FirstOrDefaultAsync();
    }

    public async Task<UserAccount> CreateUserAsync(UserAccount user)
    {
        // MongoDB will auto-generate ObjectId if Id is null
        user.CreatedAt = DateTimeHelper.GetUtcNow();
        user.UpdatedAt = DateTimeHelper.GetUtcNow();
        user.Role = "user"; // Default role
        user.Email = user.Email.ToLower(); // Store email in lowercase

        await _context.UserAccounts.InsertOneAsync(user);
        return user;
    }

    public async Task<UserAccount?> UpdateUserAsync(string id, UserAccount existingUser, UpdateProfileDto dto)
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

    public async Task<PagedResponse<UserAccount>> GetAllUsersAsync(int page, int pageSize, string? searchTerm = null)
    {
        FilterDefinition<UserAccount> filter;
        
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            // Search by name or email (case-insensitive)
            var nameFilter = Builders<UserAccount>.Filter.Regex(
                u => u.Name,
                new BsonRegularExpression(searchTerm, "i")
            );
            var emailFilter = Builders<UserAccount>.Filter.Regex(
                u => u.Email,
                new BsonRegularExpression(searchTerm, "i")
            );
            filter = Builders<UserAccount>.Filter.Or(nameFilter, emailFilter);
        }
        else
        {
            filter = Builders<UserAccount>.Filter.Empty;
        }

        var sort = Builders<UserAccount>.Sort.Descending(u => u.CreatedAt);

        var totalCount = await _context.UserAccounts.CountDocumentsAsync(filter);
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        var skip = (page - 1) * pageSize;
        var items = await _context.UserAccounts
            .Find(filter)
            .Sort(sort)
            .Skip(skip)
            .Limit(pageSize)
            .ToListAsync();

        return new PagedResponse<UserAccount>
        {
            Items = items,
            PageNumber = page,
            PageSize = pageSize,
            TotalCount = (int)totalCount
        };
    }
}

