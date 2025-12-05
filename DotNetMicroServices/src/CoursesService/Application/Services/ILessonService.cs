using CoursesService.Models;
using Shared.Core.Common;

namespace CoursesService.Services;

public interface ILessonService
{
    Task<ApiResponse<List<Lesson>>> GetLessonsByCourseAsync(string courseId, int page = 1, int pageSize = 10);
    Task<ApiResponse<Lesson>> GetLessonByIdAsync(string id);
    Task<ApiResponse<Lesson>> CreateLessonAsync(Lesson lesson);
    Task<ApiResponse<Lesson>> UpdateLessonAsync(string id, Lesson lesson);
    Task<ApiResponse<bool>> DeleteLessonAsync(string id);
}





