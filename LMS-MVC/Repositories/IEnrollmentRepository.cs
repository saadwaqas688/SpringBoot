using LMS_MVC.Models;

namespace LMS_MVC.Repositories;

public interface IEnrollmentRepository
{
    Task<List<UserCourse>> GetByUserIdAsync(string userId);
    Task<List<UserCourse>> GetByCourseIdAsync(int courseId);
    Task<UserCourse?> GetByUserAndCourseAsync(string userId, int courseId);
    Task<UserCourse> CreateAsync(UserCourse enrollment);
    Task DeleteAsync(string userId, int courseId);
    Task<bool> IsEnrolledAsync(string userId, int courseId);
}

