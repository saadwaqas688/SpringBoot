using Microsoft.AspNetCore.Mvc;
using Gateway.Infrastructure.Services;
using Shared.Core.Common;

namespace Gateway.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CoursesController : ControllerBase
{
    private readonly ICoursesGatewayService _coursesGatewayService;
    private readonly ILogger<CoursesController> _logger;

    public CoursesController(
        ICoursesGatewayService coursesGatewayService,
        ILogger<CoursesController> logger)
    {
        _coursesGatewayService = coursesGatewayService;
        _logger = logger;
    }

    [HttpGet("health")]
    public async Task<ActionResult<ApiResponse<string>>> HealthCheck()
    {
        var response = await _coursesGatewayService.HealthCheckAsync();
        return StatusCode(response.Success ? 200 : 500, response);
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResponse<object>>>> GetAllCourses(
        [FromQuery] int page = 1, 
        [FromQuery] int pageSize = 10)
    {
        var response = await _coursesGatewayService.GetAllCoursesAsync(page, pageSize);
        return StatusCode(response.Success ? 200 : 500, response);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<object>>> GetCourseById(string id)
    {
        var response = await _coursesGatewayService.GetCourseByIdAsync(id);
        return StatusCode(response.Success ? 200 : 404, response);
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<object>>> CreateCourse([FromBody] object course)
    {
        var response = await _coursesGatewayService.CreateCourseAsync(course);
        return StatusCode(response.Success ? 201 : 400, response);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateCourse(string id, [FromBody] object course)
    {
        var response = await _coursesGatewayService.UpdateCourseAsync(id, course);
        return StatusCode(response.Success ? 200 : 404, response);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteCourse(string id)
    {
        var response = await _coursesGatewayService.DeleteCourseAsync(id);
        return StatusCode(response.Success ? 200 : 404, response);
    }

    [HttpGet("status/{status}")]
    public async Task<ActionResult<ApiResponse<List<object>>>> GetCoursesByStatus(string status)
    {
        var response = await _coursesGatewayService.GetCoursesByStatusAsync(status);
        return StatusCode(response.Success ? 200 : 500, response);
    }

    [HttpGet("search")]
    public async Task<ActionResult<ApiResponse<List<object>>>> SearchCourses([FromQuery] string q)
    {
        if (string.IsNullOrWhiteSpace(q))
        {
            return BadRequest(ApiResponse<List<object>>.ErrorResponse("Search term is required"));
        }
        var response = await _coursesGatewayService.SearchCoursesAsync(q);
        return StatusCode(response.Success ? 200 : 500, response);
    }

    [HttpPost("{id}/assign-user")]
    public async Task<ActionResult<ApiResponse<object>>> AssignUser(string id, [FromBody] object dto)
    {
        var response = await _coursesGatewayService.AssignUserToCourseAsync(id, dto);
        return StatusCode(response.Success ? 200 : 400, response);
    }

    [HttpDelete("{id}/remove-user/{userId}")]
    public async Task<ActionResult<ApiResponse<bool>>> RemoveUser(string id, string userId)
    {
        var response = await _coursesGatewayService.RemoveUserFromCourseAsync(id, userId);
        return StatusCode(response.Success ? 200 : 404, response);
    }

    [HttpGet("{id}/assigned-users")]
    public async Task<ActionResult<ApiResponse<List<object>>>> GetAssignedUsers(string id, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var response = await _coursesGatewayService.GetAssignedUsersAsync(id, page, pageSize);
        return StatusCode(response.Success ? 200 : 500, response);
    }

    [HttpGet("{id}/user-progress/{userId}")]
    public async Task<ActionResult<ApiResponse<object>>> GetUserProgress(string id, string userId)
    {
        var response = await _coursesGatewayService.GetCourseUserProgressAsync(id, userId);
        return StatusCode(response.Success ? 200 : 404, response);
    }
}



