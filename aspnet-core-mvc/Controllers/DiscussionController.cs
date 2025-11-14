using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CourseManagementAPI.Models;
using CourseManagementAPI.Services;

namespace CourseManagementAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DiscussionController : ControllerBase
{
    private readonly IDiscussionService _discussionService;
    private readonly IJwtService _jwtService;

    public DiscussionController(IDiscussionService discussionService, IJwtService jwtService)
    {
        _discussionService = discussionService;
        _jwtService = jwtService;
    }

    [HttpGet]
    public async Task<ActionResult<List<Discussion>>> GetAllDiscussions()
    {
        var discussions = await _discussionService.GetAllDiscussionsAsync();
        return Ok(discussions);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Discussion>> GetDiscussionById(string id)
    {
        var discussion = await _discussionService.GetDiscussionByIdAsync(id);
        if (discussion == null)
        {
            return NotFound();
        }
        return Ok(discussion);
    }

    [HttpGet("course/{courseId}")]
    public async Task<ActionResult<List<Discussion>>> GetDiscussionsByCourseId(string courseId)
    {
        var discussions = await _discussionService.GetDiscussionsByCourseIdAsync(courseId);
        return Ok(discussions);
    }

    [HttpPost]
    public async Task<ActionResult<Discussion>> CreateDiscussion([FromBody] Discussion discussion)
    {
        try
        {
            // Get user ID from authenticated user claims (better approach)
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value 
                ?? User.FindFirst("nameid")?.Value;
            
            if (string.IsNullOrEmpty(userId))
            {
                // Fallback to manual token extraction if claims not available
                var authHeader = Request.Headers["Authorization"].ToString();
                if (authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                {
                    var token = authHeader.Substring("Bearer ".Length).Trim();
                    userId = _jwtService.GetUserIdFromToken(token);
                }
                else
                {
                    return Unauthorized(new { message = "Invalid authorization header" });
                }
            }
            
            var createdDiscussion = await _discussionService.CreateDiscussionAsync(discussion, userId);
            return StatusCode(201, createdDiscussion);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<Discussion>> UpdateDiscussion(string id, [FromBody] Discussion discussion)
    {
        try
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value 
                ?? User.FindFirst("nameid")?.Value;
            
            if (string.IsNullOrEmpty(userId))
            {
                var authHeader = Request.Headers["Authorization"].ToString();
                if (authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                {
                    var token = authHeader.Substring("Bearer ".Length).Trim();
                    userId = _jwtService.GetUserIdFromToken(token);
                }
                else
                {
                    return Unauthorized(new { message = "Invalid authorization header" });
                }
            }
            
            var updatedDiscussion = await _discussionService.UpdateDiscussionAsync(id, discussion, userId);
            return Ok(updatedDiscussion);
        }
        catch (Exception ex)
        {
            if (ex.Message.Contains("not found"))
            {
                return NotFound();
            }
            return StatusCode(403, new { message = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteDiscussion(string id)
    {
        try
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value 
                ?? User.FindFirst("nameid")?.Value;
            
            if (string.IsNullOrEmpty(userId))
            {
                var authHeader = Request.Headers["Authorization"].ToString();
                if (authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                {
                    var token = authHeader.Substring("Bearer ".Length).Trim();
                    userId = _jwtService.GetUserIdFromToken(token);
                }
                else
                {
                    return Unauthorized(new { message = "Invalid authorization header" });
                }
            }
            
            await _discussionService.DeleteDiscussionAsync(id, userId);
            return NoContent();
        }
        catch (Exception ex)
        {
            if (ex.Message.Contains("not found"))
            {
                return NotFound();
            }
            return StatusCode(403, new { message = ex.Message });
        }
    }
}


