using CoursesService.Models;
using MongoDB.Driver;
using Shared.Infrastructure.Repositories;

namespace CoursesService.Repositories;

public class DiscussionRepository : BaseRepository<Discussion>, IDiscussionRepository
{
    public DiscussionRepository(IMongoCollection<Discussion> collection, ILogger<DiscussionRepository> logger)
        : base(collection, logger)
    {
    }

    public async Task<Discussion?> GetByLessonIdAsync(string lessonId)
    {
        var filter = Builders<Discussion>.Filter.Eq(d => d.LessonId, lessonId);
        return await _collection.Find(filter).FirstOrDefaultAsync();
    }
}


