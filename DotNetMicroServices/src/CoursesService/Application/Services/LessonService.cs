using CoursesService.Models;
using CoursesService.Repositories;
using Shared.Core.Common;

namespace CoursesService.Services;

public class LessonService : ILessonService
{
    private readonly ILessonRepository _lessonRepository;
    private readonly IDiscussionRepository _discussionRepository;
    private readonly ILogger<LessonService> _logger;

    public LessonService(
        ILessonRepository lessonRepository,
        IDiscussionRepository discussionRepository,
        ILogger<LessonService> logger)
    {
        _lessonRepository = lessonRepository;
        _discussionRepository = discussionRepository;
        _logger = logger;
    }

    public async Task<ApiResponse<List<Lesson>>> GetLessonsByCourseAsync(string courseId, int page = 1, int pageSize = 10)
    {
        try
        {
            var lessons = await _lessonRepository.GetByCourseIdAsync(courseId);
            var pagedLessons = lessons.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            return ApiResponse<List<Lesson>>.SuccessResponse(pagedLessons, "Lessons retrieved successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving lessons for course {CourseId}", courseId);
            return ApiResponse<List<Lesson>>.ErrorResponse("An error occurred while retrieving lessons");
        }
    }

    public async Task<ApiResponse<Lesson>> GetLessonByIdAsync(string id)
    {
        try
        {
            var lesson = await _lessonRepository.GetByIdAsync(id);
            if (lesson == null)
            {
                return ApiResponse<Lesson>.ErrorResponse("Lesson not found");
            }
            return ApiResponse<Lesson>.SuccessResponse(lesson, "Lesson retrieved successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving lesson {LessonId}", id);
            return ApiResponse<Lesson>.ErrorResponse("An error occurred while retrieving lesson");
        }
    }

    public async Task<ApiResponse<Lesson>> CreateLessonAsync(Lesson lesson)
    {
        try
        {
            lesson.CreatedAt = DateTime.UtcNow;
            lesson.UpdatedAt = DateTime.UtcNow;
            var created = await _lessonRepository.CreateAsync(lesson);

            // Auto-create Discussion if lesson type is "discussion"
            if (created.Id != null && 
                created.LessonType?.ToLower() == "discussion")
            {
                try
                {
                    var discussion = new Discussion
                    {
                        LessonId = created.Id,
                        Description = string.Empty,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };
                    await _discussionRepository.CreateAsync(discussion);
                    _logger.LogInformation("Auto-created discussion for lesson {LessonId}", created.Id);
                }
                catch (Exception discussionEx)
                {
                    _logger.LogError(discussionEx, "Error auto-creating discussion for lesson {LessonId}", created.Id);
                    // Don't fail the lesson creation if discussion creation fails
                    // The discussion can be created later
                }
            }

            return ApiResponse<Lesson>.SuccessResponse(created, "Lesson created successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating lesson");
            return ApiResponse<Lesson>.ErrorResponse("An error occurred while creating lesson");
        }
    }

    public async Task<ApiResponse<Lesson>> UpdateLessonAsync(string id, Lesson lesson)
    {
        try
        {
            lesson.Id = id;
            lesson.UpdatedAt = DateTime.UtcNow;
            var updated = await _lessonRepository.UpdateAsync(id, lesson);
            if (updated == null)
            {
                return ApiResponse<Lesson>.ErrorResponse("Lesson not found");
            }
            return ApiResponse<Lesson>.SuccessResponse(updated, "Lesson updated successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating lesson {LessonId}", id);
            return ApiResponse<Lesson>.ErrorResponse("An error occurred while updating lesson");
        }
    }

    public async Task<ApiResponse<bool>> DeleteLessonAsync(string id)
    {
        try
        {
            var deleted = await _lessonRepository.DeleteAsync(id);
            if (!deleted)
            {
                return ApiResponse<bool>.ErrorResponse("Lesson not found");
            }
            return ApiResponse<bool>.SuccessResponse(true, "Lesson deleted successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting lesson {LessonId}", id);
            return ApiResponse<bool>.ErrorResponse("An error occurred while deleting lesson");
        }
    }
}





