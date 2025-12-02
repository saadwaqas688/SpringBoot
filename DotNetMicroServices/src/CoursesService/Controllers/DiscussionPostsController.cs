using Microsoft.AspNetCore.Mvc;
using CoursesService.Models;
using CoursesService.Repositories;
using CoursesService.DTOs;
using Shared.Common;

namespace CoursesService.Controllers;

[ApiController]
[Route("api")]
public class DiscussionPostsController : ControllerBase
{
    private readonly IDiscussionPostRepository _postRepository;
    private readonly ILogger<DiscussionPostsController> _logger;

    public DiscussionPostsController(
        IDiscussionPostRepository postRepository,
        ILogger<DiscussionPostsController> logger)
    {
        _postRepository = postRepository;
        _logger = logger;
    }

    [HttpGet("lessons/{lessonId}/posts")]
    public async Task<ActionResult<ApiResponse<List<DiscussionPostWithUserDto>>>> GetPostsByLesson(string lessonId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        try
        {
            var posts = await _postRepository.GetPostsByLessonIdWithUsersAsync(lessonId);
            var pagedPosts = posts.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            return Ok(ApiResponse<List<DiscussionPostWithUserDto>>.SuccessResponse(pagedPosts, "Posts retrieved successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving posts for lesson {LessonId}", lessonId);
            return StatusCode(500, ApiResponse<List<DiscussionPostWithUserDto>>.ErrorResponse("An error occurred while retrieving posts"));
        }
    }

    [HttpGet("posts/{postId}")]
    public async Task<ActionResult<ApiResponse<DiscussionPost>>> GetPostById(string postId)
    {
        try
        {
            var post = await _postRepository.GetByIdAsync(postId);
            if (post == null)
            {
                return NotFound(ApiResponse<DiscussionPost>.ErrorResponse("Post not found"));
            }
            return Ok(ApiResponse<DiscussionPost>.SuccessResponse(post, "Post retrieved successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving post {PostId}", postId);
            return StatusCode(500, ApiResponse<DiscussionPost>.ErrorResponse("An error occurred while retrieving post"));
        }
    }

    [HttpPost("posts")]
    public async Task<ActionResult<ApiResponse<DiscussionPost>>> CreatePost([FromBody] DiscussionPost post)
    {
        try
        {
            post.CreatedAt = DateTime.UtcNow;
            post.UpdatedAt = DateTime.UtcNow;
            var created = await _postRepository.CreateAsync(post);
            return CreatedAtAction(nameof(GetPostById), new { postId = created.Id },
                ApiResponse<DiscussionPost>.SuccessResponse(created, "Post created successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating post");
            return StatusCode(500, ApiResponse<DiscussionPost>.ErrorResponse("An error occurred while creating post"));
        }
    }

    [HttpPut("posts/{postId}")]
    public async Task<ActionResult<ApiResponse<DiscussionPost>>> UpdatePost(string postId, [FromBody] DiscussionPost post)
    {
        try
        {
            post.Id = postId;
            post.UpdatedAt = DateTime.UtcNow;
            var updated = await _postRepository.UpdateAsync(postId, post);
            if (updated == null)
            {
                return NotFound(ApiResponse<DiscussionPost>.ErrorResponse("Post not found"));
            }
            return Ok(ApiResponse<DiscussionPost>.SuccessResponse(updated, "Post updated successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating post {PostId}", postId);
            return StatusCode(500, ApiResponse<DiscussionPost>.ErrorResponse("An error occurred while updating post"));
        }
    }

    [HttpDelete("posts/{postId}")]
    public async Task<ActionResult<ApiResponse<bool>>> DeletePost(string postId)
    {
        try
        {
            var deleted = await _postRepository.DeleteAsync(postId);
            if (!deleted)
            {
                return NotFound(ApiResponse<bool>.ErrorResponse("Post not found"));
            }
            return Ok(ApiResponse<bool>.SuccessResponse(true, "Post deleted successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting post {PostId}", postId);
            return StatusCode(500, ApiResponse<bool>.ErrorResponse("An error occurred while deleting post"));
        }
    }

    [HttpGet("posts/{postId}/comments")]
    public async Task<ActionResult<ApiResponse<List<DiscussionPostWithUserDto>>>> GetComments(string postId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        try
        {
            var comments = await _postRepository.GetCommentsByPostIdWithUsersAsync(postId);
            var pagedComments = comments.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            return Ok(ApiResponse<List<DiscussionPostWithUserDto>>.SuccessResponse(pagedComments, "Comments retrieved successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving comments for post {PostId}", postId);
            return StatusCode(500, ApiResponse<List<DiscussionPostWithUserDto>>.ErrorResponse("An error occurred while retrieving comments"));
        }
    }
}


