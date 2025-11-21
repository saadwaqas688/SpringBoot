using LMS.API.DTOs;

namespace LMS.API.Services;

public interface ILessonContentService
{
    Task<List<LessonContentDto>> GetAllContentsAsync();
    Task<LessonContentDto?> GetContentByIdAsync(string id);
    Task<List<LessonContentDto>> GetContentsByLessonIdAsync(string lessonId);
    Task<LessonContentDto> CreateContentAsync(CreateLessonContentDto dto);
    Task<LessonContentDto> UpdateContentAsync(string id, UpdateLessonContentDto dto);
    Task DeleteContentAsync(string id);
}

