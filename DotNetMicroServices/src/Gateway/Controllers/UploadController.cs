using Microsoft.AspNetCore.Mvc;
using Gateway.Services;
using Shared.Common;

namespace Gateway.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UploadController : ControllerBase
{
    private readonly ICoursesGatewayService _coursesGatewayService;
    private readonly ILogger<UploadController> _logger;

    public UploadController(
        ICoursesGatewayService coursesGatewayService,
        ILogger<UploadController> logger)
    {
        _coursesGatewayService = coursesGatewayService;
        _logger = logger;
    }

    [HttpPost("image")]
    public async Task<ActionResult<ApiResponse<string>>> UploadImage(IFormFile file)
    {
        try
        {
            // Forward the file upload to CoursesService
            var response = await _coursesGatewayService.UploadImageAsync(file);
            return StatusCode(response.Success ? 200 : 400, response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading image through gateway");
            return StatusCode(500, ApiResponse<string>.ErrorResponse($"Error uploading image: {ex.Message}"));
        }
    }

    [HttpDelete("image/{fileName}")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteImage(string fileName)
    {
        try
        {
            var response = await _coursesGatewayService.DeleteImageAsync(fileName);
            return StatusCode(response.Success ? 200 : 404, response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting image through gateway");
            return StatusCode(500, ApiResponse<bool>.ErrorResponse($"Error deleting image: {ex.Message}"));
        }
    }

}














