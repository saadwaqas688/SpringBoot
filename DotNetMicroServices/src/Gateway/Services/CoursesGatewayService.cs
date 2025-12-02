using Shared.Common;
using Shared.Constants;
using Shared.Services;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;

namespace Gateway.Services;

public class CoursesGatewayService : ICoursesGatewayService
{
    private readonly IRabbitMQService _rabbitMQService;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<CoursesGatewayService> _logger;
    private readonly string _coursesServiceUrl;

    public CoursesGatewayService(
        IRabbitMQService rabbitMQService,
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        ILogger<CoursesGatewayService> logger)
    {
        _rabbitMQService = rabbitMQService;
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        
        // Get CoursesService URL from configuration, with fallbacks
        // Priority: Configuration > Environment Variable > Default
        _coursesServiceUrl = configuration["ServiceUrls:CoursesService"] 
            ?? Environment.GetEnvironmentVariable("ServiceUrls__CoursesService")
            ?? "http://localhost:5004"; // Default to localhost for local development
    }

    public async Task<ApiResponse<string>> HealthCheckAsync()
    {
        try
        {
            var response = await _rabbitMQService.SendMessageAsync<ApiResponse<string>>(
                RabbitMQConstants.CoursesServiceQueue,
                RabbitMQConstants.Courses.HealthCheck,
                new { });
            return response ?? ApiResponse<string>.ErrorResponse("Failed to get health check");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling CoursesService for health check");
            return ApiResponse<string>.ErrorResponse("An error occurred during health check");
        }
    }

    public async Task<ApiResponse<PagedResponse<object>>> GetAllCoursesAsync(int page, int pageSize)
    {
        try
        {
            var message = new { Page = page, PageSize = pageSize };
            var response = await _rabbitMQService.SendMessageAsync<ApiResponse<PagedResponse<object>>>(
                RabbitMQConstants.CoursesServiceQueue,
                RabbitMQConstants.Courses.GetAll,
                message);
            return response ?? ApiResponse<PagedResponse<object>>.ErrorResponse("Failed to get courses");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling CoursesService to get all courses");
            return ApiResponse<PagedResponse<object>>.ErrorResponse("An error occurred while retrieving courses");
        }
    }

    public async Task<ApiResponse<object>> GetCourseByIdAsync(string id)
    {
        try
        {
            var message = new { Id = id };
            var response = await _rabbitMQService.SendMessageAsync<ApiResponse<object>>(
                RabbitMQConstants.CoursesServiceQueue,
                RabbitMQConstants.Courses.GetById,
                message);
            return response ?? ApiResponse<object>.ErrorResponse("Failed to get course");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling CoursesService to get course by id: {Id}", id);
            return ApiResponse<object>.ErrorResponse("An error occurred while retrieving course");
        }
    }

    public async Task<ApiResponse<object>> CreateCourseAsync(object course)
    {
        try
        {
            var response = await _rabbitMQService.SendMessageAsync<ApiResponse<object>>(
                RabbitMQConstants.CoursesServiceQueue,
                RabbitMQConstants.Courses.Create,
                course);
            return response ?? ApiResponse<object>.ErrorResponse("Failed to create course");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling CoursesService to create course");
            return ApiResponse<object>.ErrorResponse("An error occurred while creating course");
        }
    }

    public async Task<ApiResponse<object>> UpdateCourseAsync(string id, object course)
    {
        try
        {
            var message = new { Id = id, Course = course };
            var response = await _rabbitMQService.SendMessageAsync<ApiResponse<object>>(
                RabbitMQConstants.CoursesServiceQueue,
                RabbitMQConstants.Courses.Update,
                message);
            return response ?? ApiResponse<object>.ErrorResponse("Failed to update course");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling CoursesService to update course: {Id}", id);
            return ApiResponse<object>.ErrorResponse("An error occurred while updating course");
        }
    }

    public async Task<ApiResponse<bool>> DeleteCourseAsync(string id)
    {
        try
        {
            var message = new { Id = id };
            var response = await _rabbitMQService.SendMessageAsync<ApiResponse<bool>>(
                RabbitMQConstants.CoursesServiceQueue,
                RabbitMQConstants.Courses.Delete,
                message);
            return response ?? ApiResponse<bool>.ErrorResponse("Failed to delete course");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling CoursesService to delete course: {Id}", id);
            return ApiResponse<bool>.ErrorResponse("An error occurred while deleting course");
        }
    }

    public async Task<ApiResponse<List<object>>> GetCoursesByStatusAsync(string status)
    {
        try
        {
            var message = new { Status = status };
            var response = await _rabbitMQService.SendMessageAsync<ApiResponse<List<object>>>(
                RabbitMQConstants.CoursesServiceQueue,
                RabbitMQConstants.Courses.GetByStatus,
                message);
            return response ?? ApiResponse<List<object>>.ErrorResponse("Failed to get courses by status");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling CoursesService to get courses by status: {Status}", status);
            return ApiResponse<List<object>>.ErrorResponse("An error occurred while retrieving courses");
        }
    }

    public async Task<ApiResponse<List<object>>> SearchCoursesAsync(string searchTerm)
    {
        try
        {
            var message = new { SearchTerm = searchTerm };
            var response = await _rabbitMQService.SendMessageAsync<ApiResponse<List<object>>>(
                RabbitMQConstants.CoursesServiceQueue,
                RabbitMQConstants.Courses.Search,
                message);
            return response ?? ApiResponse<List<object>>.ErrorResponse("Failed to search courses");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling CoursesService to search courses: {SearchTerm}", searchTerm);
            return ApiResponse<List<object>>.ErrorResponse("An error occurred while searching courses");
        }
    }

