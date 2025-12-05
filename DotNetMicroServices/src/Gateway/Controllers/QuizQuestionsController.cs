using Microsoft.AspNetCore.Mvc;
using Gateway.Services;
using Shared.Common;

namespace Gateway.Controllers;

[ApiController]
[Route("api")]
public class QuizQuestionsController : ControllerBase
{
    private readonly ICoursesGatewayService _coursesGatewayService;
    private readonly ILogger<QuizQuestionsController> _logger;

    public QuizQuestionsController(ICoursesGatewayService coursesGatewayService, ILogger<QuizQuestionsController> logger)
    {
        _coursesGatewayService = coursesGatewayService;
        _logger = logger;
    }

    [HttpGet("quizzes/{quizId}/questions")]
    public async Task<ActionResult<ApiResponse<List<object>>>> GetQuestionsByQuiz(string quizId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var response = await _coursesGatewayService.GetQuestionsByQuizAsync(quizId, page, pageSize);
        return StatusCode(response.Success ? 200 : 500, response);
    }

    [HttpGet("quiz-questions/{id}")]
    public async Task<ActionResult<ApiResponse<object>>> GetQuestionById(string id)
    {
        var response = await _coursesGatewayService.GetQuestionByIdAsync(id);
        return StatusCode(response.Success ? 200 : 404, response);
    }

    [HttpPost("quiz-questions")]
    public async Task<ActionResult<ApiResponse<object>>> CreateQuestion([FromBody] object question)
    {
        var response = await _coursesGatewayService.CreateQuestionAsync(question);
        return StatusCode(response.Success ? 201 : 400, response);
    }

    [HttpPut("quiz-questions/{id}")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateQuestion(string id, [FromBody] object question)
    {
        var response = await _coursesGatewayService.UpdateQuestionAsync(id, question);
        return StatusCode(response.Success ? 200 : 404, response);
    }

    [HttpDelete("quiz-questions/{id}")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteQuestion(string id)
    {
        var response = await _coursesGatewayService.DeleteQuestionAsync(id);
        return StatusCode(response.Success ? 200 : 404, response);
    }
}














