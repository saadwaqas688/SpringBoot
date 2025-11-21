using LMS_MVC.DTOs;

namespace LMS_MVC.Services;

public interface ILessonContentService
{
    Task<List<LessonContentDto>> GetContentsByLessonIdAsync(int lessonId);
    Task<LessonContentDto?> GetContentByIdAsync(int id);
    Task<LessonContentDto> CreateContentAsync(CreateLessonContentDto dto, string userId);
    Task<LessonContentDto> UpdateContentAsync(int id, UpdateLessonContentDto dto, string userId);
    Task DeleteContentAsync(int id, string userId);
}

