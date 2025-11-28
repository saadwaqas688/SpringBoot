using CoursesService.Models;
using Shared.Repositories;

namespace CoursesService.Repositories;

public interface IUserCourseRepository : IBaseRepository<UserCourse>
{
    Task<UserCourse?> GetByUserAndCourseAsync(string userId, string courseId);
    Task<IEnumerable<UserCourse>> GetByUserIdAsync(string userId);
    Task<IEnumerable<UserCourse>> GetByCourseIdAsync(string courseId);
}

