using CoursesService.Models;
using CoursesService.Repositories;
using Shared.Common;
using MongoDB.Driver;

namespace CoursesService.Services;

public class CourseService : ICourseService
{
    private readonly ICourseRepository _courseRepository;
    private readonly ILogger<CourseService> _logger;

    public CourseService(ICourseRepository courseRepository, ILogger<CourseService> logger)
    {
        _courseRepository = courseRepository;
        _logger = logger;
    }

    public async Task<ApiResponse<Course>> GetByIdAsync(string id)
    {
        try
        {
            var course = await _courseRepository.GetByIdAsync(id);
            if (course == null)
            {
                return ApiResponse<Course>.ErrorResponse("Course not found");
            }
            return ApiResponse<Course>.SuccessResponse(course, "Course retrieved successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting course by id: {Id}", id);
            return ApiResponse<Course>.ErrorResponse($"Error retrieving course: {ex.Message}");
        }
    }

    public async Task<ApiResponse<PagedResponse<Course>>> GetAllAsync(int page, int pageSize)
    {
        try
        {
            var sort = Builders<Course>.Sort.Descending(c => c.CreatedAt);
            var pagedResult = await _courseRepository.GetPagedAsync(page, pageSize, null, sort);
            return ApiResponse<PagedResponse<Course>>.SuccessResponse(pagedResult, "Courses retrieved successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all courses");
            return ApiResponse<PagedResponse<Course>>.ErrorResponse($"Error retrieving courses: {ex.Message}");
        }
    }

    public async Task<ApiResponse<Course>> CreateAsync(Course course)
    {
        try
        {
            course.CreatedAt = DateTime.UtcNow;
            course.UpdatedAt = DateTime.UtcNow;
            var created = await _courseRepository.CreateAsync(course);
            return ApiResponse<Course>.SuccessResponse(created, "Course created successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating course");
            return ApiResponse<Course>.ErrorResponse($"Error creating course: {ex.Message}");
        }
    }

    public async Task<ApiResponse<Course>> UpdateAsync(string id, Course course)
    {
        try
        {
            var existing = await _courseRepository.GetByIdAsync(id);
            if (existing == null)
            {
                return ApiResponse<Course>.ErrorResponse("Course not found");
            }

            course.Id = id;
            course.UpdatedAt = DateTime.UtcNow;
            course.CreatedAt = existing.CreatedAt; // Preserve original creation date

            var updated = await _courseRepository.UpdateAsync(id, course);
            if (updated == null)
            {
                return ApiResponse<Course>.ErrorResponse("Failed to update course");
            }

            return ApiResponse<Course>.SuccessResponse(updated, "Course updated successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating course: {Id}", id);
            return ApiResponse<Course>.ErrorResponse($"Error updating course: {ex.Message}");
        }
    }

    public async Task<ApiResponse<bool>> DeleteAsync(string id)
    {
        try
        {
            var result = await _courseRepository.DeleteAsync(id);
            if (!result)
            {
                return ApiResponse<bool>.ErrorResponse("Course not found");
            }
            return ApiResponse<bool>.SuccessResponse(true, "Course deleted successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting course: {Id}", id);
            return ApiResponse<bool>.ErrorResponse($"Error deleting course: {ex.Message}");
        }
    }

    public async Task<ApiResponse<List<Course>>> GetByStatusAsync(string status)
    {
        try
        {
            var courses = await _courseRepository.GetByStatusAsync(status);
            return ApiResponse<List<Course>>.SuccessResponse(courses.ToList(), "Courses retrieved successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting courses by status: {Status}", status);
            return ApiResponse<List<Course>>.ErrorResponse($"Error retrieving courses: {ex.Message}");
        }
    }

    public async Task<ApiResponse<List<Course>>> SearchAsync(string searchTerm)
    {
        try
        {
            var courses = await _courseRepository.SearchAsync(searchTerm);
            return ApiResponse<List<Course>>.SuccessResponse(courses.ToList(), "Courses retrieved successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching courses: {SearchTerm}", searchTerm);
            return ApiResponse<List<Course>>.ErrorResponse($"Error searching courses: {ex.Message}");
        }
    }
}

