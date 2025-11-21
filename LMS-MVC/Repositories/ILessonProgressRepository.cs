using LMS_MVC.Models;

namespace LMS_MVC.Repositories;

public interface ILessonProgressRepository
{
    Task<LessonProgress?> GetByUserAndLessonAsync(string userId, int lessonId);
    Task<List<LessonProgress>> GetByUserAndCourseAsync(string userId, int courseId);
    Task<LessonProgress> CreateOrUpdateAsync(LessonProgress progress);
    Task<bool> IsCompletedAsync(string userId, int lessonId);
}

