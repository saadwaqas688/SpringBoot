using Microsoft.AspNetCore.Mvc;
using Gateway.Infrastructure.Services;
using Shared.Core.Common;

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
        try
        {
            var response = await _coursesGatewayService.GetCourseAnalyticsAsync();
            if (response == null)
            {
                _logger.LogError("GetCourseAnalyticsAsync returned null response");
                return StatusCode(500, ApiResponse<object>.ErrorResponse("Service unavailable. Please check if CoursesService is running."));
            }
            _logger.LogInformation("Course analytics response: Success={Success}, Message={Message}", response.Success, response.Message);
            return StatusCode(response.Success ? 200 : 500, response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in GetCourseAnalytics");
            return StatusCode(500, ApiResponse<object>.ErrorResponse($"An error occurred: {ex.Message}"));
        }
    }

    [HttpGet("analytics/users")]
    public async Task<ActionResult<ApiResponse<object>>> GetUserAnalytics()
    {
        try
        {
            var response = await _coursesGatewayService.GetUserAnalyticsAsync();
            if (response == null)
            {
                _logger.LogError("GetUserAnalyticsAsync returned null response");
                return StatusCode(500, ApiResponse<object>.ErrorResponse("Service unavailable. Please check if CoursesService is running."));
            }
            _logger.LogInformation("User analytics response: Success={Success}, Message={Message}", response.Success, response.Message);
            return StatusCode(response.Success ? 200 : 500, response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in GetUserAnalytics");
            return StatusCode(500, ApiResponse<object>.ErrorResponse($"An error occurred: {ex.Message}"));
        }
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










