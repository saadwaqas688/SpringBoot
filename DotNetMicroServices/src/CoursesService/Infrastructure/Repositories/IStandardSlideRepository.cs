using CoursesService.Models;
using Shared.Infrastructure.Repositories;

namespace CoursesService.Repositories;

public interface IStandardSlideRepository : IBaseRepository<StandardSlide>
{
    Task<IEnumerable<StandardSlide>> GetByLessonIdAsync(string lessonId);
}


