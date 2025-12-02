using Microsoft.AspNetCore.Mvc;
using Gateway.Services;
using Shared.Common;

namespace Gateway.Controllers;

[ApiController]
[Route("api")]
public class ProgressController : ControllerBase
{
    private readonly ICoursesGatewayService _coursesGatewayService;
    private readonly ILogger<ProgressController> _logger;

    public ProgressController(ICoursesGatewayService coursesGatewayService, ILogger<ProgressController> logger)
    {
        _coursesGatewayService = coursesGatewayService;
        _logger = logger;
    }

    [HttpGet("users/{userId}/courses/{courseId}/progress")]
    public async Task<ActionResult<ApiResponse<object>>> GetCourseProgress(string userId, string courseId)
    {
        var response = await _coursesGatewayService.GetCourseProgressAsync(userId, courseId);
        return StatusCode(response.Success ? 200 : 404, response);
    }

    [HttpPost("progress/lesson/start")]
    public async Task<ActionResult<ApiResponse<object>>> StartLesson([FromBody] object dto)
    {
        var response = await _coursesGatewayService.StartLessonAsync(dto);
        return StatusCode(response.Success ? 200 : 400, response);
    }

    [HttpPost("progress/lesson/complete")]
    public async Task<ActionResult<ApiResponse<object>>> CompleteLesson([FromBody] object dto)
    {
        var response = await _coursesGatewayService.CompleteLessonAsync(dto);
        return StatusCode(response.Success ? 200 : 400, response);
    }

    [HttpPost("progress/slide/view")]
    public async Task<ActionResult<ApiResponse<object>>> ViewSlide([FromBody] object dto)
    {
        var response = await _coursesGatewayService.ViewSlideAsync(dto);
        return StatusCode(response.Success ? 200 : 400, response);
    }

    [HttpPost("progress/slide/complete")]
    public async Task<ActionResult<ApiResponse<object>>> CompleteSlide([FromBody] object dto)
    {
        var response = await _coursesGatewayService.CompleteSlideAsync(dto);
        return StatusCode(response.Success ? 200 : 400, response);
    }

    [HttpPost("activity/log")]
    public async Task<ActionResult<ApiResponse<object>>> LogActivity([FromBody] object activity)
    {
        var response = await _coursesGatewayService.LogActivityAsync(activity);
        return StatusCode(response.Success ? 200 : 400, response);
    }

    [HttpGet("activity/user/{userId}")]
    public async Task<ActionResult<ApiResponse<List<object>>>> GetUserActivities(string userId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var response = await _coursesGatewayService.GetUserActivitiesAsync(userId, page, pageSize);
        return StatusCode(response.Success ? 200 : 500, response);
    }

    [HttpGet("activity/course/{courseId}")]
    public async Task<ActionResult<ApiResponse<List<object>>>> GetCourseActivities(string courseId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var response = await _coursesGatewayService.GetCourseActivitiesAsync(courseId, page, pageSize);
        return StatusCode(response.Success ? 200 : 500, response);
    }
}





