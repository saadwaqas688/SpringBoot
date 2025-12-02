using CoursesService.Models;
using Shared.Repositories;

namespace CoursesService.Repositories;

public interface IDiscussionRepository : IBaseRepository<Discussion>
{
    Task<Discussion?> GetByLessonIdAsync(string lessonId);
}

