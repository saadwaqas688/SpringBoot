using Microsoft.AspNetCore.Mvc;
using CoursesService.Models;
using CoursesService.Repositories;
using Shared.Common;

namespace CoursesService.Controllers;

[ApiController]
[Route("api")]
public class QuizzesController : ControllerBase
{
    private readonly IQuizRepository _quizRepository;
    private readonly ILogger<QuizzesController> _logger;

    public QuizzesController(IQuizRepository quizRepository, ILogger<QuizzesController> logger)
    {
        _quizRepository = quizRepository;
        _logger = logger;
    }

    [HttpGet("lessons/{lessonId}/quizzes")]
    public async Task<ActionResult<ApiResponse<Quiz>>> GetQuizByLesson(string lessonId)
    {
        try
        {
            var quiz = await _quizRepository.GetByLessonIdAsync(lessonId);
            if (quiz == null)
            {
                return NotFound(ApiResponse<Quiz>.ErrorResponse("Quiz not found for this lesson"));
            }
            return Ok(ApiResponse<Quiz>.SuccessResponse(quiz, "Quiz retrieved successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving quiz for lesson {LessonId}", lessonId);
            return StatusCode(500, ApiResponse<Quiz>.ErrorResponse("An error occurred while retrieving quiz"));
        }
    }

    [HttpGet("quizzes/{id}")]
    public async Task<ActionResult<ApiResponse<Quiz>>> GetQuizById(string id)
    {
        try
        {
            var quiz = await _quizRepository.GetByIdAsync(id);
            if (quiz == null)
            {
                return NotFound(ApiResponse<Quiz>.ErrorResponse("Quiz not found"));
            }
            return Ok(ApiResponse<Quiz>.SuccessResponse(quiz, "Quiz retrieved successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving quiz {QuizId}", id);
            return StatusCode(500, ApiResponse<Quiz>.ErrorResponse("An error occurred while retrieving quiz"));
        }
    }

    [HttpPost("quizzes")]
    public async Task<ActionResult<ApiResponse<Quiz>>> CreateQuiz([FromBody] Quiz quiz)
    {
        try
        {
            var created = await _quizRepository.CreateAsync(quiz);
            return CreatedAtAction(nameof(GetQuizById), new { id = created.Id },
                ApiResponse<Quiz>.SuccessResponse(created, "Quiz created successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating quiz");
            return StatusCode(500, ApiResponse<Quiz>.ErrorResponse("An error occurred while creating quiz"));
        }
    }

    [HttpPut("quizzes/{id}")]
    public async Task<ActionResult<ApiResponse<Quiz>>> UpdateQuiz(string id, [FromBody] Quiz quiz)
    {
        try
        {
            quiz.Id = id;
            var updated = await _quizRepository.UpdateAsync(id, quiz);
            if (updated == null)
            {
                return NotFound(ApiResponse<Quiz>.ErrorResponse("Quiz not found"));
            }
            return Ok(ApiResponse<Quiz>.SuccessResponse(updated, "Quiz updated successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating quiz {QuizId}", id);
            return StatusCode(500, ApiResponse<Quiz>.ErrorResponse("An error occurred while updating quiz"));
        }
    }

    [HttpDelete("quizzes/{id}")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteQuiz(string id)
    {
        try
        {
            var deleted = await _quizRepository.DeleteAsync(id);
            if (!deleted)
            {
                return NotFound(ApiResponse<bool>.ErrorResponse("Quiz not found"));
            }
            return Ok(ApiResponse<bool>.SuccessResponse(true, "Quiz deleted successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting quiz {QuizId}", id);
            return StatusCode(500, ApiResponse<bool>.ErrorResponse("An error occurred while deleting quiz"));
        }
    }
}


