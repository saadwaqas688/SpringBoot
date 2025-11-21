using LMS_MVC.DTOs;

namespace LMS_MVC.Services;

public interface ICourseService
{
    Task<List<CourseDto>> GetAllCoursesAsync();
    Task<CourseDto?> GetCourseByIdAsync(int id);
    Task<CourseDto> CreateCourseAsync(CreateCourseDto dto, string userId);
    Task<CourseDto> UpdateCourseAsync(int id, UpdateCourseDto dto, string userId);
    Task DeleteCourseAsync(int id, string userId);
    Task<List<CourseDto>> GetUserCoursesAsync(string userId);
}

