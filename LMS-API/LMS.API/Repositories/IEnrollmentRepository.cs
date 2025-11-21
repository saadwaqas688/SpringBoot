using LMS.API.Models;

namespace LMS.API.Repositories;

public interface IEnrollmentRepository
{
    Task<List<UserCourse>> GetAllAsync();
    Task<UserCourse?> GetByIdAsync(string id);
    Task<UserCourse?> GetByUserAndCourseAsync(string userId, string courseId);
    Task<List<UserCourse>> GetByUserIdAsync(string userId);
    Task<List<UserCourse>> GetByCourseIdAsync(string courseId);
    Task<UserCourse> CreateAsync(UserCourse enrollment);
    Task DeleteAsync(string id);
    Task<bool> ExistsAsync(string userId, string courseId);
}

