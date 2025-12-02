using Microsoft.AspNetCore.Mvc;
using CoursesService.Models;
using CoursesService.Repositories;
using Shared.Common;
using MongoDB.Driver;

namespace CoursesService.Controllers;

[ApiController]
[Route("api")]
public class LessonsController : ControllerBase
{
    private readonly ILessonRepository _lessonRepository;
    private readonly IDiscussionRepository _discussionRepository;
    private readonly ILogger<LessonsController> _logger;

    public LessonsController(
        ILessonRepository lessonRepository, 
        IDiscussionRepository discussionRepository,
        ILogger<LessonsController> logger)
    {
        _lessonRepository = lessonRepository;
        _discussionRepository = discussionRepository;
        _logger = logger;
    }

    [HttpGet("courses/{courseId}/lessons")]
    public async Task<ActionResult<ApiResponse<List<Lesson>>>> GetLessonsByCourse(string courseId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        try
        {
            var lessons = await _lessonRepository.GetByCourseIdAsync(courseId);
            var pagedLessons = lessons.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            return Ok(ApiResponse<List<Lesson>>.SuccessResponse(pagedLessons, "Lessons retrieved successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving lessons for course {CourseId}", courseId);
            return StatusCode(500, ApiResponse<List<Lesson>>.ErrorResponse("An error occurred while retrieving lessons"));
        }
    }

    [HttpGet("lessons/{id}")]
    public async Task<ActionResult<ApiResponse<Lesson>>> GetLessonById(string id)
    {
        try
        {
            var lesson = await _lessonRepository.GetByIdAsync(id);
            if (lesson == null)
            {
                return NotFound(ApiResponse<Lesson>.ErrorResponse("Lesson not found"));
            }
            return Ok(ApiResponse<Lesson>.SuccessResponse(lesson, "Lesson retrieved successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving lesson {LessonId}", id);
            return StatusCode(500, ApiResponse<Lesson>.ErrorResponse("An error occurred while retrieving lesson"));
        }
    }

    [HttpPost("lessons")]
    public async Task<ActionResult<ApiResponse<Lesson>>> CreateLesson([FromBody] Lesson lesson)
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

            return CreatedAtAction(nameof(GetLessonById), new { id = created.Id },
                ApiResponse<Lesson>.SuccessResponse(created, "Lesson created successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating lesson");
            return StatusCode(500, ApiResponse<Lesson>.ErrorResponse("An error occurred while creating lesson"));
        }
    }

    [HttpPut("lessons/{id}")]
    public async Task<ActionResult<ApiResponse<Lesson>>> UpdateLesson(string id, [FromBody] Lesson lesson)
    {
        try
        {
            lesson.Id = id;
            lesson.UpdatedAt = DateTime.UtcNow;
            var updated = await _lessonRepository.UpdateAsync(id, lesson);
            if (updated == null)
            {
                return NotFound(ApiResponse<Lesson>.ErrorResponse("Lesson not found"));
            }
            return Ok(ApiResponse<Lesson>.SuccessResponse(updated, "Lesson updated successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating lesson {LessonId}", id);
            return StatusCode(500, ApiResponse<Lesson>.ErrorResponse("An error occurred while updating lesson"));
        }
    }

    [HttpDelete("lessons/{id}")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteLesson(string id)
    {
        try
        {
            var deleted = await _lessonRepository.DeleteAsync(id);
            if (!deleted)
            {
                return NotFound(ApiResponse<bool>.ErrorResponse("Lesson not found"));
            }
            return Ok(ApiResponse<bool>.SuccessResponse(true, "Lesson deleted successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting lesson {LessonId}", id);
            return StatusCode(500, ApiResponse<bool>.ErrorResponse("An error occurred while deleting lesson"));
        }
    }
}


