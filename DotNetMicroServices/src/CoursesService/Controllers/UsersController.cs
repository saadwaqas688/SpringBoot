using Microsoft.AspNetCore.Mvc;
using CoursesService.Models;
using CoursesService.Repositories;
using Shared.Common;

namespace CoursesService.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly IUserCourseRepository _userCourseRepository;
    private readonly IUserLessonProgressRepository _lessonProgressRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<UsersController> _logger;

    public UsersController(
        IUserCourseRepository userCourseRepository,
        IUserLessonProgressRepository lessonProgressRepository,
        IUserRepository userRepository,
        ILogger<UsersController> logger)
    {
        _userCourseRepository = userCourseRepository;
        _lessonProgressRepository = lessonProgressRepository;
        _userRepository = userRepository;
        _logger = logger;
    }

    [HttpGet("{userId}/courses")]
    public async Task<ActionResult<ApiResponse<List<UserCourse>>>> GetUserCourses(string userId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        try
        {
            var courses = await _userCourseRepository.GetByUserIdAsync(userId);
            var paged = courses.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            return Ok(ApiResponse<List<UserCourse>>.SuccessResponse(paged, "User courses retrieved successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving courses for user {UserId}", userId);
            return StatusCode(500, ApiResponse<List<UserCourse>>.ErrorResponse("An error occurred while retrieving courses"));
        }
    }

    [HttpGet("{userId}/progress")]
    public async Task<ActionResult<ApiResponse<UserProgressSummaryDto>>> GetUserProgress(string userId)
    {
        try
        {
            var courses = await _userCourseRepository.GetByUserIdAsync(userId);
            var lessons = await _lessonProgressRepository.GetByUserIdAsync(userId);

            var summary = new UserProgressSummaryDto
            {
                TotalCourses = courses.Count(),
                CompletedCourses = courses.Count(c => c.Status == "completed"),
                InProgressCourses = courses.Count(c => c.Status == "in_progress"),
                NotStartedCourses = courses.Count(c => c.Status == "not_started"),
                TotalLessons = lessons.Count(),
                CompletedLessons = lessons.Count(l => l.IsCompleted),
                AverageProgress = courses.Any() ? (int)courses.Average(c => c.Progress) : 0
            };

            return Ok(ApiResponse<UserProgressSummaryDto>.SuccessResponse(summary, "Progress summary retrieved successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving progress for user {UserId}", userId);
            return StatusCode(500, ApiResponse<UserProgressSummaryDto>.ErrorResponse("An error occurred while retrieving progress"));
        }
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResponse<User>>>> GetAllUsers([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? search = null)
    {
        try
        {
            var users = await _userRepository.GetAllNonAdminUsersAsync(page, pageSize, search);
            return Ok(ApiResponse<PagedResponse<User>>.SuccessResponse(users, "Users retrieved successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving users");
            return StatusCode(500, ApiResponse<PagedResponse<User>>.ErrorResponse("An error occurred while retrieving users"));
        }
    }
}

public class UserProgressSummaryDto
{
    public int TotalCourses { get; set; }
    public int CompletedCourses { get; set; }
    public int InProgressCourses { get; set; }
    public int NotStartedCourses { get; set; }
    public int TotalLessons { get; set; }
    public int CompletedLessons { get; set; }
    public int AverageProgress { get; set; }
}


