using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LMS.API.DTOs;
using LMS.API.Services;
using System.Security.Claims;

namespace LMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DiscussionController : ControllerBase
{
    private readonly IDiscussionService _discussionService;

    public DiscussionController(IDiscussionService discussionService)
    {
        _discussionService = discussionService;
    }

    [HttpGet("content/{contentId}")]
    public async Task<ActionResult<List<DiscussionPostDto>>> GetPostsByContentId(string contentId)
    {
        var posts = await _discussionService.GetPostsByContentIdAsync(contentId);
        return Ok(posts);
    }

    [HttpPost]
    public async Task<ActionResult<DiscussionPostDto>> CreatePost([FromBody] CreateDiscussionPostDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        try
        {
            var post = await _discussionService.CreatePostAsync(dto, userId);
            return CreatedAtAction(nameof(GetPostsByContentId), new { contentId = dto.ContentId }, post);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<DiscussionPostDto>> UpdatePost(string id, [FromBody] UpdateDiscussionPostDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        try
        {
            var post = await _discussionService.UpdatePostAsync(id, dto, userId);
            return Ok(post);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePost(string id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        try
        {
            await _discussionService.DeletePostAsync(id, userId);
            return NoContent();
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}