    // Course Assignment endpoints
    public async Task<ApiResponse<object>> AssignUserToCourseAsync(string courseId, object dto)
    {
        try
        {
            var message = new { CourseId = courseId, Dto = dto };
            var response = await _rabbitMQService.SendMessageAsync<ApiResponse<object>>(
                RabbitMQConstants.CoursesServiceQueue,
                RabbitMQConstants.Courses.AssignUser,
                message);
            return response ?? ApiResponse<object>.ErrorResponse("Failed to assign users");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error assigning users to course");
            return ApiResponse<object>.ErrorResponse("An error occurred while assigning users");
        }
    }

    public async Task<ApiResponse<bool>> RemoveUserFromCourseAsync(string courseId, string userId)
    {
        try
        {
            var message = new { CourseId = courseId, UserId = userId };
            var response = await _rabbitMQService.SendMessageAsync<ApiResponse<bool>>(
                RabbitMQConstants.CoursesServiceQueue,
                RabbitMQConstants.Courses.RemoveUser,
                message);
            return response ?? ApiResponse<bool>.ErrorResponse("Failed to remove user");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing user from course");
            return ApiResponse<bool>.ErrorResponse("An error occurred while removing user");
        }
    }

    public async Task<ApiResponse<List<object>>> GetAssignedUsersAsync(string courseId, int page, int pageSize)
    {
        try
        {
            var message = new { CourseId = courseId, Page = page, PageSize = pageSize };
            var response = await _rabbitMQService.SendMessageAsync<ApiResponse<List<object>>>(
                RabbitMQConstants.CoursesServiceQueue,
                RabbitMQConstants.Courses.GetAssignedUsers,
                message);
            return response ?? ApiResponse<List<object>>.ErrorResponse("Failed to get assigned users");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting assigned users");
            return ApiResponse<List<object>>.ErrorResponse("An error occurred while retrieving assigned users");
        }
    }

    public async Task<ApiResponse<object>> GetCourseUserProgressAsync(string courseId, string userId)
    {
        try
        {
            var message = new { CourseId = courseId, UserId = userId };
            var response = await _rabbitMQService.SendMessageAsync<ApiResponse<object>>(
                RabbitMQConstants.CoursesServiceQueue,
                RabbitMQConstants.Courses.GetCourseUserProgress,
                message);
            return response ?? ApiResponse<object>.ErrorResponse("Failed to get user progress");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user progress");
            return ApiResponse<object>.ErrorResponse("An error occurred while retrieving user progress");
        }
    }

    // Lesson endpoints
    public async Task<ApiResponse<List<object>>> GetLessonsByCourseAsync(string courseId, int page, int pageSize)
    {
        try
        {
            var message = new { CourseId = courseId, Page = page, PageSize = pageSize };
            var response = await _rabbitMQService.SendMessageAsync<ApiResponse<List<object>>>(
                RabbitMQConstants.CoursesServiceQueue,
                RabbitMQConstants.Courses.GetLessonsByCourse,
                message);
            return response ?? ApiResponse<List<object>>.ErrorResponse("Failed to get lessons");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting lessons");
            return ApiResponse<List<object>>.ErrorResponse("An error occurred while retrieving lessons");
        }
    }

    public async Task<ApiResponse<object>> GetLessonByIdAsync(string id)
    {
        try
        {
            var message = new { Id = id };
            var response = await _rabbitMQService.SendMessageAsync<ApiResponse<object>>(
                RabbitMQConstants.CoursesServiceQueue,
                RabbitMQConstants.Courses.GetLessonById,
                message);
            return response ?? ApiResponse<object>.ErrorResponse("Failed to get lesson");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting lesson");
            return ApiResponse<object>.ErrorResponse("An error occurred while retrieving lesson");
        }
    }

    public async Task<ApiResponse<object>> CreateLessonAsync(object lesson)
    {
        try
        {
            var response = await _rabbitMQService.SendMessageAsync<ApiResponse<object>>(
                RabbitMQConstants.CoursesServiceQueue,
                RabbitMQConstants.Courses.CreateLesson,
                lesson);
            return response ?? ApiResponse<object>.ErrorResponse("Failed to create lesson");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating lesson");
            return ApiResponse<object>.ErrorResponse("An error occurred while creating lesson");
        }
    }

    public async Task<ApiResponse<object>> UpdateLessonAsync(string id, object lesson)
    {
        try
        {
            var message = new { Id = id, Lesson = lesson };
            var response = await _rabbitMQService.SendMessageAsync<ApiResponse<object>>(
                RabbitMQConstants.CoursesServiceQueue,
                RabbitMQConstants.Courses.UpdateLesson,
                message);
            return response ?? ApiResponse<object>.ErrorResponse("Failed to update lesson");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating lesson");
            return ApiResponse<object>.ErrorResponse("An error occurred while updating lesson");
        }
    }

    public async Task<ApiResponse<bool>> DeleteLessonAsync(string id)
    {
        try
        {
            var message = new { Id = id };
            var response = await _rabbitMQService.SendMessageAsync<ApiResponse<bool>>(
                RabbitMQConstants.CoursesServiceQueue,
                RabbitMQConstants.Courses.DeleteLesson,
                message);
            return response ?? ApiResponse<bool>.ErrorResponse("Failed to delete lesson");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting lesson");
            return ApiResponse<bool>.ErrorResponse("An error occurred while deleting lesson");
        }
    }

    // Slide endpoints
    public async Task<ApiResponse<PagedResponse<object>>> GetSlidesByLessonAsync(string lessonId, int page, int pageSize)
    {
        try
        {
            var message = new { LessonId = lessonId, Page = page, PageSize = pageSize };
            var response = await _rabbitMQService.SendMessageAsync<ApiResponse<PagedResponse<object>>>(
                RabbitMQConstants.CoursesServiceQueue,
                RabbitMQConstants.Courses.GetSlidesByLesson,
                message);
            return response ?? ApiResponse<PagedResponse<object>>.ErrorResponse("Failed to get slides");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting slides");
            return ApiResponse<PagedResponse<object>>.ErrorResponse("An error occurred while retrieving slides");
        }
    }

