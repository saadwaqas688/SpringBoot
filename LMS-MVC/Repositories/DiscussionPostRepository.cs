using LMS_MVC.Data;
using LMS_MVC.Models;
using Microsoft.EntityFrameworkCore;

namespace LMS_MVC.Repositories;

public class DiscussionPostRepository : IDiscussionPostRepository
{
    private readonly ApplicationDbContext _context;

    public DiscussionPostRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<DiscussionPost>> GetByContentIdAsync(int contentId)
    {
        return await _context.DiscussionPosts
            .Include(dp => dp.User)
            .Include(dp => dp.Replies)
                .ThenInclude(r => r.User)
            .Where(dp => dp.ContentId == contentId && dp.ParentPostId == null)
            .OrderBy(dp => dp.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<DiscussionPost>> GetByLessonIdAsync(int lessonId)
    {
        return await _context.DiscussionPosts
            .Include(dp => dp.User)
            .Include(dp => dp.Replies)
                .ThenInclude(r => r.User)
            .Where(dp => dp.LessonId == lessonId && dp.ParentPostId == null)
            .OrderBy(dp => dp.CreatedAt)
            .ToListAsync();
    }

    public async Task<DiscussionPost?> GetByIdAsync(int id)
    {
        return await _context.DiscussionPosts
            .Include(dp => dp.User)
            .Include(dp => dp.ParentPost)
            .Include(dp => dp.Replies)
                .ThenInclude(r => r.User)
            .FirstOrDefaultAsync(dp => dp.Id == id);
    }

    public async Task<DiscussionPost> CreateAsync(DiscussionPost post)
    {
        try
        {
            _context.DiscussionPosts.Add(post);
            await _context.SaveChangesAsync();
            return post;
        }
        catch (Microsoft.EntityFrameworkCore.DbUpdateException ex)
        {
            // Check if it's a foreign key constraint violation
            if (ex.InnerException != null && ex.InnerException.Message.Contains("FOREIGN KEY"))
            {
                throw new Exception("Invalid lesson or content reference. Please ensure the lesson and content exist.", ex);
            }
            throw new Exception($"Database error: {ex.Message}", ex);
        }
    }

    public async Task<DiscussionPost> UpdateAsync(DiscussionPost post)
    {
        _context.DiscussionPosts.Update(post);
        await _context.SaveChangesAsync();
        return post;
    }

    public async Task DeleteAsync(int id)
    {
        var post = await GetByIdAsync(id);
        if (post != null)
        {
            _context.DiscussionPosts.Remove(post);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.DiscussionPosts.AnyAsync(dp => dp.Id == id);
    }
}

