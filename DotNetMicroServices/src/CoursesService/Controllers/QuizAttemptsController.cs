using Microsoft.AspNetCore.Mvc;
using CoursesService.Models;
using CoursesService.Repositories;
using Shared.Common;
using MongoDB.Driver;

namespace CoursesService.Controllers;

[ApiController]
[Route("api")]
public class QuizAttemptsController : ControllerBase
{
    private readonly IUserQuizAttemptRepository _attemptRepository;
    private readonly IUserQuizAnswerRepository _answerRepository;
    private readonly IQuizQuestionRepository _questionRepository;
    private readonly ILogger<QuizAttemptsController> _logger;

    public QuizAttemptsController(
        IUserQuizAttemptRepository attemptRepository,
        IUserQuizAnswerRepository answerRepository,
        IQuizQuestionRepository questionRepository,
        ILogger<QuizAttemptsController> logger)
    {
        _attemptRepository = attemptRepository;
        _answerRepository = answerRepository;
        _questionRepository = questionRepository;
        _logger = logger;
    }

    [HttpPost("quiz-attempts")]
    public async Task<ActionResult<ApiResponse<UserQuizAttempt>>> CreateAttempt([FromBody] CreateAttemptDto dto)
    {
        try
        {
            var attempt = new UserQuizAttempt
            {
                UserId = dto.UserId,
                QuizId = dto.QuizId,
                AttemptedAt = DateTime.UtcNow
            };
            var created = await _attemptRepository.CreateAsync(attempt);
            return CreatedAtAction(nameof(GetAttemptById), new { attemptId = created.Id },
                ApiResponse<UserQuizAttempt>.SuccessResponse(created, "Quiz attempt created successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating quiz attempt");
            return StatusCode(500, ApiResponse<UserQuizAttempt>.ErrorResponse("An error occurred while creating quiz attempt"));
        }
    }

    [HttpGet("quiz-attempts/{attemptId}")]
    public async Task<ActionResult<ApiResponse<QuizAttemptResponseDto>>> GetAttemptById(string attemptId)
    {
        try
        {
            var attempt = await _attemptRepository.GetByIdAsync(attemptId);
            if (attempt == null)
            {
                return NotFound(ApiResponse<QuizAttemptResponseDto>.ErrorResponse("Attempt not found"));
            }

            var answers = await _answerRepository.GetByAttemptIdAsync(attemptId);
            var response = new QuizAttemptResponseDto
            {
                Attempt = attempt,
                Answers = answers.ToList()
            };

            return Ok(ApiResponse<QuizAttemptResponseDto>.SuccessResponse(response, "Attempt retrieved successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving attempt {AttemptId}", attemptId);
            return StatusCode(500, ApiResponse<QuizAttemptResponseDto>.ErrorResponse("An error occurred while retrieving attempt"));
        }
    }

    [HttpPost("quiz-attempts/{attemptId}/answers")]
    public async Task<ActionResult<ApiResponse<UserQuizAnswer>>> AddAnswer(string attemptId, [FromBody] AddAnswerDto dto)
    {
        try
        {
            var question = await _questionRepository.GetByIdAsync(dto.QuestionId);
            if (question == null)
            {
                return NotFound(ApiResponse<UserQuizAnswer>.ErrorResponse("Question not found"));
            }

            var correctOption = question.Options.FirstOrDefault(o => o.IsCorrect);
            var isCorrect = correctOption != null && correctOption.Value == dto.SelectedOption;

            var answer = new UserQuizAnswer
            {
                AttemptId = attemptId,
                QuestionId = dto.QuestionId,
                SelectedOption = dto.SelectedOption,
                IsCorrect = isCorrect
            };

            var created = await _answerRepository.CreateAsync(answer);
            return Ok(ApiResponse<UserQuizAnswer>.SuccessResponse(created, "Answer added successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding answer");
            return StatusCode(500, ApiResponse<UserQuizAnswer>.ErrorResponse("An error occurred while adding answer"));
        }
    }

    [HttpPost("quiz-attempts/{attemptId}/submit")]
    public async Task<ActionResult<ApiResponse<QuizResultDto>>> SubmitAttempt(string attemptId)
    {
        try
        {
            var attempt = await _attemptRepository.GetByIdAsync(attemptId);
            if (attempt == null)
            {
                return NotFound(ApiResponse<QuizResultDto>.ErrorResponse("Attempt not found"));
            }

            var answers = await _answerRepository.GetByAttemptIdAsync(attemptId);
            var questions = await _questionRepository.GetByQuizIdAsync(attempt.QuizId);

            var correctCount = answers.Count(a => a.IsCorrect);
            var totalCount = questions.Count();
            var score = totalCount > 0 ? (int)((correctCount / (double)totalCount) * 100) : 0;

            attempt.CorrectAnswers = correctCount;
            attempt.TotalQuestions = totalCount;
            attempt.Score = score;

            var updated = await _attemptRepository.UpdateAsync(attemptId, attempt);

            var result = new QuizResultDto
            {
                Score = score,
                CorrectAnswers = correctCount,
                WrongAnswers = totalCount - correctCount,
                TotalQuestions = totalCount
            };

            return Ok(ApiResponse<QuizResultDto>.SuccessResponse(result, "Quiz submitted successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error submitting attempt {AttemptId}", attemptId);
            return StatusCode(500, ApiResponse<QuizResultDto>.ErrorResponse("An error occurred while submitting quiz"));
        }
    }

    [HttpGet("quizzes/{quizId}/user/{userId}/attempts")]
    public async Task<ActionResult<ApiResponse<List<UserQuizAttempt>>>> GetUserAttempts(string quizId, string userId)
    {
        try
        {
            var attempts = await _attemptRepository.GetByUserIdAsync(userId);
            var filtered = attempts.Where(a => a.QuizId == quizId).ToList();
            return Ok(ApiResponse<List<UserQuizAttempt>>.SuccessResponse(filtered, "Attempts retrieved successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving attempts for user {UserId} and quiz {QuizId}", userId, quizId);
            return StatusCode(500, ApiResponse<List<UserQuizAttempt>>.ErrorResponse("An error occurred while retrieving attempts"));
        }
    }

    [HttpGet("quizzes/{quizId}/results/{userId}")]
    public async Task<ActionResult<ApiResponse<QuizResultDto>>> GetQuizResults(string quizId, string userId)
    {
        try
        {
            var attempt = await _attemptRepository.GetLatestByUserAndQuizAsync(userId, quizId);
            if (attempt == null)
            {
                return NotFound(ApiResponse<QuizResultDto>.ErrorResponse("No attempt found"));
            }

            var result = new QuizResultDto
            {
                Score = attempt.Score,
                CorrectAnswers = attempt.CorrectAnswers,
                WrongAnswers = attempt.TotalQuestions - attempt.CorrectAnswers,
                TotalQuestions = attempt.TotalQuestions
            };

            return Ok(ApiResponse<QuizResultDto>.SuccessResponse(result, "Results retrieved successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving results for user {UserId} and quiz {QuizId}", userId, quizId);
            return StatusCode(500, ApiResponse<QuizResultDto>.ErrorResponse("An error occurred while retrieving results"));
        }
    }
}

// DTOs
public class CreateAttemptDto
{
    public string UserId { get; set; } = string.Empty;
    public string QuizId { get; set; } = string.Empty;
}

public class AddAnswerDto
{
    public string QuestionId { get; set; } = string.Empty;
    public string SelectedOption { get; set; } = string.Empty;
}

public class QuizAttemptResponseDto
{
    public UserQuizAttempt Attempt { get; set; } = null!;
    public List<UserQuizAnswer> Answers { get; set; } = new();
}

public class QuizResultDto
{
    public int Score { get; set; }
    public int CorrectAnswers { get; set; }
    public int WrongAnswers { get; set; }
    public int TotalQuestions { get; set; }
}