    public async Task<ApiResponse<object>> GetSlideByIdAsync(string id)
    {
        try
        {
            var message = new { Id = id };
            var response = await _rabbitMQService.SendMessageAsync<ApiResponse<object>>(
                RabbitMQConstants.CoursesServiceQueue,
                RabbitMQConstants.Courses.GetSlideById,
                message);
            return response ?? ApiResponse<object>.ErrorResponse("Failed to get slide");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting slide");
            return ApiResponse<object>.ErrorResponse("An error occurred while retrieving slide");
        }
    }

    public async Task<ApiResponse<object>> CreateSlideAsync(object slide)
    {
        try
        {
            var response = await _rabbitMQService.SendMessageAsync<ApiResponse<object>>(
                RabbitMQConstants.CoursesServiceQueue,
                RabbitMQConstants.Courses.CreateSlide,
                slide);
            return response ?? ApiResponse<object>.ErrorResponse("Failed to create slide");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating slide");
            return ApiResponse<object>.ErrorResponse("An error occurred while creating slide");
        }
    }

    public async Task<ApiResponse<object>> UpdateSlideAsync(string id, object slide)
    {
        try
        {
            var message = new { Id = id, Slide = slide };
            var response = await _rabbitMQService.SendMessageAsync<ApiResponse<object>>(
                RabbitMQConstants.CoursesServiceQueue,
                RabbitMQConstants.Courses.UpdateSlide,
                message);
            return response ?? ApiResponse<object>.ErrorResponse("Failed to update slide");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating slide");
            return ApiResponse<object>.ErrorResponse("An error occurred while updating slide");
        }
    }

    public async Task<ApiResponse<bool>> DeleteSlideAsync(string id)
    {
        try
        {
            var message = new { Id = id };
            var response = await _rabbitMQService.SendMessageAsync<ApiResponse<bool>>(
                RabbitMQConstants.CoursesServiceQueue,
                RabbitMQConstants.Courses.DeleteSlide,
                message);
            return response ?? ApiResponse<bool>.ErrorResponse("Failed to delete slide");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting slide");
            return ApiResponse<bool>.ErrorResponse("An error occurred while deleting slide");
        }
    }

    // Discussion endpoints
    public async Task<ApiResponse<List<object>>> GetPostsByLessonAsync(string lessonId, int page, int pageSize)
    {
        try
        {
            var message = new { LessonId = lessonId, Page = page, PageSize = pageSize };
            var response = await _rabbitMQService.SendMessageAsync<ApiResponse<List<object>>>(
                RabbitMQConstants.CoursesServiceQueue,
                RabbitMQConstants.Courses.GetPostsByLesson,
                message);
            return response ?? ApiResponse<List<object>>.ErrorResponse("Failed to get posts");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting posts");
            return ApiResponse<List<object>>.ErrorResponse("An error occurred while retrieving posts");
        }
    }

    public async Task<ApiResponse<object>> GetPostByIdAsync(string id)
    {
        try
        {
            var message = new { Id = id };
            var response = await _rabbitMQService.SendMessageAsync<ApiResponse<object>>(
                RabbitMQConstants.CoursesServiceQueue,
                RabbitMQConstants.Courses.GetPostById,
                message);
            return response ?? ApiResponse<object>.ErrorResponse("Failed to get post");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting post");
            return ApiResponse<object>.ErrorResponse("An error occurred while retrieving post");
        }
    }

    public async Task<ApiResponse<object>> CreatePostAsync(object post)
    {
        try
        {
            var response = await _rabbitMQService.SendMessageAsync<ApiResponse<object>>(
                RabbitMQConstants.CoursesServiceQueue,
                RabbitMQConstants.Courses.CreatePost,
                post);
            return response ?? ApiResponse<object>.ErrorResponse("Failed to create post");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating post");
            return ApiResponse<object>.ErrorResponse("An error occurred while creating post");
        }
    }

    public async Task<ApiResponse<object>> UpdatePostAsync(string id, object post)
    {
        try
        {
            var message = new { Id = id, Post = post };
            var response = await _rabbitMQService.SendMessageAsync<ApiResponse<object>>(
                RabbitMQConstants.CoursesServiceQueue,
                RabbitMQConstants.Courses.UpdatePost,
                message);
            return response ?? ApiResponse<object>.ErrorResponse("Failed to update post");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating post");
            return ApiResponse<object>.ErrorResponse("An error occurred while updating post");
        }
    }

    public async Task<ApiResponse<bool>> DeletePostAsync(string id)
    {
        try
        {
            var message = new { Id = id };
            var response = await _rabbitMQService.SendMessageAsync<ApiResponse<bool>>(
                RabbitMQConstants.CoursesServiceQueue,
                RabbitMQConstants.Courses.DeletePost,
                message);
            return response ?? ApiResponse<bool>.ErrorResponse("Failed to delete post");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting post");
            return ApiResponse<bool>.ErrorResponse("An error occurred while deleting post");
        }
    }

    public async Task<ApiResponse<List<object>>> GetCommentsAsync(string postId, int page, int pageSize)
    {
        try
        {
            var message = new { PostId = postId, Page = page, PageSize = pageSize };
            var response = await _rabbitMQService.SendMessageAsync<ApiResponse<List<object>>>(
                RabbitMQConstants.CoursesServiceQueue,
                RabbitMQConstants.Courses.GetComments,
                message);
            return response ?? ApiResponse<List<object>>.ErrorResponse("Failed to get comments");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting comments");
            return ApiResponse<List<object>>.ErrorResponse("An error occurred while retrieving comments");
        }
    }

