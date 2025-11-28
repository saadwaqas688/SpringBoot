using CoursesService.Models;
using MongoDB.Driver;
using Shared.Repositories;

namespace CoursesService.Repositories;

public class UserSlideProgressRepository : BaseRepository<UserSlideProgress>, IUserSlideProgressRepository
{
    public UserSlideProgressRepository(IMongoCollection<UserSlideProgress> collection, ILogger<UserSlideProgressRepository> logger)
        : base(collection, logger)
    {
    }

    public async Task<UserSlideProgress?> GetByUserAndSlideAsync(string userId, string slideId)
    {
        var filter = Builders<UserSlideProgress>.Filter.And(
            Builders<UserSlideProgress>.Filter.Eq(u => u.UserId, userId),
            Builders<UserSlideProgress>.Filter.Eq(u => u.SlideId, slideId)
        );
        return await _collection.Find(filter).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<UserSlideProgress>> GetByUserIdAsync(string userId)
    {
        var filter = Builders<UserSlideProgress>.Filter.Eq(u => u.UserId, userId);
        return await _collection.Find(filter).ToListAsync();
    }
}

