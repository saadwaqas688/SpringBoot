using LMS.API.Data;
using LMS.API.Models;
using MongoDB.Driver;

namespace LMS.API.Repositories;

public class LessonProgressRepository : ILessonProgressRepository
{
    private readonly MongoDbContext _context;

    public LessonProgressRepository(MongoDbContext context)
    {
        _context = context;
    }

    public async Task<List<LessonProgress>> GetAllAsync()
    {
        return await _context.LessonProgresses.Find(_ => true).ToListAsync();
    }

    public async Task<LessonProgress?> GetByIdAsync(string id)
    {
        return await _context.LessonProgresses.Find(lp => lp.Id == id).FirstOrDefaultAsync();
    }

    public async Task<LessonProgress?> GetByUserAndLessonAsync(string userId, string lessonId)
    {
        return await _context.LessonProgresses
            .Find(lp => lp.UserId == userId && lp.LessonId == lessonId)
            .FirstOrDefaultAsync();
    }

    public async Task<List<LessonProgress>> GetByUserIdAsync(string userId)
    {
        return await _context.LessonProgresses.Find(lp => lp.UserId == userId).ToListAsync();
    }

    public async Task<List<LessonProgress>> GetByCourseIdAsync(string courseId)
    {
        return await _context.LessonProgresses.Find(lp => lp.CourseId == courseId).ToListAsync();
    }

    public async Task<LessonProgress> CreateAsync(LessonProgress progress)
    {
        await _context.LessonProgresses.InsertOneAsync(progress);
        return progress;
    }

    public async Task<LessonProgress> UpdateAsync(LessonProgress progress)
    {
        await _context.LessonProgresses.ReplaceOneAsync(lp => lp.Id == progress.Id, progress);
        return progress;
    }

    public async Task DeleteAsync(string id)
    {
        await _context.LessonProgresses.DeleteOneAsync(lp => lp.Id == id);
    }

    public async Task<bool> ExistsAsync(string userId, string lessonId)
    {
        var progress = await _context.LessonProgresses
            .Find(lp => lp.UserId == userId && lp.LessonId == lessonId)
            .FirstOrDefaultAsync();
        return progress != null;
    }
}

