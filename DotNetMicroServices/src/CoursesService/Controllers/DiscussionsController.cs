using Microsoft.AspNetCore.Mvc;
using CoursesService.Models;
using CoursesService.Repositories;
using CoursesService.DTOs;
using Shared.Core.Common;

namespace CoursesService.Controllers;

[ApiController]
[Route("api")]
public class DiscussionsController : ControllerBase
{
    private readonly IDiscussionRepository _discussionRepository;
    private readonly IDiscussionPostRepository _postRepository;
    private readonly ILessonRepository _lessonRepository;
    private readonly ICourseRepository _courseRepository;
    private readonly IUserCourseRepository _userCourseRepository;
    private readonly ILogger<DiscussionsController> _logger;

    public DiscussionsController(
        IDiscussionRepository discussionRepository,
        IDiscussionPostRepository postRepository,
        ILessonRepository lessonRepository,
        ICourseRepository courseRepository,
        IUserCourseRepository userCourseRepository,
        ILogger<DiscussionsController> logger)
    {
        _discussionRepository = discussionRepository;
        _postRepository = postRepository;
        _lessonRepository = lessonRepository;
        _courseRepository = courseRepository;
        _userCourseRepository = userCourseRepository;
        _logger = logger;
    }

    [HttpGet("lessons/{lessonId}/discussions")]
    public async Task<ActionResult<ApiResponse<Discussion>>> GetDiscussionByLesson(string lessonId)
    {
        try
        {
            var discussion = await _discussionRepository.GetByLessonIdAsync(lessonId);
            if (discussion == null)
            {
                return NotFound(ApiResponse<Discussion>.ErrorResponse("Discussion not found for this lesson"));
            }
            return Ok(ApiResponse<Discussion>.SuccessResponse(discussion, "Discussion retrieved successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving discussion for lesson {LessonId}", lessonId);
            return StatusCode(500, ApiResponse<Discussion>.ErrorResponse("An error occurred while retrieving discussion"));
        }
    }

    [HttpGet("discussions/{id}")]
    public async Task<ActionResult<ApiResponse<object>>> GetDiscussionById(string id)
    {
        try
        {
            var discussion = await _discussionRepository.GetByIdAsync(id);
            if (discussion == null)
            {
                return NotFound(ApiResponse<object>.ErrorResponse("Discussion not found"));
            }

            // Enrich discussion with lesson and course information
            var lesson = await _lessonRepository.GetByIdAsync(discussion.LessonId);
            if (lesson == null)
            {
                return NotFound(ApiResponse<object>.ErrorResponse("Lesson not found for this discussion"));
            }

            var course = await _courseRepository.GetByIdAsync(lesson.CourseId);
            var contributionCount = await _postRepository.GetUniqueUserCountByDiscussionIdAsync(discussion.Id ?? "");

            var result = new
            {
                id = discussion.Id,
                lessonId = discussion.LessonId,
                discussionTitle = lesson.Title,
                courseTitle = course?.Title ?? "Unknown Course",
                courseId = lesson.CourseId,
                description = discussion.Description,
                contributions = contributionCount,
                createdAt = discussion.CreatedAt,
                updatedAt = discussion.UpdatedAt
            };

            return Ok(ApiResponse<object>.SuccessResponse(result, "Discussion retrieved successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving discussion {DiscussionId}", id);
            return StatusCode(500, ApiResponse<object>.ErrorResponse("An error occurred while retrieving discussion"));
        }
    }

    [HttpPost("discussions")]
    public async Task<ActionResult<ApiResponse<Discussion>>> CreateDiscussion([FromBody] Discussion discussion)
    {
        try
        {
            discussion.CreatedAt = DateTime.UtcNow;
            discussion.UpdatedAt = DateTime.UtcNow;
            var created = await _discussionRepository.CreateAsync(discussion);
            return CreatedAtAction(nameof(GetDiscussionById), new { id = created.Id },
                ApiResponse<Discussion>.SuccessResponse(created, "Discussion created successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating discussion");
            return StatusCode(500, ApiResponse<Discussion>.ErrorResponse("An error occurred while creating discussion"));
        }
    }

    [HttpPut("discussions/{id}")]
    public async Task<ActionResult<ApiResponse<Discussion>>> UpdateDiscussion(string id, [FromBody] Discussion discussion)
    {
        try
        {
            discussion.Id = id;
            discussion.UpdatedAt = DateTime.UtcNow;
            var updated = await _discussionRepository.UpdateAsync(id, discussion);
            if (updated == null)
            {
                return NotFound(ApiResponse<Discussion>.ErrorResponse("Discussion not found"));
            }
            return Ok(ApiResponse<Discussion>.SuccessResponse(updated, "Discussion updated successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating discussion {DiscussionId}", id);
            return StatusCode(500, ApiResponse<Discussion>.ErrorResponse("An error occurred while updating discussion"));
        }
    }

    [HttpDelete("discussions/{id}")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteDiscussion(string id)
    {
        try
        {
            var deleted = await _discussionRepository.DeleteAsync(id);
            if (!deleted)
            {
                return NotFound(ApiResponse<bool>.ErrorResponse("Discussion not found"));
            }
            return Ok(ApiResponse<bool>.SuccessResponse(true, "Discussion deleted successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting discussion {DiscussionId}", id);
            return StatusCode(500, ApiResponse<bool>.ErrorResponse("An error occurred while deleting discussion"));
        }
    }

    [HttpGet("discussions")]
    public async Task<ActionResult<ApiResponse<List<object>>>> GetAllDiscussions([FromQuery] string? userId = null, [FromQuery] string? userRole = null)
    {
        try
        {
            _logger.LogInformation("GetAllDiscussions called - userId: {UserId}, userRole: {UserRole}", userId ?? "null", userRole ?? "null");
            
            var allDiscussions = await _discussionRepository.GetAllAsync();
            var discussionsToReturn = allDiscussions;

            // Filter by enrollment for regular users (non-admin)
            // If userId is provided, we filter (default to non-admin if role is missing)
            if (!string.IsNullOrEmpty(userId))
            {
                // If role is not provided or is not admin, filter by enrollment
                bool isAdmin = !string.IsNullOrEmpty(userRole) && userRole.ToLower() == "admin";
                
                if (!isAdmin)
                {
                    _logger.LogInformation("Filtering discussions for user {UserId} with role {UserRole}", userId, userRole ?? "user (default)");
                    
                    // Get all courses the user is enrolled in
                    var userCourses = await _userCourseRepository.GetByUserIdAsync(userId);
                    var enrolledCourseIds = userCourses.Select(uc => uc.CourseId).ToList();
                    
                    _logger.LogInformation("User {UserId} is enrolled in {Count} courses: {CourseIds}", userId, enrolledCourseIds.Count, string.Join(", ", enrolledCourseIds));

                    // Filter discussions to only those from enrolled courses
                    var filteredDiscussions = new List<Models.Discussion>();
                    foreach (var discussion in allDiscussions)
                    {
                        var lesson = await _lessonRepository.GetByIdAsync(discussion.LessonId);
                        if (lesson != null && enrolledCourseIds.Contains(lesson.CourseId))
                        {
                            filteredDiscussions.Add(discussion);
                            _logger.LogInformation("Including discussion {DiscussionId} for course {CourseId}", discussion.Id, lesson.CourseId);
                        }
                        else if (lesson != null)
                        {
                            _logger.LogInformation("Excluding discussion {DiscussionId} for course {CourseId} (not enrolled)", discussion.Id, lesson.CourseId);
                        }
                    }
                    discussionsToReturn = filteredDiscussions;
                    _logger.LogInformation("Filtered {FilteredCount} discussions from {TotalCount} total", filteredDiscussions.Count, allDiscussions.Count());
                }
                else
                {
                    _logger.LogInformation("Admin user - returning all discussions without filtering");
                }
            }
            else
            {
                _logger.LogWarning("No userId provided - returning all discussions (should not happen for authenticated requests)");
            }

            var result = new List<object>();

            foreach (var discussion in discussionsToReturn)
            {
                var lesson = await _lessonRepository.GetByIdAsync(discussion.LessonId);
                if (lesson == null) continue;

                var course = await _courseRepository.GetByIdAsync(lesson.CourseId);
                var contributionCount = await _postRepository.GetUniqueUserCountByDiscussionIdAsync(discussion.Id ?? "");

                result.Add(new
                {
                    id = discussion.Id,
                    lessonId = discussion.LessonId,
                    discussionTitle = lesson.Title,
                    courseTitle = course?.Title ?? "Unknown Course",
                    courseId = lesson.CourseId,
                    description = discussion.Description,
                    contributions = contributionCount,
                    createdAt = discussion.CreatedAt,
                    updatedAt = discussion.UpdatedAt
                });
            }

            return Ok(ApiResponse<List<object>>.SuccessResponse(result, "Discussions retrieved successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all discussions");
            return StatusCode(500, ApiResponse<List<object>>.ErrorResponse("An error occurred while retrieving discussions"));
        }
    }

    [HttpGet("discussions/{discussionId}/posts")]
    public async Task<ActionResult<ApiResponse<List<DiscussionPostWithUserDto>>>> GetPostsByDiscussion(string discussionId)
    {
        try
        {
            var posts = await _postRepository.GetPostsByDiscussionIdWithUsersAsync(discussionId);
            return Ok(ApiResponse<List<DiscussionPostWithUserDto>>.SuccessResponse(posts.ToList(), "Posts retrieved successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving posts for discussion {DiscussionId}", discussionId);
            return StatusCode(500, ApiResponse<List<DiscussionPostWithUserDto>>.ErrorResponse("An error occurred while retrieving posts"));
        }
    }
}


