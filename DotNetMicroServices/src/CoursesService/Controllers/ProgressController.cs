using Microsoft.AspNetCore.Mvc;
using CoursesService.Models;
using CoursesService.Repositories;
using Shared.Core.Common;

namespace CoursesService.Controllers;

[ApiController]
[Route("api")]
public class ProgressController : ControllerBase
{
    private readonly IUserCourseRepository _userCourseRepository;
    private readonly IUserLessonProgressRepository _lessonProgressRepository;
    private readonly IUserSlideProgressRepository _slideProgressRepository;
    private readonly IUserActivityLogRepository _activityLogRepository;
    private readonly ILogger<ProgressController> _logger;

    public ProgressController(
        IUserCourseRepository userCourseRepository,
        IUserLessonProgressRepository lessonProgressRepository,
        IUserSlideProgressRepository slideProgressRepository,
        IUserActivityLogRepository activityLogRepository,
        ILogger<ProgressController> logger)
    {
        _userCourseRepository = userCourseRepository;
        _lessonProgressRepository = lessonProgressRepository;
        _slideProgressRepository = slideProgressRepository;
        _activityLogRepository = activityLogRepository;
        _logger = logger;
    }

    [HttpGet("users/{userId}/courses/{courseId}/progress")]
    public async Task<ActionResult<ApiResponse<UserCourse>>> GetCourseProgress(string userId, string courseId)
    {
        try
        {
            var progress = await _userCourseRepository.GetByUserAndCourseAsync(userId, courseId);
            if (progress == null)
            {
                return NotFound(ApiResponse<UserCourse>.ErrorResponse("Progress not found"));
            }
            return Ok(ApiResponse<UserCourse>.SuccessResponse(progress, "Progress retrieved successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving course progress");
            return StatusCode(500, ApiResponse<UserCourse>.ErrorResponse("An error occurred while retrieving progress"));
        }
    }

    [HttpPost("progress/lesson/start")]
    public async Task<ActionResult<ApiResponse<UserLessonProgress>>> StartLesson([FromBody] StartLessonDto dto)
    {
        try
        {
            var progress = await _lessonProgressRepository.GetByUserAndLessonAsync(dto.UserId, dto.LessonId);
            if (progress == null)
            {
                progress = new UserLessonProgress
                {
                    UserId = dto.UserId,
                    LessonId = dto.LessonId,
                    StartedAt = DateTime.UtcNow
                };
                progress = await _lessonProgressRepository.CreateAsync(progress);
            }
            else if (progress.StartedAt == null)
            {
                progress.StartedAt = DateTime.UtcNow;
                progress = await _lessonProgressRepository.UpdateAsync(progress.Id!, progress);
            }

            return Ok(ApiResponse<UserLessonProgress>.SuccessResponse(progress, "Lesson started successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error starting lesson");
            return StatusCode(500, ApiResponse<UserLessonProgress>.ErrorResponse("An error occurred while starting lesson"));
        }
    }

    [HttpPost("progress/lesson/complete")]
    public async Task<ActionResult<ApiResponse<UserLessonProgress>>> CompleteLesson([FromBody] CompleteLessonDto dto)
    {
        try
        {
            var progress = await _lessonProgressRepository.GetByUserAndLessonAsync(dto.UserId, dto.LessonId);
            if (progress == null)
            {
                progress = new UserLessonProgress
                {
                    UserId = dto.UserId,
                    LessonId = dto.LessonId,
                    StartedAt = DateTime.UtcNow,
                    IsCompleted = true,
                    CompletedAt = DateTime.UtcNow
                };
                progress = await _lessonProgressRepository.CreateAsync(progress);
            }
            else
            {
                progress.IsCompleted = true;
                progress.CompletedAt = DateTime.UtcNow;
                progress = await _lessonProgressRepository.UpdateAsync(progress.Id!, progress);
            }

            return Ok(ApiResponse<UserLessonProgress>.SuccessResponse(progress, "Lesson completed successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error completing lesson");
            return StatusCode(500, ApiResponse<UserLessonProgress>.ErrorResponse("An error occurred while completing lesson"));
        }
    }

    [HttpPost("progress/slide/view")]
    public async Task<ActionResult<ApiResponse<UserSlideProgress>>> ViewSlide([FromBody] ViewSlideDto dto)
    {
        try
        {
            var progress = await _slideProgressRepository.GetByUserAndSlideAsync(dto.UserId, dto.SlideId);
            if (progress == null)
            {
                progress = new UserSlideProgress
                {
                    UserId = dto.UserId,
                    SlideId = dto.SlideId,
                    IsViewed = true,
                    ViewedAt = DateTime.UtcNow
                };
                progress = await _slideProgressRepository.CreateAsync(progress);
            }
            else if (!progress.IsViewed)
            {
                progress.IsViewed = true;
                progress.ViewedAt = DateTime.UtcNow;
                progress = await _slideProgressRepository.UpdateAsync(progress.Id!, progress);
            }

            return Ok(ApiResponse<UserSlideProgress>.SuccessResponse(progress, "Slide viewed successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error viewing slide");
            return StatusCode(500, ApiResponse<UserSlideProgress>.ErrorResponse("An error occurred while viewing slide"));
        }
    }

    [HttpPost("progress/slide/complete")]
    public async Task<ActionResult<ApiResponse<UserSlideProgress>>> CompleteSlide([FromBody] CompleteSlideDto dto)
    {
        try
        {
            var progress = await _slideProgressRepository.GetByUserAndSlideAsync(dto.UserId, dto.SlideId);
            if (progress == null)
            {
                progress = new UserSlideProgress
                {
                    UserId = dto.UserId,
                    SlideId = dto.SlideId,
                    IsViewed = true,
                    IsCompleted = true,
                    ViewedAt = DateTime.UtcNow,
                    CompletedAt = DateTime.UtcNow,
                    TimeSpent = dto.TimeSpent
                };
                progress = await _slideProgressRepository.CreateAsync(progress);
            }
            else
            {
                progress.IsViewed = true;
                progress.IsCompleted = true;
                progress.CompletedAt = DateTime.UtcNow;
                progress.TimeSpent = dto.TimeSpent;
                if (progress.ViewedAt == null) progress.ViewedAt = DateTime.UtcNow;
                progress = await _slideProgressRepository.UpdateAsync(progress.Id!, progress);
            }

            return Ok(ApiResponse<UserSlideProgress>.SuccessResponse(progress, "Slide completed successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error completing slide");
            return StatusCode(500, ApiResponse<UserSlideProgress>.ErrorResponse("An error occurred while completing slide"));
        }
    }

    [HttpPost("activity/log")]
    public async Task<ActionResult<ApiResponse<UserActivityLog>>> LogActivity([FromBody] UserActivityLog activity)
    {
        try
        {
            activity.Timestamp = DateTime.UtcNow;
            var created = await _activityLogRepository.CreateAsync(activity);
            return Ok(ApiResponse<UserActivityLog>.SuccessResponse(created, "Activity logged successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error logging activity");
            return StatusCode(500, ApiResponse<UserActivityLog>.ErrorResponse("An error occurred while logging activity"));
        }
    }

    [HttpGet("activity/user/{userId}")]
    public async Task<ActionResult<ApiResponse<List<UserActivityLog>>>> GetUserActivities(string userId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        try
        {
            var activities = await _activityLogRepository.GetByUserIdAsync(userId);
            var paged = activities.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            return Ok(ApiResponse<List<UserActivityLog>>.SuccessResponse(paged, "Activities retrieved successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user activities");
            return StatusCode(500, ApiResponse<List<UserActivityLog>>.ErrorResponse("An error occurred while retrieving activities"));
        }
    }

    [HttpGet("activity/course/{courseId}")]
    public async Task<ActionResult<ApiResponse<List<UserActivityLog>>>> GetCourseActivities(string courseId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        try
        {
            var activities = await _activityLogRepository.GetByCourseIdAsync(courseId);
            var paged = activities.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            return Ok(ApiResponse<List<UserActivityLog>>.SuccessResponse(paged, "Activities retrieved successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving course activities");
            return StatusCode(500, ApiResponse<List<UserActivityLog>>.ErrorResponse("An error occurred while retrieving activities"));
        }
    }
}

// DTOs
public class StartLessonDto
{
    public string UserId { get; set; } = string.Empty;
    public string LessonId { get; set; } = string.Empty;
}

public class CompleteLessonDto
{
    public string UserId { get; set; } = string.Empty;
    public string LessonId { get; set; } = string.Empty;
}

public class ViewSlideDto
{
    public string UserId { get; set; } = string.Empty;
    public string SlideId { get; set; } = string.Empty;
}

public class CompleteSlideDto
{
    public string UserId { get; set; } = string.Empty;
    public string SlideId { get; set; } = string.Empty;
    public int TimeSpent { get; set; }
}



