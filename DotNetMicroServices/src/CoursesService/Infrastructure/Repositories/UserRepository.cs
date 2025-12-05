using CoursesService.Models;
using MongoDB.Driver;
using Shared.Core.Common;
using Shared.Infrastructure.Repositories;

namespace CoursesService.Repositories;

public class UserRepository : BaseRepository<User>, IUserRepository
{
    public UserRepository(IMongoCollection<User> collection, ILogger<UserRepository> logger)
        : base(collection, logger)
    {
    }

    public async Task<PagedResponse<User>> GetAllNonAdminUsersAsync(int page, int pageSize, string? searchTerm = null)
    {
        // Filter out admin users
        var filter = Builders<User>.Filter.Ne(u => u.Role, "admin");

        // Add search filter if search term is provided
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var searchFilter = Builders<User>.Filter.Or(
                Builders<User>.Filter.Regex(u => u.Name, new MongoDB.Bson.BsonRegularExpression(searchTerm, "i")),
                Builders<User>.Filter.Regex(u => u.Email, new MongoDB.Bson.BsonRegularExpression(searchTerm, "i"))
            );
            filter = Builders<User>.Filter.And(filter, searchFilter);
        }

        // Get total count
        var totalCount = await _collection.CountDocumentsAsync(filter);

        // Calculate pagination
        var skip = (page - 1) * pageSize;
        var items = await _collection
            .Find(filter)
            .SortByDescending(u => u.CreatedAt)
            .Skip(skip)
            .Limit(pageSize)
            .ToListAsync();

        return new PagedResponse<User>
        {
            Items = items,
            PageNumber = page,
            PageSize = pageSize,
            TotalCount = (int)totalCount
        };
    }
}