    // Quiz endpoints
    public async Task<ApiResponse<PagedResponse<object>>> GetAllQuizzesAsync(int page, int pageSize)
    {
        try
        {
            var httpClient = _httpClientFactory.CreateClient();
            var url = $"{_coursesServiceUrl}/api/Quizzes?page={page}&pageSize={pageSize}";
            
            _logger.LogInformation("Calling CoursesService to get all quizzes: {Url}", url);
            
            var response = await httpClient.GetAsync(url);
            var content = await response.Content.ReadAsStringAsync();
            
            if (response.IsSuccessStatusCode)
            {
                var result = JsonSerializer.Deserialize<ApiResponse<PagedResponse<object>>>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                return result ?? ApiResponse<PagedResponse<object>>.ErrorResponse("Failed to parse quizzes response");
            }
            
            _logger.LogWarning("Failed to get quizzes. Status: {StatusCode}, Response: {Content}", response.StatusCode, content);
            return ApiResponse<PagedResponse<object>>.ErrorResponse($"Failed to get quizzes: {response.StatusCode}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling CoursesService to get all quizzes");
            return ApiResponse<PagedResponse<object>>.ErrorResponse("An error occurred while retrieving quizzes");
        }
    }

    public async Task<ApiResponse<object>> GetQuizByLessonAsync(string lessonId)
    {
        try
        {
            var message = new { LessonId = lessonId };
            var response = await _rabbitMQService.SendMessageAsync<ApiResponse<object>>(
                RabbitMQConstants.CoursesServiceQueue,
                RabbitMQConstants.Courses.GetQuizByLesson,
                message);
            return response ?? ApiResponse<object>.ErrorResponse("Failed to get quiz");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting quiz");
            return ApiResponse<object>.ErrorResponse("An error occurred while retrieving quiz");
        }
    }

    public async Task<ApiResponse<object>> GetQuizByIdAsync(string id)
    {
        try
        {
            var message = new { Id = id };
            var response = await _rabbitMQService.SendMessageAsync<ApiResponse<object>>(
                RabbitMQConstants.CoursesServiceQueue,
                RabbitMQConstants.Courses.GetQuizById,
                message);
            return response ?? ApiResponse<object>.ErrorResponse("Failed to get quiz");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting quiz");
            return ApiResponse<object>.ErrorResponse("An error occurred while retrieving quiz");
        }
    }

    public async Task<ApiResponse<object>> CreateQuizAsync(object quiz)
    {
        try
        {
            var response = await _rabbitMQService.SendMessageAsync<ApiResponse<object>>(
                RabbitMQConstants.CoursesServiceQueue,
                RabbitMQConstants.Courses.CreateQuiz,
                quiz);
            return response ?? ApiResponse<object>.ErrorResponse("Failed to create quiz");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating quiz");
            return ApiResponse<object>.ErrorResponse("An error occurred while creating quiz");
        }
    }

    public async Task<ApiResponse<object>> UpdateQuizAsync(string id, object quiz)
    {
        try
        {
            var message = new { Id = id, Quiz = quiz };
            var response = await _rabbitMQService.SendMessageAsync<ApiResponse<object>>(
                RabbitMQConstants.CoursesServiceQueue,
                RabbitMQConstants.Courses.UpdateQuiz,
                message);
            return response ?? ApiResponse<object>.ErrorResponse("Failed to update quiz");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating quiz");
            return ApiResponse<object>.ErrorResponse("An error occurred while updating quiz");
        }
    }

    public async Task<ApiResponse<bool>> DeleteQuizAsync(string id)
    {
        try
        {
            var message = new { Id = id };
            var response = await _rabbitMQService.SendMessageAsync<ApiResponse<bool>>(
                RabbitMQConstants.CoursesServiceQueue,
                RabbitMQConstants.Courses.DeleteQuiz,
                message);
            return response ?? ApiResponse<bool>.ErrorResponse("Failed to delete quiz");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting quiz");
            return ApiResponse<bool>.ErrorResponse("An error occurred while deleting quiz");
        }
    }

    // Quiz Question endpoints
    public async Task<ApiResponse<List<object>>> GetQuestionsByQuizAsync(string quizId, int page, int pageSize)
    {
        try
        {
            var message = new { QuizId = quizId, Page = page, PageSize = pageSize };
            var response = await _rabbitMQService.SendMessageAsync<ApiResponse<List<object>>>(
                RabbitMQConstants.CoursesServiceQueue,
                RabbitMQConstants.Courses.GetQuestionsByQuiz,
                message);
            return response ?? ApiResponse<List<object>>.ErrorResponse("Failed to get questions");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting questions");
            return ApiResponse<List<object>>.ErrorResponse("An error occurred while retrieving questions");
        }
    }

    public async Task<ApiResponse<object>> GetQuestionByIdAsync(string id)
    {
        try
        {
            var message = new { Id = id };
            var response = await _rabbitMQService.SendMessageAsync<ApiResponse<object>>(
                RabbitMQConstants.CoursesServiceQueue,
                RabbitMQConstants.Courses.GetQuestionById,
                message);
            return response ?? ApiResponse<object>.ErrorResponse("Failed to get question");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting question");
            return ApiResponse<object>.ErrorResponse("An error occurred while retrieving question");
        }
    }

    public async Task<ApiResponse<object>> CreateQuestionAsync(object question)
    {
        try
        {
            var response = await _rabbitMQService.SendMessageAsync<ApiResponse<object>>(
                RabbitMQConstants.CoursesServiceQueue,
                RabbitMQConstants.Courses.CreateQuestion,
                question);
            return response ?? ApiResponse<object>.ErrorResponse("Failed to create question");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating question");
            return ApiResponse<object>.ErrorResponse("An error occurred while creating question");
        }
    }

    public async Task<ApiResponse<object>> UpdateQuestionAsync(string id, object question)
    {
        try
        {
            var message = new { Id = id, Question = question };
            var response = await _rabbitMQService.SendMessageAsync<ApiResponse<object>>(
                RabbitMQConstants.CoursesServiceQueue,
                RabbitMQConstants.Courses.UpdateQuestion,
                message);
            return response ?? ApiResponse<object>.ErrorResponse("Failed to update question");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating question");
            return ApiResponse<object>.ErrorResponse("An error occurred while updating question");
        }
    }

