using CoursesService.Models;
using Shared.Core.Common;

namespace CoursesService.Services;

public interface ICourseService
{
    Task<ApiResponse<Course>> GetByIdAsync(string id);
    Task<ApiResponse<PagedResponse<Course>>> GetAllAsync(int page, int pageSize);
    Task<ApiResponse<Course>> CreateAsync(Course course);
    Task<ApiResponse<Course>> UpdateAsync(string id, Course course);
    Task<ApiResponse<bool>> DeleteAsync(string id);
    Task<ApiResponse<List<Course>>> GetByStatusAsync(string status);
    Task<ApiResponse<List<Course>>> SearchAsync(string searchTerm);
}


