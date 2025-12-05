using CoursesService.Models;
using Shared.Infrastructure.Repositories;

namespace CoursesService.Repositories;

public interface IUserLessonProgressRepository : IBaseRepository<UserLessonProgress>
{
    Task<UserLessonProgress?> GetByUserAndLessonAsync(string userId, string lessonId);
    Task<IEnumerable<UserLessonProgress>> GetByUserIdAsync(string userId);
}


