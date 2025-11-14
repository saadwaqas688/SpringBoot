using CourseManagementAPI.Models;
using MongoDB.Driver;

namespace CourseManagementAPI.Repositories;

public class PostRepository : IPostRepository
{
    private readonly IMongoCollection<Post> _posts;

    public PostRepository(IMongoDatabase database)
    {
        _posts = database.GetCollection<Post>("posts");
    }

    public async Task<List<Post>> GetByDiscussionIdAsync(string discussionId)
    {
        return await _posts.Find(p => p.DiscussionId == discussionId)
            .SortBy(p => p.CreatedAt)
            .ToListAsync();
    }

    public async Task<Post?> GetByIdAsync(string id)
    {
        return await _posts.Find(p => p.Id == id).FirstOrDefaultAsync();
    }

    public async Task<Post> CreateAsync(Post post)
    {
        await _posts.InsertOneAsync(post);
        return post;
    }

    public async Task<Post> UpdateAsync(Post post)
    {
        await _posts.ReplaceOneAsync(p => p.Id == post.Id, post);
        return post;
    }

    public async Task DeleteAsync(string id)
    {
        await _posts.DeleteOneAsync(p => p.Id == id);
    }
}

