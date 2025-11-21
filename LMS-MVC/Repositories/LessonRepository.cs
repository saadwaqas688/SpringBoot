using LMS_MVC.Data;
using LMS_MVC.Models;
using Microsoft.EntityFrameworkCore;

namespace LMS_MVC.Repositories;

public class LessonRepository : ILessonRepository
{
    private readonly ApplicationDbContext _context;

    public LessonRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Lesson>> GetByCourseIdAsync(int courseId)
    {
        return await _context.Lessons
            .Include(l => l.Contents)
            .Where(l => l.CourseId == courseId)
            .OrderBy(l => l.Order)
            .ToListAsync();
    }

    public async Task<Lesson?> GetByIdAsync(int id)
    {
        return await _context.Lessons
            .Include(l => l.Contents.OrderBy(c => c.Order))
            .Include(l => l.Course)
            .FirstOrDefaultAsync(l => l.Id == id);
    }

    public async Task<Lesson> CreateAsync(Lesson lesson)
    {
        _context.Lessons.Add(lesson);
        await _context.SaveChangesAsync();
        return lesson;
    }

    public async Task<Lesson> UpdateAsync(Lesson lesson)
    {
        _context.Lessons.Update(lesson);
        await _context.SaveChangesAsync();
        return lesson;
    }

    public async Task DeleteAsync(int id)
    {
        var lesson = await GetByIdAsync(id);
        if (lesson != null)
        {
            _context.Lessons.Remove(lesson);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Lessons.AnyAsync(l => l.Id == id);
    }
}

