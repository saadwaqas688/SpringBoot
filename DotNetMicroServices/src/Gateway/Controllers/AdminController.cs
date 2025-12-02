using Microsoft.AspNetCore.Mvc;
using Gateway.Services;
using Shared.Common;

namespace Gateway.Controllers;

[ApiController]
[Route("api/admin")]
public class AdminController : ControllerBase
{
    private readonly ICoursesGatewayService _coursesGatewayService;
    private readonly ILogger<AdminController> _logger;

    public AdminController(ICoursesGatewayService coursesGatewayService, ILogger<AdminController> logger)
    {
        _coursesGatewayService = coursesGatewayService;
        _logger = logger;
    }

    [HttpGet("analytics/courses")]
    public async Task<ActionResult<ApiResponse<object>>> GetCourseAnalytics()
    {
        var response = await _coursesGatewayService.GetCourseAnalyticsAsync();
        return StatusCode(response.Success ? 200 : 500, response);
    }

    [HttpGet("analytics/users")]
    public async Task<ActionResult<ApiResponse<object>>> GetUserAnalytics()
    {
        var response = await _coursesGatewayService.GetUserAnalyticsAsync();
        return StatusCode(response.Success ? 200 : 500, response);
    }

    [HttpGet("analytics/engagement")]
    public async Task<ActionResult<ApiResponse<object>>> GetEngagementAnalytics()
    {
        var response = await _coursesGatewayService.GetEngagementAnalyticsAsync();
        return StatusCode(response.Success ? 200 : 500, response);
    }

    [HttpGet("analytics/performance")]
    public async Task<ActionResult<ApiResponse<object>>> GetPerformanceAnalytics()
    {
        var response = await _coursesGatewayService.GetPerformanceAnalyticsAsync();
        return StatusCode(response.Success ? 200 : 500, response);
    }
}





