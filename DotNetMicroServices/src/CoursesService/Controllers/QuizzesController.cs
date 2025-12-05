using Microsoft.AspNetCore.Mvc;
using CoursesService.Models;
using CoursesService.Repositories;
using CoursesService.Services;
using Shared.Core.Common;
using System.Linq;

namespace CoursesService.Controllers;

[ApiController]
[Route("api")]
public class QuizzesController : ControllerBase
{
    private readonly IQuizRepository _quizRepository;
    private readonly IQuizQuestionRepository _questionRepository;
    private readonly IQuizFileParserService _fileParserService;
    private readonly ILogger<QuizzesController> _logger;

    public QuizzesController(
        IQuizRepository quizRepository,
        IQuizQuestionRepository questionRepository,
        IQuizFileParserService fileParserService,
        ILogger<QuizzesController> logger)
    {
        _quizRepository = quizRepository;
        _questionRepository = questionRepository;
        _fileParserService = fileParserService;
        _logger = logger;
    }

    [HttpGet("Quizzes")]
    public async Task<ActionResult<ApiResponse<PagedResponse<object>>>> GetAllQuizzes(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        try
        {
            var pagedQuizzes = await _quizRepository.GetAllWithQuestionCountAsync(page, pageSize);
            return Ok(ApiResponse<PagedResponse<object>>.SuccessResponse(
                new PagedResponse<object>
                {
                    Items = pagedQuizzes.Items.Cast<object>().ToList(),
                    PageNumber = pagedQuizzes.PageNumber,
                    PageSize = pagedQuizzes.PageSize,
                    TotalCount = pagedQuizzes.TotalCount
                },
                "Quizzes retrieved successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all quizzes");
            return StatusCode(500, ApiResponse<PagedResponse<object>>.ErrorResponse("An error occurred while retrieving quizzes"));
        }
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

    [HttpPost("lessons/{lessonId}/quizzes/upload")]
    public async Task<ActionResult<ApiResponse<QuizUploadResponse>>> UploadQuizFile(
        string lessonId,
        IFormFile file,
        [FromForm] int quizScore = 100)
    {
        try
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(ApiResponse<QuizUploadResponse>.ErrorResponse("No file uploaded"));
            }

            // Validate file type
            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
            var allowedExtensions = new[] { ".csv", ".xlsx", ".xls", ".xlsm" };
            if (!allowedExtensions.Contains(fileExtension))
            {
                return BadRequest(ApiResponse<QuizUploadResponse>.ErrorResponse(
                    "Invalid file type. Only CSV and Excel files (.csv, .xlsx, .xls, .xlsm) are allowed."));
            }

            // Validate file size (50MB max)
            const long maxFileSize = 50 * 1024 * 1024; // 50MB
            if (file.Length > maxFileSize)
            {
                return BadRequest(ApiResponse<QuizUploadResponse>.ErrorResponse("File size exceeds 50MB limit"));
            }

            // Parse the file
            List<QuizQuestionData> questionData;
            using (var stream = file.OpenReadStream())
            {
                questionData = await _fileParserService.ParseFileAsync(stream, file.FileName);
            }

            if (questionData.Count == 0)
            {
                return BadRequest(ApiResponse<QuizUploadResponse>.ErrorResponse(
                    "No valid questions found in the file. Please check the file format."));
            }

            // Get or create quiz for the lesson
            var existingQuiz = await _quizRepository.GetByLessonIdAsync(lessonId);
            Quiz quiz;

            if (existingQuiz != null)
            {
                // Update existing quiz
                existingQuiz.Title = $"Quiz - {file.FileName}";
                existingQuiz.Description = $"Quiz uploaded from {file.FileName}";
                quiz = await _quizRepository.UpdateAsync(existingQuiz.Id!, existingQuiz) ?? existingQuiz;
                
                // Delete existing questions
                var existingQuestions = await _questionRepository.GetByQuizIdAsync(quiz.Id!);
                foreach (var question in existingQuestions)
                {
                    if (question.Id != null)
                    {
                        await _questionRepository.DeleteAsync(question.Id);
                    }
                }
            }
            else
            {
                // Create new quiz
                quiz = new Quiz
                {
                    LessonId = lessonId,
                    Title = $"Quiz - {file.FileName}",
                    Description = $"Quiz uploaded from {file.FileName}"
                };
                quiz = await _quizRepository.CreateAsync(quiz);
            }

            // Create quiz questions
            var createdQuestions = new List<QuizQuestion>();
            for (int i = 0; i < questionData.Count; i++)
            {
                var qData = questionData[i];
                var question = new QuizQuestion
                {
                    QuizId = quiz.Id!,
                    Question = qData.Question,
                    Order = qData.Order,
                    Options = qData.Options.Select(opt => new QuizOption
                    {
                        Value = opt.Value,
                        IsCorrect = opt.IsCorrect
                    }).ToList()
                };
                var created = await _questionRepository.CreateAsync(question);
                createdQuestions.Add(created);
            }

            var response = new QuizUploadResponse
            {
                QuizId = quiz.Id!,
                LessonId = lessonId,
                QuestionsCount = createdQuestions.Count,
                QuizScore = quizScore,
                Message = $"Successfully uploaded {createdQuestions.Count} questions to quiz"
            };

            _logger.LogInformation("Quiz uploaded successfully for lesson {LessonId}. {Count} questions created.", 
                lessonId, createdQuestions.Count);

            return Ok(ApiResponse<QuizUploadResponse>.SuccessResponse(response, response.Message));
        }
        catch (NotSupportedException ex)
        {
            _logger.LogWarning(ex, "Unsupported file type for quiz upload");
            return BadRequest(ApiResponse<QuizUploadResponse>.ErrorResponse(ex.Message));
        }
        catch (InvalidDataException ex)
        {
            _logger.LogWarning(ex, "Invalid file data for quiz upload");
            return BadRequest(ApiResponse<QuizUploadResponse>.ErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading quiz file for lesson {LessonId}", lessonId);
            return StatusCode(500, ApiResponse<QuizUploadResponse>.ErrorResponse(
                $"An error occurred while uploading quiz: {ex.Message}"));
        }
    }
}

public class QuizUploadResponse
{
    public string QuizId { get; set; } = string.Empty;
    public string LessonId { get; set; } = string.Empty;
    public int QuestionsCount { get; set; }
    public int QuizScore { get; set; }
    public string Message { get; set; } = string.Empty;
}