    public async Task<ApiResponse<bool>> DeleteQuestionAsync(string id)
    {
        try
        {
            var message = new { Id = id };
            var response = await _rabbitMQService.SendMessageAsync<ApiResponse<bool>>(
                RabbitMQConstants.CoursesServiceQueue,
                RabbitMQConstants.Courses.DeleteQuestion,
                message);
            return response ?? ApiResponse<bool>.ErrorResponse("Failed to delete question");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting question");
            return ApiResponse<bool>.ErrorResponse("An error occurred while deleting question");
        }
    }

    // Quiz Attempt endpoints
    public async Task<ApiResponse<object>> CreateAttemptAsync(object dto)
    {
        try
        {
            var response = await _rabbitMQService.SendMessageAsync<ApiResponse<object>>(
                RabbitMQConstants.CoursesServiceQueue,
                RabbitMQConstants.Courses.CreateAttempt,
                dto);
            return response ?? ApiResponse<object>.ErrorResponse("Failed to create attempt");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating attempt");
            return ApiResponse<object>.ErrorResponse("An error occurred while creating attempt");
        }
    }

    public async Task<ApiResponse<object>> GetAttemptByIdAsync(string attemptId)
    {
        try
        {
            var message = new { AttemptId = attemptId };
            var response = await _rabbitMQService.SendMessageAsync<ApiResponse<object>>(
                RabbitMQConstants.CoursesServiceQueue,
                RabbitMQConstants.Courses.GetAttemptById,
                message);
            return response ?? ApiResponse<object>.ErrorResponse("Failed to get attempt");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting attempt");
            return ApiResponse<object>.ErrorResponse("An error occurred while retrieving attempt");
        }
    }

    public async Task<ApiResponse<object>> AddAnswerAsync(string attemptId, object dto)
    {
        try
        {
            var message = new { AttemptId = attemptId, Dto = dto };
            var response = await _rabbitMQService.SendMessageAsync<ApiResponse<object>>(
                RabbitMQConstants.CoursesServiceQueue,
                RabbitMQConstants.Courses.AddAnswer,
                message);
            return response ?? ApiResponse<object>.ErrorResponse("Failed to add answer");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding answer");
            return ApiResponse<object>.ErrorResponse("An error occurred while adding answer");
        }
    }

    public async Task<ApiResponse<object>> SubmitAttemptAsync(string attemptId)
    {
        try
        {
            var message = new { AttemptId = attemptId };
            var response = await _rabbitMQService.SendMessageAsync<ApiResponse<object>>(
                RabbitMQConstants.CoursesServiceQueue,
                RabbitMQConstants.Courses.SubmitAttempt,
                message);
            return response ?? ApiResponse<object>.ErrorResponse("Failed to submit attempt");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error submitting attempt");
            return ApiResponse<object>.ErrorResponse("An error occurred while submitting attempt");
        }
    }

    public async Task<ApiResponse<List<object>>> GetUserAttemptsAsync(string quizId, string userId)
    {
        try
        {
            var message = new { QuizId = quizId, UserId = userId };
            var response = await _rabbitMQService.SendMessageAsync<ApiResponse<List<object>>>(
                RabbitMQConstants.CoursesServiceQueue,
                RabbitMQConstants.Courses.GetUserAttempts,
                message);
            return response ?? ApiResponse<List<object>>.ErrorResponse("Failed to get attempts");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting attempts");
            return ApiResponse<List<object>>.ErrorResponse("An error occurred while retrieving attempts");
        }
    }

    public async Task<ApiResponse<object>> GetQuizResultsAsync(string quizId, string userId)
    {
        try
        {
            var message = new { QuizId = quizId, UserId = userId };
            var response = await _rabbitMQService.SendMessageAsync<ApiResponse<object>>(
                RabbitMQConstants.CoursesServiceQueue,
                RabbitMQConstants.Courses.GetQuizResults,
                message);
            return response ?? ApiResponse<object>.ErrorResponse("Failed to get results");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting results");
            return ApiResponse<object>.ErrorResponse("An error occurred while retrieving results");
        }
    }

    // Progress endpoints
    public async Task<ApiResponse<object>> GetCourseProgressAsync(string userId, string courseId)
    {
        try
        {
            var message = new { UserId = userId, CourseId = courseId };
            var response = await _rabbitMQService.SendMessageAsync<ApiResponse<object>>(
                RabbitMQConstants.CoursesServiceQueue,
                RabbitMQConstants.Courses.GetCourseProgress,
                message);
            return response ?? ApiResponse<object>.ErrorResponse("Failed to get progress");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting progress");
            return ApiResponse<object>.ErrorResponse("An error occurred while retrieving progress");
        }
    }

    public async Task<ApiResponse<object>> StartLessonAsync(object dto)
    {
        try
        {
            var response = await _rabbitMQService.SendMessageAsync<ApiResponse<object>>(
                RabbitMQConstants.CoursesServiceQueue,
                RabbitMQConstants.Courses.StartLesson,
                dto);
            return response ?? ApiResponse<object>.ErrorResponse("Failed to start lesson");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error starting lesson");
            return ApiResponse<object>.ErrorResponse("An error occurred while starting lesson");
        }
    }

    public async Task<ApiResponse<object>> CompleteLessonAsync(object dto)
    {
        try
        {
            var response = await _rabbitMQService.SendMessageAsync<ApiResponse<object>>(
                RabbitMQConstants.CoursesServiceQueue,
                RabbitMQConstants.Courses.CompleteLesson,
                dto);
            return response ?? ApiResponse<object>.ErrorResponse("Failed to complete lesson");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error completing lesson");
            return ApiResponse<object>.ErrorResponse("An error occurred while completing lesson");
        }
    }

