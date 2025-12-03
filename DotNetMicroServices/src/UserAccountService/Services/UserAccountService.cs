using MongoDB.Driver;
using MongoDB.Bson;
using UserAccountService.Data;
using UserAccountService.DTOs;
using UserAccountService.Models;
using Shared.Utils;
using Shared.Common;
using Microsoft.Extensions.Logging;

namespace UserAccountService.Services;

public class UserAccountService : IUserAccountService
{
    private readonly MongoDbContext _context;
    private readonly ILogger<UserAccountService>? _logger;

    public UserAccountService(MongoDbContext context, ILogger<UserAccountService>? logger = null)
    {
        _context = context;
        _logger = logger;
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
        user.Role = user.Role ?? "user"; // Default role is "user"
        user.Status = user.Status ?? "active"; // Default status is "active"
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
        {
            updateDefinition = updateDefinition.Set(u => u.Name, dto.Name);
        }
        
        // Update Image if provided in DTO (not null - can be empty string to clear it)
        if (dto.Image != null)
        {
            updateDefinition = updateDefinition.Set(u => u.Image, dto.Image);
        }

        // Update optional profile fields if provided (not null and not empty)
        if (!string.IsNullOrEmpty(dto.Gender))
        {
            updateDefinition = updateDefinition.Set(u => u.Gender, dto.Gender);
        }
        
        if (dto.DateOfBirth.HasValue)
        {
            updateDefinition = updateDefinition.Set(u => u.DateOfBirth, dto.DateOfBirth.Value);
        }
        
        if (!string.IsNullOrEmpty(dto.MobilePhone))
        {
            updateDefinition = updateDefinition.Set(u => u.MobilePhone, dto.MobilePhone);
        }
        
        if (!string.IsNullOrEmpty(dto.Country))
        {
            updateDefinition = updateDefinition.Set(u => u.Country, dto.Country);
        }
        
        if (!string.IsNullOrEmpty(dto.State))
        {
            updateDefinition = updateDefinition.Set(u => u.State, dto.State);
        }
        
        if (!string.IsNullOrEmpty(dto.City))
        {
            updateDefinition = updateDefinition.Set(u => u.City, dto.City);
        }
        
        if (!string.IsNullOrEmpty(dto.PostalCode))
        {
            updateDefinition = updateDefinition.Set(u => u.PostalCode, dto.PostalCode);
        }

        var updateResult = await _context.UserAccounts.UpdateOneAsync(filter, updateDefinition);
        
        // Log update result
        _logger?.LogInformation("UpdateResult - MatchedCount: {MatchedCount}, ModifiedCount: {ModifiedCount}", 
            updateResult.MatchedCount, updateResult.ModifiedCount);
        
        // Return updated user
        var updatedUser = await _context.UserAccounts.Find(filter).FirstOrDefaultAsync();
        _logger?.LogInformation("Updated User - Gender: {Gender}, DateOfBirth: {DateOfBirth}, MobilePhone: {MobilePhone}, City: {City}, PostalCode: {PostalCode}",
            updatedUser?.Gender, updatedUser?.DateOfBirth, updatedUser?.MobilePhone, updatedUser?.City, updatedUser?.PostalCode);
        
        return updatedUser;
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

    public async Task<UserAccount?> UpdateUserAdminAsync(string id, UpdateUserDto dto)
    {
        var filter = Builders<UserAccount>.Filter.Eq(u => u.Id, id);
        var user = await _context.UserAccounts.Find(filter).FirstOrDefaultAsync();
        
        if (user == null)
            return null;

        var updateDefinition = Builders<UserAccount>.Update
            .Set(u => u.UpdatedAt, DateTimeHelper.GetUtcNow());

        // Update Name (combine FirstName and LastName if provided)
        if (!string.IsNullOrEmpty(dto.FirstName) || !string.IsNullOrEmpty(dto.LastName))
        {
            var firstName = dto.FirstName ?? "";
            var lastName = dto.LastName ?? "";
            var fullName = $"{firstName} {lastName}".Trim();
            if (!string.IsNullOrEmpty(fullName))
            {
                updateDefinition = updateDefinition.Set(u => u.Name, fullName);
            }
        }

        // Update Email if provided
        if (!string.IsNullOrEmpty(dto.Email))
        {
            updateDefinition = updateDefinition.Set(u => u.Email, dto.Email.ToLower());
        }

        // Update Image if provided
        if (dto.Image != null)
        {
            updateDefinition = updateDefinition.Set(u => u.Image, dto.Image);
        }

        // Update Role if provided
        if (!string.IsNullOrEmpty(dto.Role))
        {
            updateDefinition = updateDefinition.Set(u => u.Role, dto.Role);
        }

        // Update Status if provided
        if (!string.IsNullOrEmpty(dto.Status))
        {
            updateDefinition = updateDefinition.Set(u => u.Status, dto.Status);
        }

        // Update optional profile fields
        if (!string.IsNullOrEmpty(dto.Gender))
        {
            updateDefinition = updateDefinition.Set(u => u.Gender, dto.Gender);
        }
        
        if (dto.DateOfBirth.HasValue)
        {
            updateDefinition = updateDefinition.Set(u => u.DateOfBirth, dto.DateOfBirth.Value);
        }
        
        if (!string.IsNullOrEmpty(dto.MobilePhone))
        {
            updateDefinition = updateDefinition.Set(u => u.MobilePhone, dto.MobilePhone);
        }
        
        if (!string.IsNullOrEmpty(dto.Country))
        {
            updateDefinition = updateDefinition.Set(u => u.Country, dto.Country);
        }
        
        if (!string.IsNullOrEmpty(dto.State))
        {
            updateDefinition = updateDefinition.Set(u => u.State, dto.State);
        }
        
        if (!string.IsNullOrEmpty(dto.City))
        {
            updateDefinition = updateDefinition.Set(u => u.City, dto.City);
        }
        
        if (!string.IsNullOrEmpty(dto.PostalCode))
        {
            updateDefinition = updateDefinition.Set(u => u.PostalCode, dto.PostalCode);
        }

        var updateResult = await _context.UserAccounts.UpdateOneAsync(filter, updateDefinition);
        
        if (updateResult.MatchedCount == 0)
            return null;

        return await _context.UserAccounts.Find(filter).FirstOrDefaultAsync();
    }

    public async Task<bool> DeleteUserAsync(string id)
    {
        try
        {
            var filter = Builders<UserAccount>.Filter.Eq(u => u.Id, id);
            var result = await _context.UserAccounts.DeleteOneAsync(filter);
            return result.DeletedCount > 0;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error deleting user with id: {Id}", id);
            return false;
        }
    }

    public async Task<bool> UpdateUserStatusAsync(string id, string status)
    {
        try
        {
            var filter = Builders<UserAccount>.Filter.Eq(u => u.Id, id);
            var update = Builders<UserAccount>.Update
                .Set(u => u.Status, status)
                .Set(u => u.UpdatedAt, DateTimeHelper.GetUtcNow());
            
            var result = await _context.UserAccounts.UpdateOneAsync(filter, update);
            return result.MatchedCount > 0;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error updating user status for id: {Id}", id);
            return false;
        }
    }
}

