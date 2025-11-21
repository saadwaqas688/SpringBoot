using LMS.API.Data;
using LMS.API.Models;
using MongoDB.Driver;

namespace LMS.API.Repositories;

public class LessonContentRepository : ILessonContentRepository
{
    private readonly MongoDbContext _context;

    public LessonContentRepository(MongoDbContext context)
    {
        _context = context;
    }

    public async Task<List<LessonContent>> GetAllAsync()
    {
        return await _context.LessonContents.Find(_ => true).ToListAsync();
    }

    public async Task<LessonContent?> GetByIdAsync(string id)
    {
        return await _context.LessonContents.Find(lc => lc.Id == id).FirstOrDefaultAsync();
    }

    public async Task<List<LessonContent>> GetByLessonIdAsync(string lessonId)
    {
        return await _context.LessonContents.Find(lc => lc.LessonId == lessonId)
            .SortBy(lc => lc.Order)
            .ToListAsync();
    }

    public async Task<LessonContent> CreateAsync(LessonContent content)
    {
        await _context.LessonContents.InsertOneAsync(content);
        return content;
    }

    public async Task<LessonContent> UpdateAsync(LessonContent content)
    {
        await _context.LessonContents.ReplaceOneAsync(lc => lc.Id == content.Id, content);
        return content;
    }

    public async Task DeleteAsync(string id)
    {
        await _context.LessonContents.DeleteOneAsync(lc => lc.Id == id);
    }

    public async Task<bool> ExistsAsync(string id)
    {
        var content = await _context.LessonContents.Find(lc => lc.Id == id).FirstOrDefaultAsync();
        return content != null;
    }
}

