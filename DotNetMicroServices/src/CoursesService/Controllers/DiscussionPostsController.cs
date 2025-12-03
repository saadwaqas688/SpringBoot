using Microsoft.AspNetCore.Mvc;
using CoursesService.Models;
using CoursesService.Services;
using CoursesService.DTOs;
using Shared.Common;

namespace CoursesService.Controllers;

[ApiController]
[Route("api")]
public class DiscussionPostsController : ControllerBase
{
    private readonly IDiscussionPostService _postService;

    public DiscussionPostsController(IDiscussionPostService postService)
    {
        _postService = postService;
    }

    [HttpGet("lessons/{lessonId}/posts")]
    public async Task<ActionResult<ApiResponse<List<DiscussionPostWithUserDto>>>> GetPostsByLesson(string lessonId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var response = await _postService.GetPostsByLessonAsync(lessonId, page, pageSize);
        return response.Success ? Ok(response) : StatusCode(500, response);
    }

    [HttpGet("posts/{postId}")]
    public async Task<ActionResult<ApiResponse<DiscussionPost>>> GetPostById(string postId)
    {
        var response = await _postService.GetPostByIdAsync(postId);
        if (!response.Success && response.Message == "Post not found")
        {
            return NotFound(response);
        }
        return response.Success ? Ok(response) : StatusCode(500, response);
    }

    [HttpPost("posts")]
    public async Task<ActionResult<ApiResponse<DiscussionPost>>> CreatePost([FromBody] DiscussionPost post)
    {
        var response = await _postService.CreatePostAsync(post);
        if (response.Success && response.Data != null)
        {
            return CreatedAtAction(nameof(GetPostById), new { postId = response.Data.Id }, response);
        }
        return StatusCode(500, response);
    }

    [HttpPut("posts/{postId}")]
    public async Task<ActionResult<ApiResponse<DiscussionPost>>> UpdatePost(string postId, [FromBody] DiscussionPost post)
    {
        var response = await _postService.UpdatePostAsync(postId, post);
        if (!response.Success && response.Message == "Post not found")
        {
            return NotFound(response);
        }
        return response.Success ? Ok(response) : StatusCode(500, response);
    }

    [HttpDelete("posts/{postId}")]
    public async Task<ActionResult<ApiResponse<bool>>> DeletePost(string postId)
    {
        var response = await _postService.DeletePostAsync(postId);
        if (!response.Success && response.Message == "Post not found")
        {
            return NotFound(response);
        }
        return response.Success ? Ok(response) : StatusCode(500, response);
    }

    [HttpGet("posts/{postId}/comments")]
    public async Task<ActionResult<ApiResponse<List<DiscussionPostWithUserDto>>>> GetComments(string postId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var response = await _postService.GetCommentsAsync(postId, page, pageSize);
        return response.Success ? Ok(response) : StatusCode(500, response);
    }
}


