using LMS_MVC.Data;
using LMS_MVC.Models;
using Microsoft.EntityFrameworkCore;

namespace LMS_MVC.Repositories;

public class LessonContentRepository : ILessonContentRepository
{
    private readonly ApplicationDbContext _context;

    public LessonContentRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<LessonContent>> GetByLessonIdAsync(int lessonId)
    {
        return await _context.LessonContents
            .Where(lc => lc.LessonId == lessonId)
            .OrderBy(lc => lc.Order)
            .ToListAsync();
    }

    public async Task<LessonContent?> GetByIdAsync(int id)
    {
        return await _context.LessonContents
            .Include(lc => lc.Lesson)
            .FirstOrDefaultAsync(lc => lc.Id == id);
    }

    public async Task<LessonContent> CreateAsync(LessonContent content)
    {
        _context.LessonContents.Add(content);
        await _context.SaveChangesAsync();
        return content;
    }

    public async Task<LessonContent> UpdateAsync(LessonContent content)
    {
        _context.LessonContents.Update(content);
        await _context.SaveChangesAsync();
        return content;
    }

    public async Task DeleteAsync(int id)
    {
        var content = await GetByIdAsync(id);
        if (content != null)
        {
            _context.LessonContents.Remove(content);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.LessonContents.AnyAsync(lc => lc.Id == id);
    }
}

