using Microsoft.AspNetCore.Mvc;
using Gateway.Services;
using Shared.Common;

namespace Gateway.Controllers;

[ApiController]
[Route("api")]
public class SlidesController : ControllerBase
{
    private readonly ICoursesGatewayService _coursesGatewayService;
    private readonly ILogger<SlidesController> _logger;

    public SlidesController(ICoursesGatewayService coursesGatewayService, ILogger<SlidesController> logger)
    {
        _coursesGatewayService = coursesGatewayService;
        _logger = logger;
    }

    [HttpGet("lessons/{lessonId}/slides")]
    public async Task<ActionResult<ApiResponse<PagedResponse<object>>>> GetSlidesByLesson(string lessonId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var response = await _coursesGatewayService.GetSlidesByLessonAsync(lessonId, page, pageSize);
        return StatusCode(response.Success ? 200 : 500, response);
    }

    [HttpGet("slides/{id}")]
    public async Task<ActionResult<ApiResponse<object>>> GetSlideById(string id)
    {
        var response = await _coursesGatewayService.GetSlideByIdAsync(id);
        return StatusCode(response.Success ? 200 : 404, response);
    }

    [HttpPost("slides")]
    public async Task<ActionResult<ApiResponse<object>>> CreateSlide([FromBody] object slide)
    {
        var response = await _coursesGatewayService.CreateSlideAsync(slide);
        return StatusCode(response.Success ? 201 : 400, response);
    }

    [HttpPut("slides/{id}")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateSlide(string id, [FromBody] object slide)
    {
        var response = await _coursesGatewayService.UpdateSlideAsync(id, slide);
        return StatusCode(response.Success ? 200 : 404, response);
    }

    [HttpDelete("slides/{id}")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteSlide(string id)
    {
        var response = await _coursesGatewayService.DeleteSlideAsync(id);
        return StatusCode(response.Success ? 200 : 404, response);
    }
}














