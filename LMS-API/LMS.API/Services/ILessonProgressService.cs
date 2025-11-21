using LMS.API.DTOs;

namespace LMS.API.Services;

public interface ILessonProgressService
{
    Task<LessonProgressDto> UpdateProgressAsync(string userId, string lessonId, string courseId, UpdateLessonProgressDto dto);
    Task<LessonProgressDto?> GetProgressAsync(string userId, string lessonId);
    Task<List<LessonProgressDto>> GetUserProgressAsync(string userId);
}

