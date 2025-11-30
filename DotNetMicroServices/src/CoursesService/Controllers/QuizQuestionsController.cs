using Microsoft.AspNetCore.Mvc;
using CoursesService.Models;
using CoursesService.Repositories;
using Shared.Common;

namespace CoursesService.Controllers;

[ApiController]
[Route("api")]
public class QuizQuestionsController : ControllerBase
{
    private readonly IQuizQuestionRepository _questionRepository;
    private readonly ILogger<QuizQuestionsController> _logger;

    public QuizQuestionsController(IQuizQuestionRepository questionRepository, ILogger<QuizQuestionsController> logger)
    {
        _questionRepository = questionRepository;
        _logger = logger;
    }

    [HttpGet("quizzes/{quizId}/questions")]
    public async Task<ActionResult<ApiResponse<List<QuizQuestion>>>> GetQuestionsByQuiz(string quizId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        try
        {
            var questions = await _questionRepository.GetByQuizIdAsync(quizId);
            var pagedQuestions = questions.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            return Ok(ApiResponse<List<QuizQuestion>>.SuccessResponse(pagedQuestions, "Questions retrieved successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving questions for quiz {QuizId}", quizId);
            return StatusCode(500, ApiResponse<List<QuizQuestion>>.ErrorResponse("An error occurred while retrieving questions"));
        }
    }

    [HttpGet("quiz-questions/{id}")]
    public async Task<ActionResult<ApiResponse<QuizQuestion>>> GetQuestionById(string id)
    {
        try
        {
            var question = await _questionRepository.GetByIdAsync(id);
            if (question == null)
            {
                return NotFound(ApiResponse<QuizQuestion>.ErrorResponse("Question not found"));
            }
            return Ok(ApiResponse<QuizQuestion>.SuccessResponse(question, "Question retrieved successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving question {QuestionId}", id);
            return StatusCode(500, ApiResponse<QuizQuestion>.ErrorResponse("An error occurred while retrieving question"));
        }
    }

    [HttpPost("quiz-questions")]
    public async Task<ActionResult<ApiResponse<QuizQuestion>>> CreateQuestion([FromBody] QuizQuestion question)
    {
        try
        {
            question.Type = "quiz";
            var created = await _questionRepository.CreateAsync(question);
            return CreatedAtAction(nameof(GetQuestionById), new { id = created.Id },
                ApiResponse<QuizQuestion>.SuccessResponse(created, "Question created successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating question");
            return StatusCode(500, ApiResponse<QuizQuestion>.ErrorResponse("An error occurred while creating question"));
        }
    }

    [HttpPut("quiz-questions/{id}")]
    public async Task<ActionResult<ApiResponse<QuizQuestion>>> UpdateQuestion(string id, [FromBody] QuizQuestion question)
    {
        try
        {
            question.Id = id;
            var updated = await _questionRepository.UpdateAsync(id, question);
            if (updated == null)
            {
                return NotFound(ApiResponse<QuizQuestion>.ErrorResponse("Question not found"));
            }
            return Ok(ApiResponse<QuizQuestion>.SuccessResponse(updated, "Question updated successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating question {QuestionId}", id);
            return StatusCode(500, ApiResponse<QuizQuestion>.ErrorResponse("An error occurred while updating question"));
        }
    }

    [HttpDelete("quiz-questions/{id}")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteQuestion(string id)
    {
        try
        {
            var deleted = await _questionRepository.DeleteAsync(id);
            if (!deleted)
            {
                return NotFound(ApiResponse<bool>.ErrorResponse("Question not found"));
            }
            return Ok(ApiResponse<bool>.SuccessResponse(true, "Question deleted successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting question {QuestionId}", id);
            return StatusCode(500, ApiResponse<bool>.ErrorResponse("An error occurred while deleting question"));
        }
    }
}


