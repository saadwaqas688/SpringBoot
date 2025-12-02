using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Gateway.Services;
using Shared.Common;

namespace Gateway.Controllers;

[ApiController]
[Route("api")]
public class DiscussionsController : ControllerBase
{
    private readonly ICoursesGatewayService _coursesGatewayService;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly ILogger<DiscussionsController> _logger;

    public DiscussionsController(
        ICoursesGatewayService coursesGatewayService,
        IJwtTokenService jwtTokenService,
        ILogger<DiscussionsController> logger)
    {
        _coursesGatewayService = coursesGatewayService;
        _jwtTokenService = jwtTokenService;
        _logger = logger;
    }

    [Authorize]
    [HttpGet("discussions")]
    public async Task<ActionResult<ApiResponse<List<object>>>> GetAllDiscussions()
    {
        // Extract userId and role from JWT claims (primary method)
        var userId = _jwtTokenService.GetUserIdFromClaims(User);
        var userRole = _jwtTokenService.GetUserRoleFromClaims(User);
        
        // Fallback: If not found in claims, extract from token directly
        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(userRole))
        {
            var authHeader = Request.Headers["Authorization"].ToString();
            if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                var token = authHeader.Substring("Bearer ".Length).Trim();
                userId = userId ?? _jwtTokenService.GetUserIdFromToken(token);
                userRole = userRole ?? _jwtTokenService.GetUserRoleFromToken(token);
            }
        }
        
        _logger.LogInformation("GetAllDiscussions - userId: {UserId}, userRole: {UserRole}", userId ?? "null", userRole ?? "null");
        
        var response = await _coursesGatewayService.GetAllDiscussionsAsync(userId, userRole);
        return StatusCode(response.Success ? 200 : 500, response);
    }

    [HttpGet("discussions/{discussionId}/posts")]
    public async Task<ActionResult<ApiResponse<List<object>>>> GetPostsByDiscussion(string discussionId)
    {
        var response = await _coursesGatewayService.GetPostsByDiscussionAsync(discussionId);
        return StatusCode(response.Success ? 200 : 500, response);
    }

    [HttpGet("discussions/{id}")]
    public async Task<ActionResult<ApiResponse<object>>> GetDiscussionById(string id)
    {
        var response = await _coursesGatewayService.GetDiscussionByIdAsync(id);
        return StatusCode(response.Success ? 200 : 404, response);
    }

    [HttpGet("lessons/{lessonId}/discussions", Name = "GetDiscussionByLesson")]
    public async Task<ActionResult<ApiResponse<object>>> GetDiscussionByLesson(string lessonId)
    {
        var response = await _coursesGatewayService.GetDiscussionByLessonAsync(lessonId);
        // Return the response as-is, even if it's a 404 (discussion not found is valid)
        if (response.Success)
        {
            return Ok(response);
        }
        return StatusCode(404, response);
    }

    [HttpPost("discussions")]
    public async Task<ActionResult<ApiResponse<object>>> CreateDiscussion([FromBody] object discussion)
    {
        var response = await _coursesGatewayService.CreateDiscussionAsync(discussion);
        return StatusCode(response.Success ? 201 : 400, response);
    }

    [HttpPut("discussions/{id}")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateDiscussion(string id, [FromBody] object discussion)
    {
        var response = await _coursesGatewayService.UpdateDiscussionAsync(id, discussion);
        return StatusCode(response.Success ? 200 : 404, response);
    }

    [HttpDelete("discussions/{id}")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteDiscussion(string id)
    {
        var response = await _coursesGatewayService.DeleteDiscussionAsync(id);
        return StatusCode(response.Success ? 200 : 404, response);
    }
}

