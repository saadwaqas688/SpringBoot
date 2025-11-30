using Microsoft.AspNetCore.Mvc;
using CoursesService.Models;
using CoursesService.Services;
using CoursesService.Repositories;
using Shared.Common;
using System.Linq;

namespace CoursesService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CoursesController : ControllerBase
{
    private readonly ICourseService _courseService;
    private readonly IUserCourseRepository _userCourseRepository;
    private readonly ILogger<CoursesController> _logger;

    public CoursesController(ICourseService courseService, IUserCourseRepository userCourseRepository, ILogger<CoursesController> logger)
    {
        _courseService = courseService;
        _userCourseRepository = userCourseRepository;
        _logger = logger;
    }

    [HttpGet("health")]
    public ActionResult<ApiResponse<string>> HealthCheck()
    {
        return Ok(ApiResponse<string>.SuccessResponse("CoursesService is healthy", "Health check successful"));
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResponse<Course>>>> GetAllCourses(
        [FromQuery] int page = 1, 
        [FromQuery] int pageSize = 10)
    {
        var response = await _courseService.GetAllAsync(page, pageSize);
        return StatusCode(response.Success ? 200 : 500, response);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<Course>>> GetCourseById(string id)
    {
        var response = await _courseService.GetByIdAsync(id);
        return StatusCode(response.Success ? 200 : 404, response);
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<Course>>> CreateCourse([FromBody] Course course)
    {
        var response = await _courseService.CreateAsync(course);
        return StatusCode(response.Success ? 201 : 400, response);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<Course>>> UpdateCourse(string id, [FromBody] Course course)
    {
        var response = await _courseService.UpdateAsync(id, course);
        return StatusCode(response.Success ? 200 : 404, response);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteCourse(string id)
    {
        var response = await _courseService.DeleteAsync(id);
        return StatusCode(response.Success ? 200 : 404, response);
    }

    [HttpGet("status/{status}")]
    public async Task<ActionResult<ApiResponse<List<Course>>>> GetCoursesByStatus(string status)
    {
        var response = await _courseService.GetByStatusAsync(status);
        return StatusCode(response.Success ? 200 : 500, response);
    }

    [HttpGet("search")]
    public async Task<ActionResult<ApiResponse<List<Course>>>> SearchCourses([FromQuery] string q)
    {
        if (string.IsNullOrWhiteSpace(q))
        {
            return BadRequest(ApiResponse<List<Course>>.ErrorResponse("Search term is required"));
        }
        var response = await _courseService.SearchAsync(q);
        return StatusCode(response.Success ? 200 : 500, response);
    }

    [HttpPost("{id}/assign-user")]
    public async Task<ActionResult<ApiResponse<AssignUsersResponse>>> AssignUser(string id, [FromBody] AssignUserDto dto)
    {
        try
        {
            if (dto.UserIds == null || dto.UserIds.Length == 0)
            {
                return BadRequest(ApiResponse<AssignUsersResponse>.ErrorResponse("At least one user ID is required"));
            }

            var assignedUsers = new List<UserCourse>();
            var alreadyAssigned = new List<string>();
            var failedUsers = new List<string>();

            foreach (var userId in dto.UserIds.Distinct())
            {
                if (string.IsNullOrWhiteSpace(userId))
                {
                    failedUsers.Add(userId ?? "null");
                    continue;
                }

                try
                {
                    var existing = await _userCourseRepository.GetByUserAndCourseAsync(userId, id);
                    if (existing != null)
                    {
                        alreadyAssigned.Add(userId);
                        continue;
                    }

                    var userCourse = new UserCourse
                    {
                        UserId = userId,
                        CourseId = id,
                        Status = "not_started",
                        Progress = 0,
                        AssignedAt = DateTime.UtcNow
                    };

                    var created = await _userCourseRepository.CreateAsync(userCourse);
                    assignedUsers.Add(created);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Error assigning user {UserId} to course {CourseId}", userId, id);
                    failedUsers.Add(userId);
                }
            }

            var response = new AssignUsersResponse
            {
                AssignedCount = assignedUsers.Count,
                AlreadyAssignedCount = alreadyAssigned.Count,
                FailedCount = failedUsers.Count,
                AssignedUsers = assignedUsers,
                AlreadyAssignedUserIds = alreadyAssigned,
                FailedUserIds = failedUsers
            };

            var message = $"Assigned {assignedUsers.Count} user(s) to course";
            if (alreadyAssigned.Count > 0)
            {
                message += $", {alreadyAssigned.Count} user(s) were already assigned";
            }
            if (failedUsers.Count > 0)
            {
                message += $", {failedUsers.Count} user(s) failed to assign";
            }

            return Ok(ApiResponse<AssignUsersResponse>.SuccessResponse(response, message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error assigning users to course");
            return StatusCode(500, ApiResponse<AssignUsersResponse>.ErrorResponse("An error occurred while assigning users"));
        }
    }

    [HttpDelete("{id}/remove-user/{userId}")]
    public async Task<ActionResult<ApiResponse<bool>>> RemoveUser(string id, string userId)
    {
        try
        {
            var userCourse = await _userCourseRepository.GetByUserAndCourseAsync(userId, id);
            if (userCourse == null)
            {
                return NotFound(ApiResponse<bool>.ErrorResponse("User is not assigned to this course"));
            }

            var deleted = await _userCourseRepository.DeleteAsync(userCourse.Id!);
            return Ok(ApiResponse<bool>.SuccessResponse(deleted, "User removed from course successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing user from course");
            return StatusCode(500, ApiResponse<bool>.ErrorResponse("An error occurred while removing user"));
        }
    }

    [HttpGet("{id}/assigned-users")]
    public async Task<ActionResult<ApiResponse<List<UserCourse>>>> GetAssignedUsers(string id, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        try
        {
            var users = await _userCourseRepository.GetByCourseIdAsync(id);
            var paged = users.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            return Ok(ApiResponse<List<UserCourse>>.SuccessResponse(paged, "Assigned users retrieved successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving assigned users");
            return StatusCode(500, ApiResponse<List<UserCourse>>.ErrorResponse("An error occurred while retrieving assigned users"));
        }
    }

    [HttpGet("{id}/user-progress/{userId}")]
    public async Task<ActionResult<ApiResponse<CourseUserProgressDto>>> GetUserProgress(string id, string userId)
    {
        try
        {
            var userCourse = await _userCourseRepository.GetByUserAndCourseAsync(userId, id);
            if (userCourse == null)
            {
                return NotFound(ApiResponse<CourseUserProgressDto>.ErrorResponse("User is not assigned to this course"));
            }

            var progress = new CourseUserProgressDto
            {
                Progress = userCourse.Progress,
                Status = userCourse.Status,
                AssignedAt = userCourse.AssignedAt,
                CompletedAt = userCourse.CompletedAt
            };

            return Ok(ApiResponse<CourseUserProgressDto>.SuccessResponse(progress, "User progress retrieved successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user progress");
            return StatusCode(500, ApiResponse<CourseUserProgressDto>.ErrorResponse("An error occurred while retrieving user progress"));
        }
    }
}

public class AssignUserDto
{
    public string[] UserIds { get; set; } = Array.Empty<string>();
}

public class AssignUsersResponse
{
    public int AssignedCount { get; set; }
    public int AlreadyAssignedCount { get; set; }
    public int FailedCount { get; set; }
    public List<UserCourse> AssignedUsers { get; set; } = new();
    public List<string> AlreadyAssignedUserIds { get; set; } = new();
    public List<string> FailedUserIds { get; set; } = new();
}

public class CourseUserProgressDto
{
    public int Progress { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime AssignedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
}

