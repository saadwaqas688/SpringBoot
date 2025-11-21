using LMS.API.DTOs;

namespace LMS.API.Services;

public interface ILessonService
{
    Task<List<LessonDto>> GetAllLessonsAsync();
    Task<LessonDto?> GetLessonByIdAsync(string id);
    Task<List<LessonDto>> GetLessonsByCourseIdAsync(string courseId);
    Task<LessonDto> CreateLessonAsync(CreateLessonDto dto);
    Task<LessonDto> UpdateLessonAsync(string id, UpdateLessonDto dto);
    Task DeleteLessonAsync(string id);
}

