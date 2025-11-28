using CoursesService.Models;
using Shared.Repositories;

namespace CoursesService.Repositories;

public interface IQuizRepository : IBaseRepository<Quiz>
{
    Task<Quiz?> GetByLessonIdAsync(string lessonId);
}

