using Microsoft.AspNetCore.Mvc;
using Gateway.Services;
using Shared.Common;

namespace Gateway.Controllers;

[ApiController]
[Route("api")]
public class LessonsController : ControllerBase
{
    private readonly ICoursesGatewayService _coursesGatewayService;
    private readonly ILogger<LessonsController> _logger;

    public LessonsController(ICoursesGatewayService coursesGatewayService, ILogger<LessonsController> logger)
    {
        _coursesGatewayService = coursesGatewayService;
        _logger = logger;
    }

    [HttpGet("courses/{courseId}/lessons")]
    public async Task<ActionResult<ApiResponse<List<object>>>> GetLessonsByCourse(string courseId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var response = await _coursesGatewayService.GetLessonsByCourseAsync(courseId, page, pageSize);
        return StatusCode(response.Success ? 200 : 500, response);
    }

    [HttpGet("lessons/{id}")]
    public async Task<ActionResult<ApiResponse<object>>> GetLessonById(string id)
    {
        var response = await _coursesGatewayService.GetLessonByIdAsync(id);
        return StatusCode(response.Success ? 200 : 404, response);
    }

    [HttpPost("lessons")]
    public async Task<ActionResult<ApiResponse<object>>> CreateLesson([FromBody] object lesson)
    {
        var response = await _coursesGatewayService.CreateLessonAsync(lesson);
        return StatusCode(response.Success ? 201 : 400, response);
    }

    [HttpPut("lessons/{id}")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateLesson(string id, [FromBody] object lesson)
    {
        var response = await _coursesGatewayService.UpdateLessonAsync(id, lesson);
        return StatusCode(response.Success ? 200 : 404, response);
    }

    [HttpDelete("lessons/{id}")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteLesson(string id)
    {
        var response = await _coursesGatewayService.DeleteLessonAsync(id);
        return StatusCode(response.Success ? 200 : 404, response);
    }
}












