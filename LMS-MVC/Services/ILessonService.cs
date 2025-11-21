using LMS_MVC.DTOs;

namespace LMS_MVC.Services;

public interface ILessonService
{
    Task<List<LessonDto>> GetLessonsByCourseIdAsync(int courseId);
    Task<LessonDto?> GetLessonByIdAsync(int id);
    Task<LessonDto> CreateLessonAsync(CreateLessonDto dto, string userId);
    Task<LessonDto> UpdateLessonAsync(int id, UpdateLessonDto dto, string userId);
    Task DeleteLessonAsync(int id, string userId);
}

