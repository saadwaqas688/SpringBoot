using CoursesService.Models;
using Shared.Repositories;

namespace CoursesService.Repositories;

public interface ILessonRepository : IBaseRepository<Lesson>
{
    Task<IEnumerable<Lesson>> GetByCourseIdAsync(string courseId);
    Task<IEnumerable<Lesson>> GetByCourseIdAndTypeAsync(string courseId, string lessonType);
}

