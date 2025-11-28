using CoursesService.Models;
using Shared.Repositories;

namespace CoursesService.Repositories;

public interface IStandardSlideRepository : IBaseRepository<StandardSlide>
{
    Task<IEnumerable<StandardSlide>> GetByLessonIdAsync(string lessonId);
}

