using CoursesService.Models;
using MongoDB.Driver;
using Shared.Repositories;

namespace CoursesService.Repositories;

public class UserActivityLogRepository : BaseRepository<UserActivityLog>, IUserActivityLogRepository
{
    public UserActivityLogRepository(IMongoCollection<UserActivityLog> collection, ILogger<UserActivityLogRepository> logger)
        : base(collection, logger)
    {
    }

    public async Task<IEnumerable<UserActivityLog>> GetByUserIdAsync(string userId)
    {
        var filter = Builders<UserActivityLog>.Filter.Eq(u => u.UserId, userId);
        var sort = Builders<UserActivityLog>.Sort.Descending(u => u.Timestamp);
        return await _collection.Find(filter).Sort(sort).ToListAsync();
    }

    public async Task<IEnumerable<UserActivityLog>> GetByCourseIdAsync(string courseId)
    {
        var filter = Builders<UserActivityLog>.Filter.And(
            Builders<UserActivityLog>.Filter.Eq(u => u.EntityType, "course"),
            Builders<UserActivityLog>.Filter.Eq(u => u.EntityId, courseId)
        );
        var sort = Builders<UserActivityLog>.Sort.Descending(u => u.Timestamp);
        return await _collection.Find(filter).Sort(sort).ToListAsync();
    }
}

