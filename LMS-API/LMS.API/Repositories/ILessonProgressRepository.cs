using LMS.API.Models;

namespace LMS.API.Repositories;

public interface ILessonProgressRepository
{
    Task<List<LessonProgress>> GetAllAsync();
    Task<LessonProgress?> GetByIdAsync(string id);
    Task<LessonProgress?> GetByUserAndLessonAsync(string userId, string lessonId);
    Task<List<LessonProgress>> GetByUserIdAsync(string userId);
    Task<List<LessonProgress>> GetByCourseIdAsync(string courseId);
    Task<LessonProgress> CreateAsync(LessonProgress progress);
    Task<LessonProgress> UpdateAsync(LessonProgress progress);
    Task DeleteAsync(string id);
    Task<bool> ExistsAsync(string userId, string lessonId);
}

