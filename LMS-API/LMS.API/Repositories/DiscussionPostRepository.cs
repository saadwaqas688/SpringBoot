using LMS.API.Data;
using LMS.API.Models;
using MongoDB.Driver;

namespace LMS.API.Repositories;

public class DiscussionPostRepository : IDiscussionPostRepository
{
    private readonly MongoDbContext _context;

    public DiscussionPostRepository(MongoDbContext context)
    {
        _context = context;
    }

    public async Task<List<DiscussionPost>> GetAllAsync()
    {
        return await _context.DiscussionPosts.Find(_ => true).ToListAsync();
    }

    public async Task<DiscussionPost?> GetByIdAsync(string id)
    {
        return await _context.DiscussionPosts.Find(p => p.Id == id).FirstOrDefaultAsync();
    }

    public async Task<List<DiscussionPost>> GetByLessonIdAsync(string lessonId)
    {
        return await _context.DiscussionPosts.Find(p => p.LessonId == lessonId)
            .SortByDescending(p => p.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<DiscussionPost>> GetByContentIdAsync(string contentId)
    {
        return await _context.DiscussionPosts.Find(p => p.ContentId == contentId)
            .SortByDescending(p => p.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<DiscussionPost>> GetRepliesAsync(string parentPostId)
    {
        return await _context.DiscussionPosts.Find(p => p.ParentPostId == parentPostId)
            .SortBy(p => p.CreatedAt)
            .ToListAsync();
    }

    public async Task<DiscussionPost> CreateAsync(DiscussionPost post)
    {
        await _context.DiscussionPosts.InsertOneAsync(post);
        return post;
    }

    public async Task<DiscussionPost> UpdateAsync(DiscussionPost post)
    {
        await _context.DiscussionPosts.ReplaceOneAsync(p => p.Id == post.Id, post);
        return post;
    }

    public async Task DeleteAsync(string id)
    {
        await _context.DiscussionPosts.DeleteOneAsync(p => p.Id == id);
    }

    public async Task<bool> ExistsAsync(string id)
    {
        var post = await _context.DiscussionPosts.Find(p => p.Id == id).FirstOrDefaultAsync();
        return post != null;
    }
}

