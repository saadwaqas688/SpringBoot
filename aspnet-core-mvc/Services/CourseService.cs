using CourseManagementAPI.Models;
using CourseManagementAPI.Repositories;

namespace CourseManagementAPI.Services;

public class CourseService : ICourseService
{
    private readonly ICourseRepository _courseRepository;

    public CourseService(ICourseRepository courseRepository)
    {
        _courseRepository = courseRepository;
    }

    public async Task<List<Course>> GetAllCoursesAsync()
    {
        return await _courseRepository.GetAllAsync();
    }

    public async Task<Course?> GetCourseByIdAsync(string id)
    {
        return await _courseRepository.GetByIdAsync(id);
    }

    public async Task<Course> CreateCourseAsync(Course course)
    {
        course.CreatedAt = DateTime.UtcNow;
        course.UpdatedAt = DateTime.UtcNow;
        return await _courseRepository.CreateAsync(course);
    }

    public async Task<Course> UpdateCourseAsync(string id, Course courseDetails)
    {
        var course = await _courseRepository.GetByIdAsync(id);
        if (course == null)
        {
            throw new Exception("Course not found with id: " + id);
        }

        course.Name = courseDetails.Name;
        course.Description = courseDetails.Description;
        course.UpdatedAt = DateTime.UtcNow;
        return await _courseRepository.UpdateAsync(course);
    }

    public async Task DeleteCourseAsync(string id)
    {
        var course = await _courseRepository.GetByIdAsync(id);
        if (course == null)
        {
            throw new Exception("Course not found with id: " + id);
        }

        await _courseRepository.DeleteAsync(id);
    }
}