    public async Task<ApiResponse<object>> ViewSlideAsync(object dto)
    {
        try
        {
            var response = await _rabbitMQService.SendMessageAsync<ApiResponse<object>>(
                RabbitMQConstants.CoursesServiceQueue,
                RabbitMQConstants.Courses.ViewSlide,
                dto);
            return response ?? ApiResponse<object>.ErrorResponse("Failed to view slide");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error viewing slide");
            return ApiResponse<object>.ErrorResponse("An error occurred while viewing slide");
        }
    }

    public async Task<ApiResponse<object>> CompleteSlideAsync(object dto)
    {
        try
        {
            var response = await _rabbitMQService.SendMessageAsync<ApiResponse<object>>(
                RabbitMQConstants.CoursesServiceQueue,
                RabbitMQConstants.Courses.CompleteSlide,
                dto);
            return response ?? ApiResponse<object>.ErrorResponse("Failed to complete slide");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error completing slide");
            return ApiResponse<object>.ErrorResponse("An error occurred while completing slide");
        }
    }

    public async Task<ApiResponse<object>> LogActivityAsync(object activity)
    {
        try
        {
            var response = await _rabbitMQService.SendMessageAsync<ApiResponse<object>>(
                RabbitMQConstants.CoursesServiceQueue,
                RabbitMQConstants.Courses.LogActivity,
                activity);
            return response ?? ApiResponse<object>.ErrorResponse("Failed to log activity");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error logging activity");
            return ApiResponse<object>.ErrorResponse("An error occurred while logging activity");
        }
    }

    public async Task<ApiResponse<List<object>>> GetUserActivitiesAsync(string userId, int page, int pageSize)
    {
        try
        {
            var message = new { UserId = userId, Page = page, PageSize = pageSize };
            var response = await _rabbitMQService.SendMessageAsync<ApiResponse<List<object>>>(
                RabbitMQConstants.CoursesServiceQueue,
                RabbitMQConstants.Courses.GetUserActivities,
                message);
            return response ?? ApiResponse<List<object>>.ErrorResponse("Failed to get activities");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting activities");
            return ApiResponse<List<object>>.ErrorResponse("An error occurred while retrieving activities");
        }
    }

    public async Task<ApiResponse<List<object>>> GetCourseActivitiesAsync(string courseId, int page, int pageSize)
    {
        try
        {
            var message = new { CourseId = courseId, Page = page, PageSize = pageSize };
            var response = await _rabbitMQService.SendMessageAsync<ApiResponse<List<object>>>(
                RabbitMQConstants.CoursesServiceQueue,
                RabbitMQConstants.Courses.GetCourseActivities,
                message);
            return response ?? ApiResponse<List<object>>.ErrorResponse("Failed to get activities");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting activities");
            return ApiResponse<List<object>>.ErrorResponse("An error occurred while retrieving activities");
        }
    }

    // User endpoints
    public async Task<ApiResponse<List<object>>> GetUserCoursesAsync(string userId, int page, int pageSize)
    {
        try
        {
            var message = new { UserId = userId, Page = page, PageSize = pageSize };
            var response = await _rabbitMQService.SendMessageAsync<ApiResponse<List<object>>>(
                RabbitMQConstants.CoursesServiceQueue,
                RabbitMQConstants.Courses.GetUserCourses,
                message);
            return response ?? ApiResponse<List<object>>.ErrorResponse("Failed to get user courses");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user courses");
            return ApiResponse<List<object>>.ErrorResponse("An error occurred while retrieving user courses");
        }
    }

    public async Task<ApiResponse<object>> GetUserProgressAsync(string userId)
    {
        try
        {
            var message = new { UserId = userId };
            var response = await _rabbitMQService.SendMessageAsync<ApiResponse<object>>(
                RabbitMQConstants.CoursesServiceQueue,
                RabbitMQConstants.Courses.GetUserProgressSummary,
                message);
            return response ?? ApiResponse<object>.ErrorResponse("Failed to get user progress");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user progress");
            return ApiResponse<object>.ErrorResponse("An error occurred while retrieving user progress");
        }
    }

    public async Task<ApiResponse<PagedResponse<object>>> GetAllUsersAsync(int page, int pageSize, string? searchTerm = null)
    {
        try
        {
            var httpClient = _httpClientFactory.CreateClient();
            var queryParams = new List<string> { $"page={page}", $"pageSize={pageSize}" };
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                queryParams.Add($"search={Uri.EscapeDataString(searchTerm)}");
            }
            var url = $"{_coursesServiceUrl}/api/users?{string.Join("&", queryParams)}";
            
            var response = await httpClient.GetAsync(url);
            var responseContent = await response.Content.ReadAsStringAsync();
            
            if (response.IsSuccessStatusCode)
            {
                var result = JsonSerializer.Deserialize<ApiResponse<PagedResponse<object>>>(responseContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                return result ?? ApiResponse<PagedResponse<object>>.ErrorResponse("Failed to deserialize response");
            }
            
            return ApiResponse<PagedResponse<object>>.ErrorResponse($"Failed to get users: {response.StatusCode}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all users");
            return ApiResponse<PagedResponse<object>>.ErrorResponse("An error occurred while retrieving users");
        }
    }

    // Admin endpoints
    public async Task<ApiResponse<object>> GetCourseAnalyticsAsync()
    {
        try
        {
            var response = await _rabbitMQService.SendMessageAsync<ApiResponse<object>>(
                RabbitMQConstants.CoursesServiceQueue,
                RabbitMQConstants.Courses.GetCourseAnalytics,
                new { });
            return response ?? ApiResponse<object>.ErrorResponse("Failed to get analytics");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting course analytics");
            return ApiResponse<object>.ErrorResponse("An error occurred while retrieving analytics");
        }
    }

    public async Task<ApiResponse<object>> GetUserAnalyticsAsync()
    {
        try
        {
            var response = await _rabbitMQService.SendMessageAsync<ApiResponse<object>>(
                RabbitMQConstants.CoursesServiceQueue,
                RabbitMQConstants.Courses.GetUserAnalytics,
                new { });
            return response ?? ApiResponse<object>.ErrorResponse("Failed to get analytics");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user analytics");
            return ApiResponse<object>.ErrorResponse("An error occurred while retrieving analytics");
        }
    }

