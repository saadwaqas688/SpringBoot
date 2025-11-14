using CourseManagementAPI.Models;

namespace CourseManagementAPI.Services;

public interface ICourseService
{
    Task<List<Course>> GetAllCoursesAsync();
    Task<Course?> GetCourseByIdAsync(string id);
    Task<Course> CreateCourseAsync(Course course);
    Task<Course> UpdateCourseAsync(string id, Course courseDetails);
    Task DeleteCourseAsync(string id);
}



