using LMS.API.Models;

namespace LMS.API.Repositories;

public interface ILessonContentRepository
{
    Task<List<LessonContent>> GetAllAsync();
    Task<LessonContent?> GetByIdAsync(string id);
    Task<List<LessonContent>> GetByLessonIdAsync(string lessonId);
    Task<LessonContent> CreateAsync(LessonContent content);
    Task<LessonContent> UpdateAsync(LessonContent content);
    Task DeleteAsync(string id);
    Task<bool> ExistsAsync(string id);
}

