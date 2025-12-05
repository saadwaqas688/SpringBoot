using CoursesService.Models;
using MongoDB.Driver;
using Shared.Infrastructure.Repositories;

namespace CoursesService.Repositories;

public class LessonRepository : BaseRepository<Lesson>, ILessonRepository
{
    public LessonRepository(IMongoCollection<Lesson> collection, ILogger<LessonRepository> logger)
        : base(collection, logger)
    {
    }

    public async Task<IEnumerable<Lesson>> GetByCourseIdAsync(string courseId)
    {
        var filter = Builders<Lesson>.Filter.Eq(l => l.CourseId, courseId);
        var sort = Builders<Lesson>.Sort.Ascending(l => l.Order);
        return await _collection.Find(filter).Sort(sort).ToListAsync();
    }

    public async Task<IEnumerable<Lesson>> GetByCourseIdAndTypeAsync(string courseId, string lessonType)
    {
        var filter = Builders<Lesson>.Filter.And(
            Builders<Lesson>.Filter.Eq(l => l.CourseId, courseId),
            Builders<Lesson>.Filter.Eq(l => l.LessonType, lessonType)
        );
        var sort = Builders<Lesson>.Sort.Ascending(l => l.Order);
        return await _collection.Find(filter).Sort(sort).ToListAsync();
    }
}


