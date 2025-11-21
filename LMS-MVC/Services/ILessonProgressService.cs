using LMS_MVC.DTOs;

namespace LMS_MVC.Services;

public interface ILessonProgressService
{
    Task<LessonProgressDto?> GetProgressAsync(string userId, int lessonId);
    Task<List<LessonProgressDto>> GetProgressByCourseAsync(string userId, int courseId);
    Task<LessonProgressDto> UpdateProgressAsync(string userId, int lessonId, int courseId, UpdateLessonProgressDto dto);
}

