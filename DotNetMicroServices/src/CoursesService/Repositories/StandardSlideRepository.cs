using CoursesService.Models;
using MongoDB.Driver;
using Shared.Repositories;

namespace CoursesService.Repositories;

public class StandardSlideRepository : BaseRepository<StandardSlide>, IStandardSlideRepository
{
    public StandardSlideRepository(IMongoCollection<StandardSlide> collection, ILogger<StandardSlideRepository> logger)
        : base(collection, logger)
    {
    }

    public async Task<IEnumerable<StandardSlide>> GetByLessonIdAsync(string lessonId)
    {
        var filter = Builders<StandardSlide>.Filter.Eq(s => s.LessonId, lessonId);
        var sort = Builders<StandardSlide>.Sort.Ascending(s => s.Order);
        return await _collection.Find(filter).Sort(sort).ToListAsync();
    }
}

