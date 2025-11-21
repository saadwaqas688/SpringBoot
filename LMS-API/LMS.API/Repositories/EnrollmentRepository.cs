using LMS.API.Data;
using LMS.API.Models;
using MongoDB.Driver;

namespace LMS.API.Repositories;

public class EnrollmentRepository : IEnrollmentRepository
{
    private readonly MongoDbContext _context;

    public EnrollmentRepository(MongoDbContext context)
    {
        _context = context;
    }

    public async Task<List<UserCourse>> GetAllAsync()
    {
        return await _context.UserCourses.Find(_ => true).ToListAsync();
    }

    public async Task<UserCourse?> GetByIdAsync(string id)
    {
        return await _context.UserCourses.Find(uc => uc.Id == id).FirstOrDefaultAsync();
    }

    public async Task<UserCourse?> GetByUserAndCourseAsync(string userId, string courseId)
    {
        return await _context.UserCourses.Find(uc => uc.UserId == userId && uc.CourseId == courseId)
            .FirstOrDefaultAsync();
    }

    public async Task<List<UserCourse>> GetByUserIdAsync(string userId)
    {
        return await _context.UserCourses.Find(uc => uc.UserId == userId).ToListAsync();
    }

    public async Task<List<UserCourse>> GetByCourseIdAsync(string courseId)
    {
        return await _context.UserCourses.Find(uc => uc.CourseId == courseId).ToListAsync();
    }

    public async Task<UserCourse> CreateAsync(UserCourse enrollment)
    {
        await _context.UserCourses.InsertOneAsync(enrollment);
        return enrollment;
    }

    public async Task DeleteAsync(string id)
    {
        await _context.UserCourses.DeleteOneAsync(uc => uc.Id == id);
    }

    public async Task<bool> ExistsAsync(string userId, string courseId)
    {
        var enrollment = await _context.UserCourses
            .Find(uc => uc.UserId == userId && uc.CourseId == courseId)
            .FirstOrDefaultAsync();
        return enrollment != null;
    }
}

