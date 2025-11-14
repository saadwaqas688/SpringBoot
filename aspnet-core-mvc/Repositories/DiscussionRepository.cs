using CourseManagementAPI.Models;
using MongoDB.Driver;

namespace CourseManagementAPI.Repositories;

public class DiscussionRepository : IDiscussionRepository
{
    private readonly IMongoCollection<Discussion> _discussions;

    public DiscussionRepository(IMongoDatabase database)
    {
        _discussions = database.GetCollection<Discussion>("discussions");
    }

    public async Task<List<Discussion>> GetAllAsync()
    {
        return await _discussions.Find(_ => true).ToListAsync();
    }

    public async Task<Discussion?> GetByIdAsync(string id)
    {
        return await _discussions.Find(d => d.Id == id).FirstOrDefaultAsync();
    }

    public async Task<List<Discussion>> GetByCourseIdAsync(string courseId)
    {
        return await _discussions.Find(d => d.CourseId == courseId)
            .SortByDescending(d => d.CreatedAt)
            .ToListAsync();
    }

    public async Task<Discussion> CreateAsync(Discussion discussion)
    {
        await _discussions.InsertOneAsync(discussion);
        return discussion;
    }

    public async Task<Discussion> UpdateAsync(Discussion discussion)
    {
        await _discussions.ReplaceOneAsync(d => d.Id == discussion.Id, discussion);
        return discussion;
    }

    public async Task DeleteAsync(string id)
    {
        await _discussions.DeleteOneAsync(d => d.Id == id);
    }
}



