using CoursesService.Models;
using MongoDB.Driver;
using Shared.Repositories;

namespace CoursesService.Repositories;

public class UserCourseRepository : BaseRepository<UserCourse>, IUserCourseRepository
{
    public UserCourseRepository(IMongoCollection<UserCourse> collection, ILogger<UserCourseRepository> logger)
        : base(collection, logger)
    {
    }

    public async Task<UserCourse?> GetByUserAndCourseAsync(string userId, string courseId)
    {
        var filter = Builders<UserCourse>.Filter.And(
            Builders<UserCourse>.Filter.Eq(u => u.UserId, userId),
            Builders<UserCourse>.Filter.Eq(u => u.CourseId, courseId)
        );
        return await _collection.Find(filter).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<UserCourse>> GetByUserIdAsync(string userId)
    {
        var filter = Builders<UserCourse>.Filter.Eq(u => u.UserId, userId);
        return await _collection.Find(filter).ToListAsync();
    }

    public async Task<IEnumerable<UserCourse>> GetByCourseIdAsync(string courseId)
    {
        var filter = Builders<UserCourse>.Filter.Eq(u => u.CourseId, courseId);
        return await _collection.Find(filter).ToListAsync();
    }
}

