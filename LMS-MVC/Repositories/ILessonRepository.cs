using LMS_MVC.Models;

namespace LMS_MVC.Repositories;

public interface ILessonRepository
{
    Task<List<Lesson>> GetByCourseIdAsync(int courseId);
    Task<Lesson?> GetByIdAsync(int id);
    Task<Lesson> CreateAsync(Lesson lesson);
    Task<Lesson> UpdateAsync(Lesson lesson);
    Task DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
}

