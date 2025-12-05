using CoursesService.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using Shared.Infrastructure.Repositories;

namespace CoursesService.Repositories;

public class CourseRepository : BaseRepository<Course>, ICourseRepository
{
    public CourseRepository(IMongoCollection<Course> collection, ILogger<CourseRepository> logger)
        : base(collection, logger)
    {
    }

    public async Task<IEnumerable<Course>> GetByStatusAsync(string status)
    {
        var filter = Builders<Course>.Filter.Eq(c => c.Status, status);
        return await _collection.Find(filter).ToListAsync();
    }

    public async Task<IEnumerable<Course>> SearchAsync(string searchTerm)
    {
        var filter = Builders<Course>.Filter.Or(
            Builders<Course>.Filter.Regex(c => c.Title, new BsonRegularExpression(searchTerm, "i")),
            Builders<Course>.Filter.Regex(c => c.Description, new BsonRegularExpression(searchTerm, "i"))
        );
        return await _collection.Find(filter).ToListAsync();
    }
}


