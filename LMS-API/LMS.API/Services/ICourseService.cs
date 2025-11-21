using LMS.API.DTOs;

namespace LMS.API.Services;

public interface ICourseService
{
    Task<List<CourseDto>> GetAllCoursesAsync();
    Task<CourseDto?> GetCourseByIdAsync(string id);
    Task<CourseDto> CreateCourseAsync(CreateCourseDto dto, string userId);
    Task<CourseDto> UpdateCourseAsync(string id, UpdateCourseDto dto, string userId);
    Task DeleteCourseAsync(string id, string userId);
    Task<List<CourseDto>> GetUserCoursesAsync(string userId);
}

