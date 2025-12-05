using CoursesService.Models;
using Shared.Infrastructure.Repositories;

namespace CoursesService.Repositories;

public interface IUserActivityLogRepository : IBaseRepository<UserActivityLog>
{
    Task<IEnumerable<UserActivityLog>> GetByUserIdAsync(string userId);
    Task<IEnumerable<UserActivityLog>> GetByCourseIdAsync(string courseId);
}


