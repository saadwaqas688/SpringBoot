using CoursesService.Models;
using MongoDB.Driver;
using Shared.Repositories;

namespace CoursesService.Repositories;

public class DiscussionPostRepository : BaseRepository<DiscussionPost>, IDiscussionPostRepository
{
    public DiscussionPostRepository(IMongoCollection<DiscussionPost> collection, ILogger<DiscussionPostRepository> logger)
        : base(collection, logger)
    {
    }

    public async Task<IEnumerable<DiscussionPost>> GetByLessonIdAsync(string lessonId)
    {
        var filter = Builders<DiscussionPost>.Filter.Eq(d => d.LessonId, lessonId);
        var sort = Builders<DiscussionPost>.Sort.Descending(d => d.CreatedAt);
        return await _collection.Find(filter).Sort(sort).ToListAsync();
    }

    public async Task<IEnumerable<DiscussionPost>> GetCommentsByPostIdAsync(string postId)
    {
        var filter = Builders<DiscussionPost>.Filter.Eq(d => d.ParentPostId, postId);
        var sort = Builders<DiscussionPost>.Sort.Ascending(d => d.CreatedAt);
        return await _collection.Find(filter).Sort(sort).ToListAsync();
    }

    public async Task<IEnumerable<DiscussionPost>> GetPostsByLessonIdAsync(string lessonId)
    {
        var filter = Builders<DiscussionPost>.Filter.And(
            Builders<DiscussionPost>.Filter.Eq(d => d.LessonId, lessonId),
            Builders<DiscussionPost>.Filter.Eq(d => d.ParentPostId, (string?)null)
        );
        var sort = Builders<DiscussionPost>.Sort.Descending(d => d.CreatedAt);
        return await _collection.Find(filter).Sort(sort).ToListAsync();
    }
}

