using CoursesService.Models;
using Shared.Infrastructure.Repositories;

namespace CoursesService.Repositories;

public interface IDiscussionRepository : IBaseRepository<Discussion>
{
    Task<Discussion?> GetByLessonIdAsync(string lessonId);
}


