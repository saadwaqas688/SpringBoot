using Microsoft.AspNetCore.Mvc;
using Gateway.Infrastructure.Services;
using Shared.Core.Common;

namespace Gateway.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly ICoursesGatewayService _coursesGatewayService;
    private readonly ILogger<UsersController> _logger;

    public UsersController(ICoursesGatewayService coursesGatewayService, ILogger<UsersController> logger)
    {
        _coursesGatewayService = coursesGatewayService;
        _logger = logger;
    }

    [HttpGet("{userId}/courses")]
    public async Task<ActionResult<ApiResponse<List<object>>>> GetUserCourses(string userId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var response = await _coursesGatewayService.GetUserCoursesAsync(userId, page, pageSize);
        return StatusCode(response.Success ? 200 : 500, response);
    }

    [HttpGet("{userId}/progress")]
    public async Task<ActionResult<ApiResponse<object>>> GetUserProgress(string userId)
    {
        var response = await _coursesGatewayService.GetUserProgressAsync(userId);
        return StatusCode(response.Success ? 200 : 500, response);
    }

}




