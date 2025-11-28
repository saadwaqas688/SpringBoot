using Microsoft.AspNetCore.Mvc;
using CoursesService.Models;
using CoursesService.Repositories;
using Shared.Common;

namespace CoursesService.Controllers;

[ApiController]
[Route("api/admin")]
public class AdminController : ControllerBase
{
    private readonly ICourseRepository _courseRepository;
    private readonly IUserCourseRepository _userCourseRepository;
    private readonly IUserActivityLogRepository _activityLogRepository;
    private readonly IUserQuizAttemptRepository _quizAttemptRepository;
    private readonly ILogger<AdminController> _logger;

    public AdminController(
        ICourseRepository courseRepository,
        IUserCourseRepository userCourseRepository,
        IUserActivityLogRepository activityLogRepository,
        IUserQuizAttemptRepository quizAttemptRepository,
        ILogger<AdminController> logger)
    {
        _courseRepository = courseRepository;
        _userCourseRepository = userCourseRepository;
        _activityLogRepository = activityLogRepository;
        _quizAttemptRepository = quizAttemptRepository;
        _logger = logger;
    }

    [HttpGet("analytics/courses")]
    public async Task<ActionResult<ApiResponse<CourseAnalyticsDto>>> GetCourseAnalytics()
    {
        try
        {
            var courses = await _courseRepository.GetAllAsync();
            var userCourses = await _userCourseRepository.GetAllAsync();

            var analytics = new CourseAnalyticsDto
            {
                TotalCourses = courses.Count(),
                PublishedCourses = courses.Count(c => c.Status == "published"),
                DraftCourses = courses.Count(c => c.Status == "draft"),
                TotalEnrollments = userCourses.Count(),
                AverageEnrollmentsPerCourse = courses.Any() ? (int)Math.Round(userCourses.Count() / (double)courses.Count()) : 0,
                CompletionRate = userCourses.Any() ? (int)Math.Round((userCourses.Count(uc => uc.Status == "completed") / (double)userCourses.Count()) * 100) : 0
            };

            return Ok(ApiResponse<CourseAnalyticsDto>.SuccessResponse(analytics, "Course analytics retrieved successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving course analytics");
            return StatusCode(500, ApiResponse<CourseAnalyticsDto>.ErrorResponse("An error occurred while retrieving analytics"));
        }
    }

    [HttpGet("analytics/users")]
    public async Task<ActionResult<ApiResponse<UserAnalyticsDto>>> GetUserAnalytics()
    {
        try
        {
            var userCourses = await _userCourseRepository.GetAllAsync();
            var uniqueUsers = userCourses.Select(uc => uc.UserId).Distinct();

            var analytics = new UserAnalyticsDto
            {
                TotalUsers = uniqueUsers.Count(),
                ActiveUsers = uniqueUsers.Count(), // Users with at least one course
                UsersWithCompletedCourses = uniqueUsers.Count(u => userCourses.Any(uc => uc.UserId == u && uc.Status == "completed")),
                AverageCoursesPerUser = uniqueUsers.Any() ? (int)Math.Round(userCourses.Count() / (double)uniqueUsers.Count()) : 0
            };

            return Ok(ApiResponse<UserAnalyticsDto>.SuccessResponse(analytics, "User analytics retrieved successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user analytics");
            return StatusCode(500, ApiResponse<UserAnalyticsDto>.ErrorResponse("An error occurred while retrieving analytics"));
        }
    }

    [HttpGet("analytics/engagement")]
    public async Task<ActionResult<ApiResponse<EngagementAnalyticsDto>>> GetEngagementAnalytics()
    {
        try
        {
            var activities = await _activityLogRepository.GetAllAsync();
            var userCourses = await _userCourseRepository.GetAllAsync();

            var analytics = new EngagementAnalyticsDto
            {
                TotalActivities = activities.Count(),
                ActivitiesLast7Days = activities.Count(a => a.Timestamp >= DateTime.UtcNow.AddDays(-7)),
                ActivitiesLast30Days = activities.Count(a => a.Timestamp >= DateTime.UtcNow.AddDays(-30)),
                ActiveEnrollments = userCourses.Count(uc => uc.Status == "in_progress"),
                AverageSessionDuration = 0 // Would need to calculate from activity logs
            };

            return Ok(ApiResponse<EngagementAnalyticsDto>.SuccessResponse(analytics, "Engagement analytics retrieved successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving engagement analytics");
            return StatusCode(500, ApiResponse<EngagementAnalyticsDto>.ErrorResponse("An error occurred while retrieving analytics"));
        }
    }

    [HttpGet("analytics/performance")]
    public async Task<ActionResult<ApiResponse<PerformanceAnalyticsDto>>> GetPerformanceAnalytics()
    {
        try
        {
            var attempts = await _quizAttemptRepository.GetAllAsync();

            var analytics = new PerformanceAnalyticsDto
            {
                TotalQuizAttempts = attempts.Count(),
                AverageScore = attempts.Any() ? (int)Math.Round(attempts.Average(a => a.Score)) : 0,
                PassRate = attempts.Any() ? (int)Math.Round((attempts.Count(a => a.Score >= 70) / (double)attempts.Count()) * 100) : 0,
                TotalQuestionsAnswered = attempts.Sum(a => a.TotalQuestions),
                CorrectAnswers = attempts.Sum(a => a.CorrectAnswers)
            };

            return Ok(ApiResponse<PerformanceAnalyticsDto>.SuccessResponse(analytics, "Performance analytics retrieved successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving performance analytics");
            return StatusCode(500, ApiResponse<PerformanceAnalyticsDto>.ErrorResponse("An error occurred while retrieving analytics"));
        }
    }
}

// DTOs
public class CourseAnalyticsDto
{
    public int TotalCourses { get; set; }
    public int PublishedCourses { get; set; }
    public int DraftCourses { get; set; }
    public int TotalEnrollments { get; set; }
    public int AverageEnrollmentsPerCourse { get; set; }
    public int CompletionRate { get; set; }
}

public class UserAnalyticsDto
{
    public int TotalUsers { get; set; }
    public int ActiveUsers { get; set; }
    public int UsersWithCompletedCourses { get; set; }
    public int AverageCoursesPerUser { get; set; }
}

public class EngagementAnalyticsDto
{
    public int TotalActivities { get; set; }
    public int ActivitiesLast7Days { get; set; }
    public int ActivitiesLast30Days { get; set; }
    public int ActiveEnrollments { get; set; }
    public int AverageSessionDuration { get; set; }
}

public class PerformanceAnalyticsDto
{
    public int TotalQuizAttempts { get; set; }
    public int AverageScore { get; set; }
    public int PassRate { get; set; }
    public int TotalQuestionsAnswered { get; set; }
    public int CorrectAnswers { get; set; }
}


