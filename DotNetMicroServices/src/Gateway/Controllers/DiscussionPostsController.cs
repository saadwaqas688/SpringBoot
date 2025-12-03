using Microsoft.AspNetCore.Mvc;
using Gateway.Services;
using Shared.Common;

namespace Gateway.Controllers;

[ApiController]
[Route("api")]
public class DiscussionPostsController : ControllerBase
{
    private readonly ICoursesGatewayService _coursesGatewayService;
    private readonly ILogger<DiscussionPostsController> _logger;

    public DiscussionPostsController(ICoursesGatewayService coursesGatewayService, ILogger<DiscussionPostsController> logger)
    {
        _coursesGatewayService = coursesGatewayService;
        _logger = logger;
    }

    [HttpGet("lessons/{lessonId}/posts")]
    public async Task<ActionResult<ApiResponse<List<object>>>> GetPostsByLesson(string lessonId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var response = await _coursesGatewayService.GetPostsByLessonAsync(lessonId, page, pageSize);
        return StatusCode(response.Success ? 200 : 500, response);
    }

    [HttpGet("posts/{postId}")]
    public async Task<ActionResult<ApiResponse<object>>> GetPostById(string postId)
    {
        var response = await _coursesGatewayService.GetPostByIdAsync(postId);
        return StatusCode(response.Success ? 200 : 404, response);
    }

    [HttpPost("posts")]
    public async Task<ActionResult<ApiResponse<object>>> CreatePost([FromBody] object post)
    {
        var response = await _coursesGatewayService.CreatePostAsync(post);
        return StatusCode(response.Success ? 201 : 400, response);
    }

    [HttpPut("posts/{postId}")]
    public async Task<ActionResult<ApiResponse<object>>> UpdatePost(string postId, [FromBody] object post)
    {
        var response = await _coursesGatewayService.UpdatePostAsync(postId, post);
        return StatusCode(response.Success ? 200 : 404, response);
    }

    [HttpDelete("posts/{postId}")]
    public async Task<ActionResult<ApiResponse<bool>>> DeletePost(string postId)
    {
        var response = await _coursesGatewayService.DeletePostAsync(postId);
        return StatusCode(response.Success ? 200 : 404, response);
    }

    [HttpGet("posts/{postId}/comments")]
    public async Task<ActionResult<ApiResponse<List<object>>>> GetComments(string postId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var response = await _coursesGatewayService.GetCommentsAsync(postId, page, pageSize);
        return StatusCode(response.Success ? 200 : 500, response);
    }
}











