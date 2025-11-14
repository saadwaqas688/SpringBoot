using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CourseManagementAPI.DTOs;
using CourseManagementAPI.Models;
using CourseManagementAPI.Services;

namespace CourseManagementAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PostController : ControllerBase
{
    private readonly IPostService _postService;
    private readonly IJwtService _jwtService;

    public PostController(IPostService postService, IJwtService jwtService)
    {
        _postService = postService;
        _jwtService = jwtService;
    }

    [HttpGet("discussion/{discussionId}")]
    public async Task<ActionResult<List<Post>>> GetPostsByDiscussionId(string discussionId)
    {
        var posts = await _postService.GetPostsByDiscussionIdAsync(discussionId);
        return Ok(posts);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Post>> GetPostById(string id)
    {
        var post = await _postService.GetPostByIdAsync(id);
        if (post == null)
        {
            return NotFound();
        }
        return Ok(post);
    }

    [HttpPost]
    public async Task<ActionResult<Post>> CreatePost([FromBody] CreatePostRequest request)
    {
        try
        {
            var userId = _jwtService.GetUserIdFromToken(Request.Headers["Authorization"].ToString().Replace("Bearer ", ""));
            
            var post = new Post
            {
                DiscussionId = request.DiscussionId,
                CourseId = request.CourseId,
                Content = request.Content
            };

            var createdPost = await _postService.CreatePostAsync(post, userId);
            return StatusCode(201, createdPost);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<Post>> UpdatePost(string id, [FromBody] UpdatePostRequest request)
    {
        try
        {
            var userId = _jwtService.GetUserIdFromToken(Request.Headers["Authorization"].ToString().Replace("Bearer ", ""));
            
            var post = new Post
            {
                Content = request.Content
            };

            var updatedPost = await _postService.UpdatePostAsync(id, post, userId);
            return Ok(updatedPost);
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
    public async Task<IActionResult> DeletePost(string id)
    {
        try
        {
            var userId = _jwtService.GetUserIdFromToken(Request.Headers["Authorization"].ToString().Replace("Bearer ", ""));
            await _postService.DeletePostAsync(id, userId);
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

