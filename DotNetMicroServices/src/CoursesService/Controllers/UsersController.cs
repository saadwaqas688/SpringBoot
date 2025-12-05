using Microsoft.AspNetCore.Mvc;
using CoursesService.Models;
using CoursesService.Repositories;
using Shared.Core.Common;

namespace CoursesService.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly IUserCourseRepository _userCourseRepository;
    private readonly IUserLessonProgressRepository _lessonProgressRepository;
    private readonly IUserRepository _userRepository;
    private readonly ICourseRepository _courseRepository;
    private readonly ILogger<UsersController> _logger;

    public UsersController(
        IUserCourseRepository userCourseRepository,
        IUserLessonProgressRepository lessonProgressRepository,
        IUserRepository userRepository,
        ICourseRepository courseRepository,
        ILogger<UsersController> logger)
    {
        _userCourseRepository = userCourseRepository;
        _lessonProgressRepository = lessonProgressRepository;
        _userRepository = userRepository;
        _courseRepository = courseRepository;
        _logger = logger;
    }

    [HttpGet("{userId}/courses")]
    public async Task<ActionResult<ApiResponse<List<object>>>> GetUserCourses(string userId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        try
        {
            var userCourses = await _userCourseRepository.GetByUserIdAsync(userId);
            var courseIds = userCourses.Select(uc => uc.CourseId).ToList();
            
            // Get full course details for each courseId
            var courses = new List<object>();
            
            foreach (var courseId in courseIds)
            {
                var course = await _courseRepository.GetByIdAsync(courseId);
                if (course != null)
                {
                    // Combine course data with user course progress data
                    var userCourse = userCourses.FirstOrDefault(uc => uc.CourseId == courseId);
                    courses.Add(new
                    {
                        Id = course.Id,
                        Title = course.Title,
                        Description = course.Description,
                        Thumbnail = course.Thumbnail,
                        Status = course.Status,
                        CreatedAt = course.CreatedAt,
                        UpdatedAt = course.UpdatedAt,
                        Progress = userCourse?.Progress ?? 0,
                        EnrollmentStatus = userCourse?.Status ?? "not_started",
                        AssignedAt = userCourse?.AssignedAt,
                        CompletedAt = userCourse?.CompletedAt
                    });
                }
            }
            
            var paged = courses.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            return Ok(ApiResponse<List<object>>.SuccessResponse(paged, "User courses retrieved successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving courses for user {UserId}", userId);
            return StatusCode(500, ApiResponse<List<object>>.ErrorResponse("An error occurred while retrieving courses"));
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



