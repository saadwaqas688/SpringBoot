using LMS.API.Models;

namespace LMS.API.Repositories;

public interface ILessonRepository
{
    Task<List<Lesson>> GetAllAsync();
    Task<Lesson?> GetByIdAsync(string id);
    Task<List<Lesson>> GetByCourseIdAsync(string courseId);
    Task<Lesson> CreateAsync(Lesson lesson);
    Task<Lesson> UpdateAsync(Lesson lesson);
    Task DeleteAsync(string id);
    Task<bool> ExistsAsync(string id);
}