    public async Task<ApiResponse<object>> GetEngagementAnalyticsAsync()
    {
        try
        {
            var response = await _rabbitMQService.SendMessageAsync<ApiResponse<object>>(
                RabbitMQConstants.CoursesServiceQueue,
                RabbitMQConstants.Courses.GetEngagementAnalytics,
                new { });
            return response ?? ApiResponse<object>.ErrorResponse("Failed to get analytics");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting engagement analytics");
            return ApiResponse<object>.ErrorResponse("An error occurred while retrieving analytics");
        }
    }

    public async Task<ApiResponse<object>> GetPerformanceAnalyticsAsync()
    {
        try
        {
            var response = await _rabbitMQService.SendMessageAsync<ApiResponse<object>>(
                RabbitMQConstants.CoursesServiceQueue,
                RabbitMQConstants.Courses.GetPerformanceAnalytics,
                new { });
            return response ?? ApiResponse<object>.ErrorResponse("Failed to get analytics");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting performance analytics");
            return ApiResponse<object>.ErrorResponse("An error occurred while retrieving analytics");
        }
    }

    public async Task<ApiResponse<string>> UploadImageAsync(IFormFile file)
    {
        try
        {
            var httpClient = _httpClientFactory.CreateClient();
            using var content = new MultipartFormDataContent();
            using var stream = file.OpenReadStream();
            content.Add(new StreamContent(stream), "file", file.FileName);

            var response = await httpClient.PostAsync($"{_coursesServiceUrl}/api/Upload/image", content);
            var responseContent = await response.Content.ReadAsStringAsync();
            
            if (response.IsSuccessStatusCode)
            {
                var result = JsonSerializer.Deserialize<ApiResponse<string>>(responseContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });
                return result ?? ApiResponse<string>.ErrorResponse("Failed to upload image");
            }
            
            return ApiResponse<string>.ErrorResponse($"Upload failed: {responseContent}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading image through gateway");
            return ApiResponse<string>.ErrorResponse($"Error uploading image: {ex.Message}");
        }
    }

    public async Task<ApiResponse<bool>> DeleteImageAsync(string fileName)
    {
        try
        {
            var httpClient = _httpClientFactory.CreateClient();
            var response = await httpClient.DeleteAsync($"{_coursesServiceUrl}/api/Upload/image/{fileName}");
            var responseContent = await response.Content.ReadAsStringAsync();
            
            if (response.IsSuccessStatusCode)
            {
                var result = JsonSerializer.Deserialize<ApiResponse<bool>>(responseContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });
                return result ?? ApiResponse<bool>.ErrorResponse("Failed to delete image");
            }
            
            return ApiResponse<bool>.ErrorResponse($"Delete failed: {responseContent}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting image through gateway");
            return ApiResponse<bool>.ErrorResponse($"Error deleting image: {ex.Message}");
        }
    }

    public async Task<ApiResponse<object>> UploadQuizFileAsync(string lessonId, IFormFile file, int quizScore)
    {
        try
        {
            var httpClient = _httpClientFactory.CreateClient();
            using var content = new MultipartFormDataContent();
            using var stream = file.OpenReadStream();
            content.Add(new StreamContent(stream), "file", file.FileName);
            content.Add(new StringContent(quizScore.ToString()), "quizScore");

            var response = await httpClient.PostAsync(
                $"{_coursesServiceUrl}/api/lessons/{lessonId}/quizzes/upload", 
                content);
            var responseContent = await response.Content.ReadAsStringAsync();
            
            if (response.IsSuccessStatusCode)
            {
                var result = JsonSerializer.Deserialize<ApiResponse<object>>(responseContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });
                return result ?? ApiResponse<object>.ErrorResponse("Failed to upload quiz file");
            }
            
            var errorResult = JsonSerializer.Deserialize<ApiResponse<object>>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
            return errorResult ?? ApiResponse<object>.ErrorResponse($"Upload failed: {responseContent}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading quiz file through gateway");
            return ApiResponse<object>.ErrorResponse($"Error uploading quiz file: {ex.Message}");
        }
    }

    // Discussion endpoints
    public async Task<ApiResponse<List<object>>> GetAllDiscussionsAsync(string? userId = null, string? userRole = null)
    {
        try
        {
            _logger.LogInformation("GetAllDiscussionsAsync - userId: {UserId}, userRole: {UserRole}", userId ?? "null", userRole ?? "null");
            
            var httpClient = _httpClientFactory.CreateClient();
            var queryParams = new List<string>();
            
            if (!string.IsNullOrEmpty(userId))
            {
                queryParams.Add($"userId={Uri.EscapeDataString(userId)}");
            }
            
            if (!string.IsNullOrEmpty(userRole))
            {
                queryParams.Add($"userRole={Uri.EscapeDataString(userRole)}");
            }
            
            var queryString = queryParams.Any() ? "?" + string.Join("&", queryParams) : "";
            var url = $"{_coursesServiceUrl}/api/discussions{queryString}";
            _logger.LogInformation("Calling CoursesService: {Url}", url);
            
            var response = await httpClient.GetAsync(url);
            var responseContent = await response.Content.ReadAsStringAsync();
            
            if (response.IsSuccessStatusCode)
            {
                var result = JsonSerializer.Deserialize<ApiResponse<List<object>>>(responseContent, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });
                return result ?? ApiResponse<List<object>>.ErrorResponse("Failed to parse response");
            }
            
            var errorResult = JsonSerializer.Deserialize<ApiResponse<List<object>>>(responseContent, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
            return errorResult ?? ApiResponse<List<object>>.ErrorResponse($"Request failed: {responseContent}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all discussions");
            return ApiResponse<List<object>>.ErrorResponse($"An error occurred while retrieving discussions: {ex.Message}");
        }
    }

    public async Task<ApiResponse<List<object>>> GetPostsByDiscussionAsync(string discussionId)
    {
        try
        {
            var httpClient = _httpClientFactory.CreateClient();
            var response = await httpClient.GetAsync($"{_coursesServiceUrl}/api/discussions/{discussionId}/posts");
            var responseContent = await response.Content.ReadAsStringAsync();
            
            if (response.IsSuccessStatusCode)
            {
                var result = JsonSerializer.Deserialize<ApiResponse<List<object>>>(responseContent, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });
                return result ?? ApiResponse<List<object>>.ErrorResponse("Failed to parse response");
            }
            
            var errorResult = JsonSerializer.Deserialize<ApiResponse<List<object>>>(responseContent, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
            return errorResult ?? ApiResponse<List<object>>.ErrorResponse($"Request failed: {responseContent}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting posts by discussion {DiscussionId}", discussionId);
            return ApiResponse<List<object>>.ErrorResponse($"An error occurred while retrieving posts: {ex.Message}");
        }
    }

    public async Task<ApiResponse<object>> GetDiscussionByLessonAsync(string lessonId)
    {
        try
        {
            var httpClient = _httpClientFactory.CreateClient();
            var response = await httpClient.GetAsync($"{_coursesServiceUrl}/api/lessons/{lessonId}/discussions");
            var responseContent = await response.Content.ReadAsStringAsync();
            
            // Try to parse the response regardless of status code
            var result = JsonSerializer.Deserialize<ApiResponse<object>>(responseContent, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
            
            if (result != null)
            {
                return result; // Return the parsed response (even if it's an error response like 404)
            }
            
            // If parsing failed, return error
            return ApiResponse<object>.ErrorResponse(response.IsSuccessStatusCode 
                ? "Failed to parse response" 
                : $"Request failed with status {response.StatusCode}: {responseContent}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting discussion by lesson");
            return ApiResponse<object>.ErrorResponse($"An error occurred while retrieving discussion: {ex.Message}");
        }
    }

    public async Task<ApiResponse<object>> GetDiscussionByIdAsync(string id)
    {
        try
        {
            var httpClient = _httpClientFactory.CreateClient();
            var response = await httpClient.GetAsync($"{_coursesServiceUrl}/api/discussions/{id}");
            var responseContent = await response.Content.ReadAsStringAsync();
            
            if (response.IsSuccessStatusCode)
            {
                var result = JsonSerializer.Deserialize<ApiResponse<object>>(responseContent, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });
                return result ?? ApiResponse<object>.ErrorResponse("Failed to parse response");
            }
            
            var errorResult = JsonSerializer.Deserialize<ApiResponse<object>>(responseContent, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
            return errorResult ?? ApiResponse<object>.ErrorResponse($"Request failed: {responseContent}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting discussion by id");
            return ApiResponse<object>.ErrorResponse($"An error occurred while retrieving discussion: {ex.Message}");
        }
    }

    public async Task<ApiResponse<object>> CreateDiscussionAsync(object discussion)
    {
        try
        {
            var httpClient = _httpClientFactory.CreateClient();
            var json = JsonSerializer.Serialize(discussion, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            
            var response = await httpClient.PostAsync($"{_coursesServiceUrl}/api/discussions", content);
            var responseContent = await response.Content.ReadAsStringAsync();
            
            if (response.IsSuccessStatusCode)
            {
                var result = JsonSerializer.Deserialize<ApiResponse<object>>(responseContent, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });
                return result ?? ApiResponse<object>.ErrorResponse("Failed to parse response");
            }
            
            var errorResult = JsonSerializer.Deserialize<ApiResponse<object>>(responseContent, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
            return errorResult ?? ApiResponse<object>.ErrorResponse($"Request failed: {responseContent}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating discussion");
            return ApiResponse<object>.ErrorResponse($"An error occurred while creating discussion: {ex.Message}");
        }
    }

    public async Task<ApiResponse<object>> UpdateDiscussionAsync(string id, object discussion)
    {
        try
        {
            var httpClient = _httpClientFactory.CreateClient();
            var json = JsonSerializer.Serialize(discussion, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            
            var response = await httpClient.PutAsync($"{_coursesServiceUrl}/api/discussions/{id}", content);
            var responseContent = await response.Content.ReadAsStringAsync();
            
            if (response.IsSuccessStatusCode)
            {
                var result = JsonSerializer.Deserialize<ApiResponse<object>>(responseContent, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });
                return result ?? ApiResponse<object>.ErrorResponse("Failed to parse response");
            }
            
            var errorResult = JsonSerializer.Deserialize<ApiResponse<object>>(responseContent, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
            return errorResult ?? ApiResponse<object>.ErrorResponse($"Request failed: {responseContent}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating discussion");
            return ApiResponse<object>.ErrorResponse($"An error occurred while updating discussion: {ex.Message}");
        }
    }

    public async Task<ApiResponse<bool>> DeleteDiscussionAsync(string id)
    {
        try
        {
            var httpClient = _httpClientFactory.CreateClient();
            var response = await httpClient.DeleteAsync($"{_coursesServiceUrl}/api/discussions/{id}");
            var responseContent = await response.Content.ReadAsStringAsync();
            
            if (response.IsSuccessStatusCode)
            {
                var result = JsonSerializer.Deserialize<ApiResponse<bool>>(responseContent, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });
                return result ?? ApiResponse<bool>.ErrorResponse("Failed to parse response");
            }
            
            var errorResult = JsonSerializer.Deserialize<ApiResponse<bool>>(responseContent, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
            return errorResult ?? ApiResponse<bool>.ErrorResponse($"Request failed: {responseContent}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting discussion");
            return ApiResponse<bool>.ErrorResponse($"An error occurred while deleting discussion: {ex.Message}");
        }
    }

}

