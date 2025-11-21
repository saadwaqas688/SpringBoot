using LMS.API.Data;
using LMS.API.Models;
using MongoDB.Driver;

namespace LMS.API.Repositories;

public class LessonRepository : ILessonRepository
{
    private readonly MongoDbContext _context;

    public LessonRepository(MongoDbContext context)
    {
        _context = context;
    }

    public async Task<List<Lesson>> GetAllAsync()
    {
        return await _context.Lessons.Find(_ => true).ToListAsync();
    }

    public async Task<Lesson?> GetByIdAsync(string id)
    {
        return await _context.Lessons.Find(l => l.Id == id).FirstOrDefaultAsync();
    }

    public async Task<List<Lesson>> GetByCourseIdAsync(string courseId)
    {
        return await _context.Lessons.Find(l => l.CourseId == courseId).ToListAsync();
    }

    public async Task<Lesson> CreateAsync(Lesson lesson)
    {
        await _context.Lessons.InsertOneAsync(lesson);
        return lesson;
    }

    public async Task<Lesson> UpdateAsync(Lesson lesson)
    {
        await _context.Lessons.ReplaceOneAsync(l => l.Id == lesson.Id, lesson);
        return lesson;
    }

    public async Task DeleteAsync(string id)
    {
        await _context.Lessons.DeleteOneAsync(l => l.Id == id);
    }

    public async Task<bool> ExistsAsync(string id)
    {
        var lesson = await _context.Lessons.Find(l => l.Id == id).FirstOrDefaultAsync();
        return lesson != null;
    }
}

