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

    [HttpGet("lessons/{lessonId}/quizzes")]
    public async Task<ActionResult<ApiResponse<object>>> GetQuizByLesson(string lessonId)
    {
        var response = await _coursesGatewayService.GetQuizByLessonAsync(lessonId);
        return StatusCode(response.Success ? 200 : 404, response);
    }

    [HttpGet("quizzes/{id}")]
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
}


