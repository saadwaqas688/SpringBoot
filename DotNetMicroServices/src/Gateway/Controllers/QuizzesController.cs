using Microsoft.AspNetCore.Mvc;
using Gateway.Services;
using Shared.Common;

namespace Gateway.Controllers;

[ApiController]
[Route("api")]
public class QuizzesController : ControllerBase
{
    private readonly ICoursesGatewayService _coursesGatewayService;
    private readonly ILogger<QuizzesController> _logger;

    public QuizzesController(ICoursesGatewayService coursesGatewayService, ILogger<QuizzesController> logger)
    {
        _coursesGatewayService = coursesGatewayService;
        _logger = logger;
    }

    [HttpGet("Quizzes")]
    public async Task<ActionResult<ApiResponse<object>>> GetAllQuizzes(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var response = await _coursesGatewayService.GetAllQuizzesAsync(page, pageSize);
        return StatusCode(response.Success ? 200 : 500, response);
    }

    [HttpGet("lessons/{lessonId}/quizzes")]
    public async Task<ActionResult<ApiResponse<object>>> GetQuizByLesson(string lessonId)
    {
        var response = await _coursesGatewayService.GetQuizByLessonAsync(lessonId);
        return StatusCode(response.Success ? 200 : 404, response);
    }

    [HttpGet("Quizzes/{id}")]
    public async Task<ActionResult<ApiResponse<object>>> GetQuizById(string id)
    {
        var response = await _coursesGatewayService.GetQuizByIdAsync(id);
        return StatusCode(response.Success ? 200 : 404, response);
    }

    [HttpPost("quizzes")]
    public async Task<ActionResult<ApiResponse<object>>> CreateQuiz([FromBody] object quiz)
    {
        var response = await _coursesGatewayService.CreateQuizAsync(quiz);
        return StatusCode(response.Success ? 201 : 400, response);
    }

    [HttpPut("quizzes/{id}")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateQuiz(string id, [FromBody] object quiz)
    {
        var response = await _coursesGatewayService.UpdateQuizAsync(id, quiz);
        return StatusCode(response.Success ? 200 : 404, response);
    }

    [HttpDelete("quizzes/{id}")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteQuiz(string id)
    {
        var response = await _coursesGatewayService.DeleteQuizAsync(id);
        return StatusCode(response.Success ? 200 : 404, response);
    }

    [HttpPost("lessons/{lessonId}/quizzes/upload")]
    public async Task<ActionResult<ApiResponse<object>>> UploadQuizFile(
        string lessonId,
        IFormFile file,
        [FromForm] int quizScore = 100)
    {
        try
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("No file uploaded"));
            }

            var response = await _coursesGatewayService.UploadQuizFileAsync(lessonId, file, quizScore);
            return StatusCode(response.Success ? 200 : 400, response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading quiz file through gateway");
            return StatusCode(500, ApiResponse<object>.ErrorResponse($"Error uploading quiz file: {ex.Message}"));
        }
    }
}





