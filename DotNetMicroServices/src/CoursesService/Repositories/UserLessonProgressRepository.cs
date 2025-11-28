using CoursesService.Models;
using MongoDB.Driver;
using Shared.Repositories;

namespace CoursesService.Repositories;

public class UserLessonProgressRepository : BaseRepository<UserLessonProgress>, IUserLessonProgressRepository
{
    public UserLessonProgressRepository(IMongoCollection<UserLessonProgress> collection, ILogger<UserLessonProgressRepository> logger)
        : base(collection, logger)
    {
    }

    public async Task<UserLessonProgress?> GetByUserAndLessonAsync(string userId, string lessonId)
    {
        var filter = Builders<UserLessonProgress>.Filter.And(
            Builders<UserLessonProgress>.Filter.Eq(u => u.UserId, userId),
            Builders<UserLessonProgress>.Filter.Eq(u => u.LessonId, lessonId)
        );
        return await _collection.Find(filter).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<UserLessonProgress>> GetByUserIdAsync(string userId)
    {
        var filter = Builders<UserLessonProgress>.Filter.Eq(u => u.UserId, userId);
        return await _collection.Find(filter).ToListAsync();
    }
}

