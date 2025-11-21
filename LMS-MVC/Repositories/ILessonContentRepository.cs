using LMS_MVC.Models;

namespace LMS_MVC.Repositories;

public interface ILessonContentRepository
{
    Task<List<LessonContent>> GetByLessonIdAsync(int lessonId);
    Task<LessonContent?> GetByIdAsync(int id);
    Task<LessonContent> CreateAsync(LessonContent content);
    Task<LessonContent> UpdateAsync(LessonContent content);
    Task DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
}

