using LMS_MVC.Data;
using LMS_MVC.Models;
using Microsoft.EntityFrameworkCore;

namespace LMS_MVC.Repositories;

public class LessonProgressRepository : ILessonProgressRepository
{
    private readonly ApplicationDbContext _context;

    public LessonProgressRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<LessonProgress?> GetByUserAndLessonAsync(string userId, int lessonId)
    {
        return await _context.LessonProgresses
            .Include(lp => lp.Lesson)
            .Include(lp => lp.Course)
            .FirstOrDefaultAsync(lp => lp.UserId == userId && lp.LessonId == lessonId);
    }

    public async Task<List<LessonProgress>> GetByUserAndCourseAsync(string userId, int courseId)
    {
        return await _context.LessonProgresses
            .Include(lp => lp.Lesson)
            .Where(lp => lp.UserId == userId && lp.CourseId == courseId)
            .ToListAsync();
    }

    public async Task<LessonProgress> CreateOrUpdateAsync(LessonProgress progress)
    {
        var existing = await GetByUserAndLessonAsync(progress.UserId, progress.LessonId);
        
        if (existing != null)
        {
            existing.IsCompleted = progress.IsCompleted;
            existing.CompletedAt = progress.CompletedAt;
            _context.LessonProgresses.Update(existing);
            await _context.SaveChangesAsync();
            return existing;
        }
        else
        {
            _context.LessonProgresses.Add(progress);
            await _context.SaveChangesAsync();
            return progress;
        }
    }

    public async Task<bool> IsCompletedAsync(string userId, int lessonId)
    {
        var progress = await GetByUserAndLessonAsync(userId, lessonId);
        return progress?.IsCompleted ?? false;
    }
}

