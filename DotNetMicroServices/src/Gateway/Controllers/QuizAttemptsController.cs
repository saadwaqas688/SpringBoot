using Microsoft.AspNetCore.Mvc;
using Gateway.Services;
using Shared.Common;

namespace Gateway.Controllers;

[ApiController]
[Route("api")]
public class QuizAttemptsController : ControllerBase
{
    private readonly ICoursesGatewayService _coursesGatewayService;
    private readonly ILogger<QuizAttemptsController> _logger;

    public QuizAttemptsController(ICoursesGatewayService coursesGatewayService, ILogger<QuizAttemptsController> logger)
    {
        _coursesGatewayService = coursesGatewayService;
        _logger = logger;
    }

    [HttpPost("quiz-attempts")]
    public async Task<ActionResult<ApiResponse<object>>> CreateAttempt([FromBody] object dto)
    {
        var response = await _coursesGatewayService.CreateAttemptAsync(dto);
        return StatusCode(response.Success ? 201 : 400, response);
    }

    [HttpGet("quiz-attempts/{attemptId}")]
    public async Task<ActionResult<ApiResponse<object>>> GetAttemptById(string attemptId)
    {
        var response = await _coursesGatewayService.GetAttemptByIdAsync(attemptId);
        return StatusCode(response.Success ? 200 : 404, response);
    }

    [HttpPost("quiz-attempts/{attemptId}/answers")]
    public async Task<ActionResult<ApiResponse<object>>> AddAnswer(string attemptId, [FromBody] object dto)
    {
        var response = await _coursesGatewayService.AddAnswerAsync(attemptId, dto);
        return StatusCode(response.Success ? 200 : 400, response);
    }

    [HttpPost("quiz-attempts/{attemptId}/submit")]
    public async Task<ActionResult<ApiResponse<object>>> SubmitAttempt(string attemptId)
    {
        var response = await _coursesGatewayService.SubmitAttemptAsync(attemptId);
        return StatusCode(response.Success ? 200 : 400, response);
    }

    [HttpGet("quizzes/{quizId}/user/{userId}/attempts")]
    public async Task<ActionResult<ApiResponse<List<object>>>> GetUserAttempts(string quizId, string userId)
    {
        var response = await _coursesGatewayService.GetUserAttemptsAsync(quizId, userId);
        return StatusCode(response.Success ? 200 : 500, response);
    }

    [HttpGet("quizzes/{quizId}/results/{userId}")]
    public async Task<ActionResult<ApiResponse<object>>> GetQuizResults(string quizId, string userId)
    {
        var response = await _coursesGatewayService.GetQuizResultsAsync(quizId, userId);
        return StatusCode(response.Success ? 200 : 404, response);
    }
}





