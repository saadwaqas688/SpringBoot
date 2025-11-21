using LMS_MVC.Data;
using LMS_MVC.Models;
using Microsoft.EntityFrameworkCore;

namespace LMS_MVC.Repositories;

public class EnrollmentRepository : IEnrollmentRepository
{
    private readonly ApplicationDbContext _context;

    public EnrollmentRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<UserCourse>> GetByUserIdAsync(string userId)
    {
        return await _context.UserCourses
            .Include(uc => uc.Course)
            .Include(uc => uc.User)
            .Where(uc => uc.UserId == userId)
            .ToListAsync();
    }

    public async Task<List<UserCourse>> GetByCourseIdAsync(int courseId)
    {
        return await _context.UserCourses
            .Include(uc => uc.User)
            .Include(uc => uc.Course)
            .Where(uc => uc.CourseId == courseId)
            .ToListAsync();
    }

    public async Task<UserCourse?> GetByUserAndCourseAsync(string userId, int courseId)
    {
        return await _context.UserCourses
            .Include(uc => uc.Course)
            .Include(uc => uc.User)
            .FirstOrDefaultAsync(uc => uc.UserId == userId && uc.CourseId == courseId);
    }

    public async Task<UserCourse> CreateAsync(UserCourse enrollment)
    {
        _context.UserCourses.Add(enrollment);
        await _context.SaveChangesAsync();
        return enrollment;
    }

    public async Task DeleteAsync(string userId, int courseId)
    {
        var enrollment = await GetByUserAndCourseAsync(userId, courseId);
        if (enrollment != null)
        {
            _context.UserCourses.Remove(enrollment);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> IsEnrolledAsync(string userId, int courseId)
    {
        return await _context.UserCourses
            .AnyAsync(uc => uc.UserId == userId && uc.CourseId == courseId);
    }
}

