using Shared.Common;
using Shared.Constants;
using System.Text.Json;
using CoursesService.Models;
using CoursesService.Repositories;
using MongoDB.Driver;
using System.Linq;

namespace CoursesService.Services;

public class CoursesMessageHandler
{
    private readonly ICourseService _courseService;
    private readonly IDiscussionPostService _postService;
    private readonly ILessonService _lessonService;
    private readonly ILessonRepository _lessonRepository;
    private readonly IStandardSlideRepository _slideRepository;
    private readonly IDiscussionRepository _discussionRepository;
    private readonly IQuizRepository _quizRepository;
    private readonly IQuizQuestionRepository _questionRepository;
    private readonly IUserQuizAttemptRepository _attemptRepository;
    private readonly IUserQuizAnswerRepository _answerRepository;
    private readonly IUserCourseRepository _userCourseRepository;
    private readonly IUserLessonProgressRepository _lessonProgressRepository;
    private readonly IUserSlideProgressRepository _slideProgressRepository;
    private readonly IUserActivityLogRepository _activityLogRepository;
    private readonly ILogger<CoursesMessageHandler> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    public CoursesMessageHandler(
        ICourseService courseService,
        IDiscussionPostService postService,
        ILessonService lessonService,
        ILessonRepository lessonRepository,
        IStandardSlideRepository slideRepository,
        IDiscussionRepository discussionRepository,
        IQuizRepository quizRepository,
        IQuizQuestionRepository questionRepository,
        IUserQuizAttemptRepository attemptRepository,
        IUserQuizAnswerRepository answerRepository,
        IUserCourseRepository userCourseRepository,
        IUserLessonProgressRepository lessonProgressRepository,
        IUserSlideProgressRepository slideProgressRepository,
        IUserActivityLogRepository activityLogRepository,
        ILogger<CoursesMessageHandler> logger)
    {
        _courseService = courseService;
        _postService = postService;
        _lessonService = lessonService;
        _lessonRepository = lessonRepository;
        _slideRepository = slideRepository;
        _discussionRepository = discussionRepository;
        _quizRepository = quizRepository;
        _questionRepository = questionRepository;
        _attemptRepository = attemptRepository;
        _answerRepository = answerRepository;
        _userCourseRepository = userCourseRepository;
        _lessonProgressRepository = lessonProgressRepository;
        _slideProgressRepository = slideProgressRepository;
        _activityLogRepository = activityLogRepository;
        _logger = logger;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    public async Task<object?> HandleMessage(string messageJson, string routingKey)
    {
        try
        {
            _logger.LogInformation("Handling message with routing key: {RoutingKey}", routingKey);

            return routingKey switch
            {
                // Course endpoints
                RabbitMQConstants.Courses.HealthCheck => HandleHealthCheck(),
                RabbitMQConstants.Courses.GetAll => await HandleGetAll(messageJson),
                RabbitMQConstants.Courses.GetById => await HandleGetById(messageJson),
                RabbitMQConstants.Courses.Create => await HandleCreate(messageJson),
                RabbitMQConstants.Courses.Update => await HandleUpdate(messageJson),
                RabbitMQConstants.Courses.Delete => await HandleDelete(messageJson),
                RabbitMQConstants.Courses.GetByStatus => await HandleGetByStatus(messageJson),
                RabbitMQConstants.Courses.Search => await HandleSearch(messageJson),
                RabbitMQConstants.Courses.AssignUser => await HandleAssignUser(messageJson),
                RabbitMQConstants.Courses.RemoveUser => await HandleRemoveUser(messageJson),
                RabbitMQConstants.Courses.GetAssignedUsers => await HandleGetAssignedUsers(messageJson),
                RabbitMQConstants.Courses.GetCourseUserProgress => await HandleGetCourseUserProgress(messageJson),
                
                // Lesson endpoints
                RabbitMQConstants.Courses.GetLessonsByCourse => await HandleGetLessonsByCourse(messageJson),
                RabbitMQConstants.Courses.GetLessonById => await HandleGetLessonById(messageJson),
                RabbitMQConstants.Courses.CreateLesson => await HandleCreateLesson(messageJson),
                RabbitMQConstants.Courses.UpdateLesson => await HandleUpdateLesson(messageJson),
                RabbitMQConstants.Courses.DeleteLesson => await HandleDeleteLesson(messageJson),
                
                // Slide endpoints
                RabbitMQConstants.Courses.GetSlidesByLesson => await HandleGetSlidesByLesson(messageJson),
                RabbitMQConstants.Courses.GetSlideById => await HandleGetSlideById(messageJson),
                RabbitMQConstants.Courses.CreateSlide => await HandleCreateSlide(messageJson),
                RabbitMQConstants.Courses.UpdateSlide => await HandleUpdateSlide(messageJson),
                RabbitMQConstants.Courses.DeleteSlide => await HandleDeleteSlide(messageJson),
                
                // Discussion endpoints
                RabbitMQConstants.Courses.GetPostsByLesson => await HandleGetPostsByLesson(messageJson),
                RabbitMQConstants.Courses.GetPostById => await HandleGetPostById(messageJson),
                RabbitMQConstants.Courses.CreatePost => await HandleCreatePost(messageJson),
                RabbitMQConstants.Courses.UpdatePost => await HandleUpdatePost(messageJson),
                RabbitMQConstants.Courses.DeletePost => await HandleDeletePost(messageJson),
                RabbitMQConstants.Courses.GetComments => await HandleGetComments(messageJson),
                
                // Quiz endpoints
                RabbitMQConstants.Courses.GetQuizByLesson => await HandleGetQuizByLesson(messageJson),
                RabbitMQConstants.Courses.GetQuizById => await HandleGetQuizById(messageJson),
                RabbitMQConstants.Courses.CreateQuiz => await HandleCreateQuiz(messageJson),
                RabbitMQConstants.Courses.UpdateQuiz => await HandleUpdateQuiz(messageJson),
                RabbitMQConstants.Courses.DeleteQuiz => await HandleDeleteQuiz(messageJson),
                
                // Quiz Question endpoints
                RabbitMQConstants.Courses.GetQuestionsByQuiz => await HandleGetQuestionsByQuiz(messageJson),
                RabbitMQConstants.Courses.GetQuestionById => await HandleGetQuestionById(messageJson),
                RabbitMQConstants.Courses.CreateQuestion => await HandleCreateQuestion(messageJson),
                RabbitMQConstants.Courses.UpdateQuestion => await HandleUpdateQuestion(messageJson),
                RabbitMQConstants.Courses.DeleteQuestion => await HandleDeleteQuestion(messageJson),
                
                // Quiz Attempt endpoints
                RabbitMQConstants.Courses.CreateAttempt => await HandleCreateAttempt(messageJson),
                RabbitMQConstants.Courses.GetAttemptById => await HandleGetAttemptById(messageJson),
                RabbitMQConstants.Courses.AddAnswer => await HandleAddAnswer(messageJson),
                RabbitMQConstants.Courses.SubmitAttempt => await HandleSubmitAttempt(messageJson),
                RabbitMQConstants.Courses.GetUserAttempts => await HandleGetUserAttempts(messageJson),
                RabbitMQConstants.Courses.GetQuizResults => await HandleGetQuizResults(messageJson),
                
                // Progress endpoints
                RabbitMQConstants.Courses.GetCourseProgress => await HandleGetCourseProgress(messageJson),
                RabbitMQConstants.Courses.StartLesson => await HandleStartLesson(messageJson),
                RabbitMQConstants.Courses.CompleteLesson => await HandleCompleteLesson(messageJson),
                RabbitMQConstants.Courses.ViewSlide => await HandleViewSlide(messageJson),
                RabbitMQConstants.Courses.CompleteSlide => await HandleCompleteSlide(messageJson),
                RabbitMQConstants.Courses.LogActivity => await HandleLogActivity(messageJson),
                RabbitMQConstants.Courses.GetUserActivities => await HandleGetUserActivities(messageJson),
                RabbitMQConstants.Courses.GetCourseActivities => await HandleGetCourseActivities(messageJson),
                
                // User endpoints
                RabbitMQConstants.Courses.GetUserCourses => await HandleGetUserCourses(messageJson),
                RabbitMQConstants.Courses.GetUserProgressSummary => await HandleGetUserProgress(messageJson),
                
                // Admin endpoints
                RabbitMQConstants.Courses.GetCourseAnalytics => await HandleGetCourseAnalytics(messageJson),
                RabbitMQConstants.Courses.GetUserAnalytics => await HandleGetUserAnalytics(messageJson),
                RabbitMQConstants.Courses.GetEngagementAnalytics => await HandleGetEngagementAnalytics(messageJson),
                RabbitMQConstants.Courses.GetPerformanceAnalytics => await HandleGetPerformanceAnalytics(messageJson),
                
                _ => ApiResponse<object>.ErrorResponse($"Unknown routing key: {routingKey}")
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling message with routing key: {RoutingKey}", routingKey);
            return ApiResponse<object>.ErrorResponse($"Error processing message: {ex.Message}");
        }
    }

    private object HandleHealthCheck()
    {
        return ApiResponse<string>.SuccessResponse("CoursesService is healthy", "Health check successful");
    }

    private async Task<object> HandleGetAll(string messageJson)
    {
        var request = JsonSerializer.Deserialize<GetAllRequest>(messageJson, _jsonOptions);
        var page = request?.Page ?? 1;
        var pageSize = request?.PageSize ?? 10;
        return await _courseService.GetAllAsync(page, pageSize);
    }

    private async Task<object> HandleGetById(string messageJson)
    {
        var request = JsonSerializer.Deserialize<GetByIdRequest>(messageJson, _jsonOptions);
        if (string.IsNullOrEmpty(request?.Id))
        {
            return ApiResponse<Course>.ErrorResponse("Id is required");
        }
        return await _courseService.GetByIdAsync(request.Id);
    }

    private async Task<object> HandleCreate(string messageJson)
    {
        var course = JsonSerializer.Deserialize<Course>(messageJson, _jsonOptions);
        if (course == null)
        {
            return ApiResponse<Course>.ErrorResponse("Invalid course data");
        }
        return await _courseService.CreateAsync(course);
    }

    private async Task<object> HandleUpdate(string messageJson)
    {
        var request = JsonSerializer.Deserialize<UpdateRequest>(messageJson, _jsonOptions);
        if (string.IsNullOrEmpty(request?.Id) || request.Course == null)
        {
            return ApiResponse<Course>.ErrorResponse("Id and course data are required");
        }
        return await _courseService.UpdateAsync(request.Id, request.Course);
    }

    private async Task<object> HandleDelete(string messageJson)
    {
        var request = JsonSerializer.Deserialize<GetByIdRequest>(messageJson, _jsonOptions);
        if (string.IsNullOrEmpty(request?.Id))
        {
            return ApiResponse<bool>.ErrorResponse("Id is required");
        }
        return await _courseService.DeleteAsync(request.Id);
    }

    private async Task<object> HandleGetByStatus(string messageJson)
    {
        var request = JsonSerializer.Deserialize<GetByStatusRequest>(messageJson, _jsonOptions);
        if (string.IsNullOrEmpty(request?.Status))
        {
            return ApiResponse<List<Course>>.ErrorResponse("Status is required");
        }
        return await _courseService.GetByStatusAsync(request.Status);
    }

    private async Task<object> HandleSearch(string messageJson)
    {
        var request = JsonSerializer.Deserialize<SearchRequest>(messageJson, _jsonOptions);
        if (string.IsNullOrEmpty(request?.SearchTerm))
        {
            return ApiResponse<List<Course>>.ErrorResponse("Search term is required");
        }
        return await _courseService.SearchAsync(request.SearchTerm);
    }

    // Helper classes for deserializing requests
    private class GetAllRequest
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    private class GetByIdRequest
    {
        public string? Id { get; set; }
    }

    private class UpdateRequest
    {
        public string? Id { get; set; }
        public Course? Course { get; set; }
    }

    private class GetByStatusRequest
    {
        public string? Status { get; set; }
    }

    private class SearchRequest
    {
        public string? SearchTerm { get; set; }
    }

    // Course Assignment handlers
    private async Task<object> HandleAssignUser(string messageJson)
    {
        var request = JsonSerializer.Deserialize<AssignUserRequest>(messageJson, _jsonOptions);
        if (string.IsNullOrEmpty(request?.CourseId) || request?.Dto?.UserIds == null || request.Dto.UserIds.Length == 0)
        {
            return ApiResponse<object>.ErrorResponse("CourseId and at least one UserId are required");
        }

        var assignedUsers = new List<object>();
        var alreadyAssigned = new List<string>();
        var failedUsers = new List<string>();

        foreach (var userId in request.Dto.UserIds.Distinct())
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                failedUsers.Add(userId ?? "null");
                continue;
            }

            try
            {
                var existing = await _userCourseRepository.GetByUserAndCourseAsync(userId, request.CourseId);
                if (existing != null)
                {
                    alreadyAssigned.Add(userId);
                    continue;
                }

                var userCourse = new UserCourse
                {
                    UserId = userId,
                    CourseId = request.CourseId,
                    Status = "not_started",
                    Progress = 0,
                    AssignedAt = DateTime.UtcNow
                };
                var created = await _userCourseRepository.CreateAsync(userCourse);
                assignedUsers.Add(created);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error assigning user {UserId} to course {CourseId}", userId, request.CourseId);
                failedUsers.Add(userId);
            }
        }

        var response = new
        {
            AssignedCount = assignedUsers.Count,
            AlreadyAssignedCount = alreadyAssigned.Count,
            FailedCount = failedUsers.Count,
            AssignedUsers = assignedUsers,
            AlreadyAssignedUserIds = alreadyAssigned,
            FailedUserIds = failedUsers
        };

        var message = $"Assigned {assignedUsers.Count} user(s) successfully";
        if (alreadyAssigned.Count > 0)
        {
            message += $", {alreadyAssigned.Count} user(s) were already assigned";
        }
        if (failedUsers.Count > 0)
        {
            message += $", {failedUsers.Count} user(s) failed to assign";
        }

        return ApiResponse<object>.SuccessResponse(response, message);
    }

    private async Task<object> HandleRemoveUser(string messageJson)
    {
        var request = JsonSerializer.Deserialize<RemoveUserRequest>(messageJson, _jsonOptions);
        if (string.IsNullOrEmpty(request?.CourseId) || string.IsNullOrEmpty(request?.UserId))
        {
            return ApiResponse<bool>.ErrorResponse("CourseId and UserId are required");
        }
        var userCourse = await _userCourseRepository.GetByUserAndCourseAsync(request.UserId, request.CourseId);
        if (userCourse == null)
        {
            return ApiResponse<bool>.ErrorResponse("User is not assigned to this course");
        }
        var deleted = await _userCourseRepository.DeleteAsync(userCourse.Id!);
        return ApiResponse<bool>.SuccessResponse(deleted, "User removed successfully");
    }

    private async Task<object> HandleGetAssignedUsers(string messageJson)
    {
        var request = JsonSerializer.Deserialize<PagedRequest>(messageJson, _jsonOptions);
        if (string.IsNullOrEmpty(request?.CourseId))
        {
            return ApiResponse<List<object>>.ErrorResponse("CourseId is required");
        }
        var users = await _userCourseRepository.GetByCourseIdAsync(request.CourseId);
        var paged = users.Skip((request.Page - 1) * request.PageSize).Take(request.PageSize).ToList();
        return ApiResponse<List<object>>.SuccessResponse(paged.Cast<object>().ToList(), "Assigned users retrieved successfully");
    }

    private async Task<object> HandleGetCourseUserProgress(string messageJson)
    {
        var request = JsonSerializer.Deserialize<GetUserProgressRequest>(messageJson, _jsonOptions);
        if (string.IsNullOrEmpty(request?.CourseId) || string.IsNullOrEmpty(request?.UserId))
        {
            return ApiResponse<object>.ErrorResponse("CourseId and UserId are required");
        }
        var userCourse = await _userCourseRepository.GetByUserAndCourseAsync(request.UserId, request.CourseId);
        if (userCourse == null)
        {
            return ApiResponse<object>.ErrorResponse("User is not assigned to this course");
        }
        return ApiResponse<object>.SuccessResponse(userCourse, "Progress retrieved successfully");
    }

    // Lesson handlers
    private async Task<object> HandleGetLessonsByCourse(string messageJson)
    {
        var request = JsonSerializer.Deserialize<PagedRequest>(messageJson, _jsonOptions);
        if (string.IsNullOrEmpty(request?.CourseId))
        {
            return ApiResponse<List<object>>.ErrorResponse("CourseId is required");
        }
        var response = await _lessonService.GetLessonsByCourseAsync(request.CourseId, request.Page, request.PageSize);
        // Convert to generic object response for message handler
        return ApiResponse<List<object>>.SuccessResponse(
            response.Data?.Cast<object>().ToList() ?? new List<object>(), 
            response.Message ?? "Lessons retrieved successfully");
    }

    private async Task<object> HandleGetLessonById(string messageJson)
    {
        var request = JsonSerializer.Deserialize<GetByIdRequest>(messageJson, _jsonOptions);
        if (string.IsNullOrEmpty(request?.Id))
        {
            return ApiResponse<object>.ErrorResponse("Id is required");
        }
        var response = await _lessonService.GetLessonByIdAsync(request.Id);
        // Convert to generic object response for message handler
        return ApiResponse<object>.SuccessResponse(response.Data, response.Message ?? "Lesson retrieved successfully");
    }

    private async Task<object> HandleCreateLesson(string messageJson)
    {
        var lesson = JsonSerializer.Deserialize<Lesson>(messageJson, _jsonOptions);
        if (lesson == null)
        {
            return ApiResponse<object>.ErrorResponse("Invalid lesson data");
        }
        var response = await _lessonService.CreateLessonAsync(lesson);
        // Convert to generic object response for message handler
        return ApiResponse<object>.SuccessResponse(response.Data, response.Message ?? "Lesson created successfully");
    }

    private async Task<object> HandleUpdateLesson(string messageJson)
    {
        var request = JsonSerializer.Deserialize<UpdateLessonRequest>(messageJson, _jsonOptions);
        if (string.IsNullOrEmpty(request?.Id) || request.Lesson == null)
        {
            return ApiResponse<object>.ErrorResponse("Id and lesson data are required");
        }
        var response = await _lessonService.UpdateLessonAsync(request.Id, request.Lesson);
        // Convert to generic object response for message handler
        return ApiResponse<object>.SuccessResponse(response.Data, response.Message ?? "Lesson updated successfully");
    }

    private async Task<object> HandleDeleteLesson(string messageJson)
    {
        var request = JsonSerializer.Deserialize<GetByIdRequest>(messageJson, _jsonOptions);
        if (string.IsNullOrEmpty(request?.Id))
        {
            return ApiResponse<bool>.ErrorResponse("Id is required");
        }
        var response = await _lessonService.DeleteLessonAsync(request.Id);
        return response;
    }

    // Slide handlers
    private async Task<object> HandleGetSlidesByLesson(string messageJson)
    {
        var request = JsonSerializer.Deserialize<PagedRequest>(messageJson, _jsonOptions);
        if (string.IsNullOrEmpty(request?.LessonId))
        {
            return ApiResponse<PagedResponse<object>>.ErrorResponse("LessonId is required");
        }
        var filter = Builders<StandardSlide>.Filter.Eq(s => s.LessonId, request.LessonId);
        var sort = Builders<StandardSlide>.Sort.Ascending(s => s.Order);
        var pagedResult = await _slideRepository.GetPagedAsync(request.Page, request.PageSize, filter, sort);
        
        // Convert PagedResponse<StandardSlide> to PagedResponse<object>
        var objectPagedResult = new PagedResponse<object>
        {
            Items = pagedResult.Items.Cast<object>().ToList(),
            PageNumber = pagedResult.PageNumber,
            PageSize = pagedResult.PageSize,
            TotalCount = pagedResult.TotalCount
        };
        
        return ApiResponse<PagedResponse<object>>.SuccessResponse(objectPagedResult, "Slides retrieved successfully");
    }

    private async Task<object> HandleGetSlideById(string messageJson)
    {
        var request = JsonSerializer.Deserialize<GetByIdRequest>(messageJson, _jsonOptions);
        if (string.IsNullOrEmpty(request?.Id))
        {
            return ApiResponse<object>.ErrorResponse("Id is required");
        }
        var slide = await _slideRepository.GetByIdAsync(request.Id);
        if (slide == null)
        {
            return ApiResponse<object>.ErrorResponse("Slide not found");
        }
        return ApiResponse<object>.SuccessResponse(slide, "Slide retrieved successfully");
    }

    private async Task<object> HandleCreateSlide(string messageJson)
    {
        var slide = JsonSerializer.Deserialize<StandardSlide>(messageJson, _jsonOptions);
        if (slide == null)
        {
            return ApiResponse<object>.ErrorResponse("Invalid slide data");
        }
        var created = await _slideRepository.CreateAsync(slide);
        return ApiResponse<object>.SuccessResponse(created, "Slide created successfully");
    }

    private async Task<object> HandleUpdateSlide(string messageJson)
    {
        var request = JsonSerializer.Deserialize<UpdateSlideRequest>(messageJson, _jsonOptions);
        if (string.IsNullOrEmpty(request?.Id) || request.Slide == null)
        {
            return ApiResponse<object>.ErrorResponse("Id and slide data are required");
        }
        request.Slide.Id = request.Id;
        var updated = await _slideRepository.UpdateAsync(request.Id, request.Slide);
        if (updated == null)
        {
            return ApiResponse<object>.ErrorResponse("Slide not found");
        }
        return ApiResponse<object>.SuccessResponse(updated, "Slide updated successfully");
    }

    private async Task<object> HandleDeleteSlide(string messageJson)
    {
        var request = JsonSerializer.Deserialize<GetByIdRequest>(messageJson, _jsonOptions);
        if (string.IsNullOrEmpty(request?.Id))
        {
            return ApiResponse<bool>.ErrorResponse("Id is required");
        }
        var deleted = await _slideRepository.DeleteAsync(request.Id);
        return ApiResponse<bool>.SuccessResponse(deleted, "Slide deleted successfully");
    }

    // Discussion handlers
    private async Task<object> HandleGetPostsByLesson(string messageJson)
    {
        var request = JsonSerializer.Deserialize<PagedRequest>(messageJson, _jsonOptions);
        if (string.IsNullOrEmpty(request?.LessonId))
        {
            return ApiResponse<List<object>>.ErrorResponse("LessonId is required");
        }
        var response = await _postService.GetPostsByLessonAsync(request.LessonId, request.Page, request.PageSize);
        // Convert to generic object response for message handler
        return ApiResponse<List<object>>.SuccessResponse(
            response.Data?.Cast<object>().ToList() ?? new List<object>(), 
            response.Message ?? "Posts retrieved successfully");
    }

    private async Task<object> HandleGetPostById(string messageJson)
    {
        var request = JsonSerializer.Deserialize<GetByIdRequest>(messageJson, _jsonOptions);
        if (string.IsNullOrEmpty(request?.Id))
        {
            return ApiResponse<object>.ErrorResponse("Id is required");
        }
        var response = await _postService.GetPostByIdAsync(request.Id);
        // Convert to generic object response for message handler
        return ApiResponse<object>.SuccessResponse(response.Data, response.Message ?? "Post retrieved successfully");
    }

    private async Task<object> HandleCreatePost(string messageJson)
    {
        var post = JsonSerializer.Deserialize<DiscussionPost>(messageJson, _jsonOptions);
        if (post == null)
        {
            return ApiResponse<object>.ErrorResponse("Invalid post data");
        }
        var response = await _postService.CreatePostAsync(post);
        // Convert to generic object response for message handler
        return ApiResponse<object>.SuccessResponse(response.Data, response.Message ?? "Post created successfully");
    }

    private async Task<object> HandleUpdatePost(string messageJson)
    {
        var request = JsonSerializer.Deserialize<UpdatePostRequest>(messageJson, _jsonOptions);
        if (string.IsNullOrEmpty(request?.Id) || request.Post == null)
        {
            return ApiResponse<object>.ErrorResponse("Id and post data are required");
        }
        var response = await _postService.UpdatePostAsync(request.Id, request.Post);
        // Convert to generic object response for message handler
        return ApiResponse<object>.SuccessResponse(response.Data, response.Message ?? "Post updated successfully");
    }

    private async Task<object> HandleDeletePost(string messageJson)
    {
        var request = JsonSerializer.Deserialize<GetByIdRequest>(messageJson, _jsonOptions);
        if (string.IsNullOrEmpty(request?.Id))
        {
            return ApiResponse<bool>.ErrorResponse("Id is required");
        }
        var response = await _postService.DeletePostAsync(request.Id);
        return response;
    }

    private async Task<object> HandleGetComments(string messageJson)
    {
        var request = JsonSerializer.Deserialize<PagedRequest>(messageJson, _jsonOptions);
        if (string.IsNullOrEmpty(request?.PostId))
        {
            return ApiResponse<List<object>>.ErrorResponse("PostId is required");
        }
        var response = await _postService.GetCommentsAsync(request.PostId, request.Page, request.PageSize);
        // Convert to generic object response for message handler
        return ApiResponse<List<object>>.SuccessResponse(
            response.Data?.Cast<object>().ToList() ?? new List<object>(), 
            response.Message ?? "Comments retrieved successfully");
    }

    // Quiz handlers
    private async Task<object> HandleGetQuizByLesson(string messageJson)
    {
        var request = JsonSerializer.Deserialize<GetByLessonRequest>(messageJson, _jsonOptions);
        if (string.IsNullOrEmpty(request?.LessonId))
        {
            return ApiResponse<object>.ErrorResponse("LessonId is required");
        }
        var quiz = await _quizRepository.GetByLessonIdAsync(request.LessonId);
        if (quiz == null)
        {
            return ApiResponse<object>.ErrorResponse("Quiz not found for this lesson");
        }
        return ApiResponse<object>.SuccessResponse(quiz, "Quiz retrieved successfully");
    }

    private async Task<object> HandleGetQuizById(string messageJson)
    {
        var request = JsonSerializer.Deserialize<GetByIdRequest>(messageJson, _jsonOptions);
        if (string.IsNullOrEmpty(request?.Id))
        {
            return ApiResponse<object>.ErrorResponse("Id is required");
        }
        var quiz = await _quizRepository.GetByIdAsync(request.Id);
        if (quiz == null)
        {
            return ApiResponse<object>.ErrorResponse("Quiz not found");
        }
        return ApiResponse<object>.SuccessResponse(quiz, "Quiz retrieved successfully");
    }

    private async Task<object> HandleCreateQuiz(string messageJson)
    {
        var quiz = JsonSerializer.Deserialize<Quiz>(messageJson, _jsonOptions);
        if (quiz == null)
        {
            return ApiResponse<object>.ErrorResponse("Invalid quiz data");
        }
        var created = await _quizRepository.CreateAsync(quiz);
        return ApiResponse<object>.SuccessResponse(created, "Quiz created successfully");
    }

    private async Task<object> HandleUpdateQuiz(string messageJson)
    {
        var request = JsonSerializer.Deserialize<UpdateQuizRequest>(messageJson, _jsonOptions);
        if (string.IsNullOrEmpty(request?.Id) || request.Quiz == null)
        {
            return ApiResponse<object>.ErrorResponse("Id and quiz data are required");
        }
        request.Quiz.Id = request.Id;
        var updated = await _quizRepository.UpdateAsync(request.Id, request.Quiz);
        if (updated == null)
        {
            return ApiResponse<object>.ErrorResponse("Quiz not found");
        }
        return ApiResponse<object>.SuccessResponse(updated, "Quiz updated successfully");
    }

    private async Task<object> HandleDeleteQuiz(string messageJson)
    {
        var request = JsonSerializer.Deserialize<GetByIdRequest>(messageJson, _jsonOptions);
        if (string.IsNullOrEmpty(request?.Id))
        {
            return ApiResponse<bool>.ErrorResponse("Id is required");
        }
        var deleted = await _quizRepository.DeleteAsync(request.Id);
        return ApiResponse<bool>.SuccessResponse(deleted, "Quiz deleted successfully");
    }

    // Quiz Question handlers
    private async Task<object> HandleGetQuestionsByQuiz(string messageJson)
    {
        var request = JsonSerializer.Deserialize<PagedRequest>(messageJson, _jsonOptions);
        if (string.IsNullOrEmpty(request?.QuizId))
        {
            return ApiResponse<List<object>>.ErrorResponse("QuizId is required");
        }
        var questions = await _questionRepository.GetByQuizIdAsync(request.QuizId);
        var paged = questions.Skip((request.Page - 1) * request.PageSize).Take(request.PageSize).ToList();
        return ApiResponse<List<object>>.SuccessResponse(paged.Cast<object>().ToList(), "Questions retrieved successfully");
    }

    private async Task<object> HandleGetQuestionById(string messageJson)
    {
        var request = JsonSerializer.Deserialize<GetByIdRequest>(messageJson, _jsonOptions);
        if (string.IsNullOrEmpty(request?.Id))
        {
            return ApiResponse<object>.ErrorResponse("Id is required");
        }
        var question = await _questionRepository.GetByIdAsync(request.Id);
        if (question == null)
        {
            return ApiResponse<object>.ErrorResponse("Question not found");
        }
        return ApiResponse<object>.SuccessResponse(question, "Question retrieved successfully");
    }

    private async Task<object> HandleCreateQuestion(string messageJson)
    {
        var question = JsonSerializer.Deserialize<QuizQuestion>(messageJson, _jsonOptions);
        if (question == null)
        {
            return ApiResponse<object>.ErrorResponse("Invalid question data");
        }
        question.Type = "quiz";
        var created = await _questionRepository.CreateAsync(question);
        return ApiResponse<object>.SuccessResponse(created, "Question created successfully");
    }

    private async Task<object> HandleUpdateQuestion(string messageJson)
    {
        var request = JsonSerializer.Deserialize<UpdateQuestionRequest>(messageJson, _jsonOptions);
        if (string.IsNullOrEmpty(request?.Id) || request.Question == null)
        {
            return ApiResponse<object>.ErrorResponse("Id and question data are required");
        }
        request.Question.Id = request.Id;
        var updated = await _questionRepository.UpdateAsync(request.Id, request.Question);
        if (updated == null)
        {
            return ApiResponse<object>.ErrorResponse("Question not found");
        }
        return ApiResponse<object>.SuccessResponse(updated, "Question updated successfully");
    }

    private async Task<object> HandleDeleteQuestion(string messageJson)
    {
        var request = JsonSerializer.Deserialize<GetByIdRequest>(messageJson, _jsonOptions);
        if (string.IsNullOrEmpty(request?.Id))
        {
            return ApiResponse<bool>.ErrorResponse("Id is required");
        }
        var deleted = await _questionRepository.DeleteAsync(request.Id);
        return ApiResponse<bool>.SuccessResponse(deleted, "Question deleted successfully");
    }

    // Quiz Attempt handlers - simplified implementations
    private async Task<object> HandleCreateAttempt(string messageJson)
    {
        var dto = JsonSerializer.Deserialize<CreateAttemptDto>(messageJson, _jsonOptions);
        if (dto == null || string.IsNullOrEmpty(dto.UserId) || string.IsNullOrEmpty(dto.QuizId))
        {
            return ApiResponse<object>.ErrorResponse("UserId and QuizId are required");
        }
        var attempt = new UserQuizAttempt
        {
            UserId = dto.UserId,
            QuizId = dto.QuizId,
            AttemptedAt = DateTime.UtcNow
        };
        var created = await _attemptRepository.CreateAsync(attempt);
        return ApiResponse<object>.SuccessResponse(created, "Attempt created successfully");
    }

    private async Task<object> HandleGetAttemptById(string messageJson)
    {
        var request = JsonSerializer.Deserialize<GetAttemptRequest>(messageJson, _jsonOptions);
        if (string.IsNullOrEmpty(request?.AttemptId))
        {
            return ApiResponse<object>.ErrorResponse("AttemptId is required");
        }
        var attempt = await _attemptRepository.GetByIdAsync(request.AttemptId);
        if (attempt == null)
        {
            return ApiResponse<object>.ErrorResponse("Attempt not found");
        }
        var answers = await _answerRepository.GetByAttemptIdAsync(request.AttemptId);
        var response = new { Attempt = attempt, Answers = answers };
        return ApiResponse<object>.SuccessResponse(response, "Attempt retrieved successfully");
    }

    private async Task<object> HandleAddAnswer(string messageJson)
    {
        var request = JsonSerializer.Deserialize<AddAnswerRequest>(messageJson, _jsonOptions);
        if (string.IsNullOrEmpty(request?.AttemptId) || string.IsNullOrEmpty(request?.Dto?.QuestionId))
        {
            return ApiResponse<object>.ErrorResponse("AttemptId and QuestionId are required");
        }
        var question = await _questionRepository.GetByIdAsync(request.Dto.QuestionId);
        if (question == null)
        {
            return ApiResponse<object>.ErrorResponse("Question not found");
        }
        var correctOption = question.Options.FirstOrDefault(o => o.IsCorrect);
        var isCorrect = correctOption != null && correctOption.Value == request.Dto.SelectedOption;
        var answer = new UserQuizAnswer
        {
            AttemptId = request.AttemptId,
            QuestionId = request.Dto.QuestionId,
            SelectedOption = request.Dto.SelectedOption,
            IsCorrect = isCorrect
        };
        var created = await _answerRepository.CreateAsync(answer);
        return ApiResponse<object>.SuccessResponse(created, "Answer added successfully");
    }

    private async Task<object> HandleSubmitAttempt(string messageJson)
    {
        var request = JsonSerializer.Deserialize<GetAttemptRequest>(messageJson, _jsonOptions);
        if (string.IsNullOrEmpty(request?.AttemptId))
        {
            return ApiResponse<object>.ErrorResponse("AttemptId is required");
        }
        var attempt = await _attemptRepository.GetByIdAsync(request.AttemptId);
        if (attempt == null)
        {
            return ApiResponse<object>.ErrorResponse("Attempt not found");
        }
        var answers = await _answerRepository.GetByAttemptIdAsync(request.AttemptId);
        var questions = await _questionRepository.GetByQuizIdAsync(attempt.QuizId);
        var correctCount = answers.Count(a => a.IsCorrect);
        var totalCount = questions.Count();
        var score = totalCount > 0 ? (int)((correctCount / (double)totalCount) * 100) : 0;
        attempt.CorrectAnswers = correctCount;
        attempt.TotalQuestions = totalCount;
        attempt.Score = score;
        var updated = await _attemptRepository.UpdateAsync(request.AttemptId, attempt);
        var result = new { Score = score, CorrectAnswers = correctCount, WrongAnswers = totalCount - correctCount, TotalQuestions = totalCount };
        return ApiResponse<object>.SuccessResponse(result, "Quiz submitted successfully");
    }

    private async Task<object> HandleGetUserAttempts(string messageJson)
    {
        var request = JsonSerializer.Deserialize<GetUserAttemptsRequest>(messageJson, _jsonOptions);
        if (string.IsNullOrEmpty(request?.QuizId) || string.IsNullOrEmpty(request?.UserId))
        {
            return ApiResponse<List<object>>.ErrorResponse("QuizId and UserId are required");
        }
        var attempts = await _attemptRepository.GetByUserIdAsync(request.UserId);
        var filtered = attempts.Where(a => a.QuizId == request.QuizId).ToList();
        return ApiResponse<List<object>>.SuccessResponse(filtered.Cast<object>().ToList(), "Attempts retrieved successfully");
    }

    private async Task<object> HandleGetQuizResults(string messageJson)
    {
        var request = JsonSerializer.Deserialize<GetQuizResultsRequest>(messageJson, _jsonOptions);
        if (string.IsNullOrEmpty(request?.QuizId) || string.IsNullOrEmpty(request?.UserId))
        {
            return ApiResponse<object>.ErrorResponse("QuizId and UserId are required");
        }
        var attempt = await _attemptRepository.GetLatestByUserAndQuizAsync(request.UserId, request.QuizId);
        if (attempt == null)
        {
            return ApiResponse<object>.ErrorResponse("No attempt found");
        }
        var result = new { Score = attempt.Score, CorrectAnswers = attempt.CorrectAnswers, WrongAnswers = attempt.TotalQuestions - attempt.CorrectAnswers, TotalQuestions = attempt.TotalQuestions };
        return ApiResponse<object>.SuccessResponse(result, "Results retrieved successfully");
    }

    // Progress handlers
    private async Task<object> HandleGetCourseProgress(string messageJson)
    {
        var request = JsonSerializer.Deserialize<GetUserProgressRequest>(messageJson, _jsonOptions);
        if (string.IsNullOrEmpty(request?.UserId) || string.IsNullOrEmpty(request?.CourseId))
        {
            return ApiResponse<object>.ErrorResponse("UserId and CourseId are required");
        }
        var progress = await _userCourseRepository.GetByUserAndCourseAsync(request.UserId, request.CourseId);
        if (progress == null)
        {
            return ApiResponse<object>.ErrorResponse("Progress not found");
        }
        return ApiResponse<object>.SuccessResponse(progress, "Progress retrieved successfully");
    }

    private async Task<object> HandleStartLesson(string messageJson)
    {
        var dto = JsonSerializer.Deserialize<StartLessonDto>(messageJson, _jsonOptions);
        if (dto == null || string.IsNullOrEmpty(dto.UserId) || string.IsNullOrEmpty(dto.LessonId))
        {
            return ApiResponse<object>.ErrorResponse("UserId and LessonId are required");
        }
        var progress = await _lessonProgressRepository.GetByUserAndLessonAsync(dto.UserId, dto.LessonId);
        if (progress == null)
        {
            progress = new UserLessonProgress { UserId = dto.UserId, LessonId = dto.LessonId, StartedAt = DateTime.UtcNow };
            progress = await _lessonProgressRepository.CreateAsync(progress);
        }
        else if (progress.StartedAt == null)
        {
            progress.StartedAt = DateTime.UtcNow;
            progress = await _lessonProgressRepository.UpdateAsync(progress.Id!, progress);
        }
        return ApiResponse<object>.SuccessResponse(progress, "Lesson started successfully");
    }

    private async Task<object> HandleCompleteLesson(string messageJson)
    {
        var dto = JsonSerializer.Deserialize<CompleteLessonDto>(messageJson, _jsonOptions);
        if (dto == null || string.IsNullOrEmpty(dto.UserId) || string.IsNullOrEmpty(dto.LessonId))
        {
            return ApiResponse<object>.ErrorResponse("UserId and LessonId are required");
        }
        var progress = await _lessonProgressRepository.GetByUserAndLessonAsync(dto.UserId, dto.LessonId);
        if (progress == null)
        {
            progress = new UserLessonProgress { UserId = dto.UserId, LessonId = dto.LessonId, StartedAt = DateTime.UtcNow, IsCompleted = true, CompletedAt = DateTime.UtcNow };
            progress = await _lessonProgressRepository.CreateAsync(progress);
        }
        else
        {
            progress.IsCompleted = true;
            progress.CompletedAt = DateTime.UtcNow;
            progress = await _lessonProgressRepository.UpdateAsync(progress.Id!, progress);
        }
        return ApiResponse<object>.SuccessResponse(progress, "Lesson completed successfully");
    }

    private async Task<object> HandleViewSlide(string messageJson)
    {
        var dto = JsonSerializer.Deserialize<ViewSlideDto>(messageJson, _jsonOptions);
        if (dto == null || string.IsNullOrEmpty(dto.UserId) || string.IsNullOrEmpty(dto.SlideId))
        {
            return ApiResponse<object>.ErrorResponse("UserId and SlideId are required");
        }
        var progress = await _slideProgressRepository.GetByUserAndSlideAsync(dto.UserId, dto.SlideId);
        if (progress == null)
        {
            progress = new UserSlideProgress { UserId = dto.UserId, SlideId = dto.SlideId, IsViewed = true, ViewedAt = DateTime.UtcNow };
            progress = await _slideProgressRepository.CreateAsync(progress);
        }
        else if (!progress.IsViewed)
        {
            progress.IsViewed = true;
            progress.ViewedAt = DateTime.UtcNow;
            progress = await _slideProgressRepository.UpdateAsync(progress.Id!, progress);
        }
        return ApiResponse<object>.SuccessResponse(progress, "Slide viewed successfully");
    }

    private async Task<object> HandleCompleteSlide(string messageJson)
    {
        var dto = JsonSerializer.Deserialize<CompleteSlideDto>(messageJson, _jsonOptions);
        if (dto == null || string.IsNullOrEmpty(dto.UserId) || string.IsNullOrEmpty(dto.SlideId))
        {
            return ApiResponse<object>.ErrorResponse("UserId and SlideId are required");
        }
        var progress = await _slideProgressRepository.GetByUserAndSlideAsync(dto.UserId, dto.SlideId);
        if (progress == null)
        {
            progress = new UserSlideProgress { UserId = dto.UserId, SlideId = dto.SlideId, IsViewed = true, IsCompleted = true, ViewedAt = DateTime.UtcNow, CompletedAt = DateTime.UtcNow, TimeSpent = dto.TimeSpent };
            progress = await _slideProgressRepository.CreateAsync(progress);
        }
        else
        {
            progress.IsViewed = true;
            progress.IsCompleted = true;
            progress.CompletedAt = DateTime.UtcNow;
            progress.TimeSpent = dto.TimeSpent;
            if (progress.ViewedAt == null) progress.ViewedAt = DateTime.UtcNow;
            progress = await _slideProgressRepository.UpdateAsync(progress.Id!, progress);
        }
        return ApiResponse<object>.SuccessResponse(progress, "Slide completed successfully");
    }

    private async Task<object> HandleLogActivity(string messageJson)
    {
        var activity = JsonSerializer.Deserialize<UserActivityLog>(messageJson, _jsonOptions);
        if (activity == null)
        {
            return ApiResponse<object>.ErrorResponse("Invalid activity data");
        }
        activity.Timestamp = DateTime.UtcNow;
        var created = await _activityLogRepository.CreateAsync(activity);
        return ApiResponse<object>.SuccessResponse(created, "Activity logged successfully");
    }

    private async Task<object> HandleGetUserActivities(string messageJson)
    {
        var request = JsonSerializer.Deserialize<PagedRequest>(messageJson, _jsonOptions);
        if (string.IsNullOrEmpty(request?.UserId))
        {
            return ApiResponse<List<object>>.ErrorResponse("UserId is required");
        }
        var activities = await _activityLogRepository.GetByUserIdAsync(request.UserId);
        var paged = activities.Skip((request.Page - 1) * request.PageSize).Take(request.PageSize).ToList();
        return ApiResponse<List<object>>.SuccessResponse(paged.Cast<object>().ToList(), "Activities retrieved successfully");
    }

    private async Task<object> HandleGetCourseActivities(string messageJson)
    {
        var request = JsonSerializer.Deserialize<PagedRequest>(messageJson, _jsonOptions);
        if (string.IsNullOrEmpty(request?.CourseId))
        {
            return ApiResponse<List<object>>.ErrorResponse("CourseId is required");
        }
        var activities = await _activityLogRepository.GetByCourseIdAsync(request.CourseId);
        var paged = activities.Skip((request.Page - 1) * request.PageSize).Take(request.PageSize).ToList();
        return ApiResponse<List<object>>.SuccessResponse(paged.Cast<object>().ToList(), "Activities retrieved successfully");
    }

    // User handlers
    private async Task<object> HandleGetUserCourses(string messageJson)
    {
        var request = JsonSerializer.Deserialize<PagedRequest>(messageJson, _jsonOptions);
        if (string.IsNullOrEmpty(request?.UserId))
        {
            return ApiResponse<List<object>>.ErrorResponse("UserId is required");
        }
        var userCourses = await _userCourseRepository.GetByUserIdAsync(request.UserId);
        var courseIds = userCourses.Select(uc => uc.CourseId).ToList();
        
        // Get full course details for each courseId
        var courses = new List<object>();
        
        foreach (var courseId in courseIds)
        {
            var courseResponse = await _courseService.GetByIdAsync(courseId);
            if (courseResponse.Success && courseResponse.Data != null)
            {
                var course = courseResponse.Data;
                var userCourse = userCourses.FirstOrDefault(uc => uc.CourseId == courseId);
                courses.Add(new
                {
                    Id = course.Id,
                    Title = course.Title,
                    Description = course.Description,
                    Thumbnail = course.Thumbnail,
                    Status = course.Status,
                    CreatedAt = course.CreatedAt,
                    UpdatedAt = course.UpdatedAt,
                    Progress = userCourse?.Progress ?? 0,
                    EnrollmentStatus = userCourse?.Status ?? "not_started",
                    AssignedAt = userCourse?.AssignedAt,
                    CompletedAt = userCourse?.CompletedAt
                });
            }
        }
        
        var paged = courses.Skip((request.Page - 1) * request.PageSize).Take(request.PageSize).ToList();
        return ApiResponse<List<object>>.SuccessResponse(paged, "User courses retrieved successfully");
    }

    private async Task<object> HandleGetUserProgress(string messageJson)
    {
        var request = JsonSerializer.Deserialize<GetUserProgressRequest>(messageJson, _jsonOptions);
        if (string.IsNullOrEmpty(request?.UserId))
        {
            return ApiResponse<object>.ErrorResponse("UserId is required");
        }
        var courses = await _userCourseRepository.GetByUserIdAsync(request.UserId);
        var lessons = await _lessonProgressRepository.GetByUserIdAsync(request.UserId);
        var summary = new
        {
            TotalCourses = courses.Count(),
            CompletedCourses = courses.Count(c => c.Status == "completed"),
            InProgressCourses = courses.Count(c => c.Status == "in_progress"),
            NotStartedCourses = courses.Count(c => c.Status == "not_started"),
            TotalLessons = lessons.Count(),
            CompletedLessons = lessons.Count(l => l.IsCompleted),
            AverageProgress = courses.Any() ? (int)courses.Average(c => c.Progress) : 0
        };
        return ApiResponse<object>.SuccessResponse(summary, "Progress summary retrieved successfully");
    }

    // Admin handlers
    private async Task<object> HandleGetCourseAnalytics(string messageJson)
    {
        var coursesResponse = await _courseService.GetAllAsync(1, 10000);
        var allCourses = coursesResponse.Data?.Items ?? new List<Course>();
        var userCourses = await _userCourseRepository.GetAllAsync();
        var analytics = new
        {
            TotalCourses = allCourses.Count(),
            PublishedCourses = allCourses.Count(c => c.Status == "published"),
            DraftCourses = allCourses.Count(c => c.Status == "draft"),
            TotalEnrollments = userCourses.Count(),
            AverageEnrollmentsPerCourse = allCourses.Any() ? (int)Math.Round(userCourses.Count() / (double)allCourses.Count()) : 0,
            CompletionRate = userCourses.Any() ? (int)Math.Round((userCourses.Count(uc => uc.Status == "completed") / (double)userCourses.Count()) * 100) : 0
        };
        return ApiResponse<object>.SuccessResponse(analytics, "Course analytics retrieved successfully");
    }

    private async Task<object> HandleGetUserAnalytics(string messageJson)
    {
        var userCourses = await _userCourseRepository.GetAllAsync();
        var uniqueUsers = userCourses.Select(uc => uc.UserId).Distinct();
        var analytics = new
        {
            TotalUsers = uniqueUsers.Count(),
            ActiveUsers = uniqueUsers.Count(),
            UsersWithCompletedCourses = uniqueUsers.Count(u => userCourses.Any(uc => uc.UserId == u && uc.Status == "completed")),
            AverageCoursesPerUser = uniqueUsers.Any() ? (int)Math.Round(userCourses.Count() / (double)uniqueUsers.Count()) : 0
        };
        return ApiResponse<object>.SuccessResponse(analytics, "User analytics retrieved successfully");
    }

    private async Task<object> HandleGetEngagementAnalytics(string messageJson)
    {
        var activities = await _activityLogRepository.GetAllAsync();
        var userCourses = await _userCourseRepository.GetAllAsync();
        var analytics = new
        {
            TotalActivities = activities.Count(),
            ActivitiesLast7Days = activities.Count(a => a.Timestamp >= DateTime.UtcNow.AddDays(-7)),
            ActivitiesLast30Days = activities.Count(a => a.Timestamp >= DateTime.UtcNow.AddDays(-30)),
            ActiveEnrollments = userCourses.Count(uc => uc.Status == "in_progress"),
            AverageSessionDuration = 0
        };
        return ApiResponse<object>.SuccessResponse(analytics, "Engagement analytics retrieved successfully");
    }

    private async Task<object> HandleGetPerformanceAnalytics(string messageJson)
    {
        var attempts = await _attemptRepository.GetAllAsync();
        var analytics = new
        {
            TotalQuizAttempts = attempts.Count(),
            AverageScore = attempts.Any() ? (int)Math.Round(attempts.Average(a => a.Score)) : 0,
            PassRate = attempts.Any() ? (int)Math.Round((attempts.Count(a => a.Score >= 70) / (double)attempts.Count()) * 100) : 0,
            TotalQuestionsAnswered = attempts.Sum(a => a.TotalQuestions),
            CorrectAnswers = attempts.Sum(a => a.CorrectAnswers)
        };
        return ApiResponse<object>.SuccessResponse(analytics, "Performance analytics retrieved successfully");
    }

    // Helper classes
    private class AssignUserRequest
    {
        public string? CourseId { get; set; }
        public AssignUserDto? Dto { get; set; }
    }

    private class AssignUserDto
    {
        public string[] UserIds { get; set; } = Array.Empty<string>();
    }

    private class RemoveUserRequest
    {
        public string? CourseId { get; set; }
        public string? UserId { get; set; }
    }

    private class PagedRequest
    {
        public string? CourseId { get; set; }
        public string? LessonId { get; set; }
        public string? QuizId { get; set; }
        public string? PostId { get; set; }
        public string? UserId { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    private class GetUserProgressRequest
    {
        public string? UserId { get; set; }
        public string? CourseId { get; set; }
    }

    private class UpdateLessonRequest
    {
        public string? Id { get; set; }
        public Lesson? Lesson { get; set; }
    }

    private class UpdateSlideRequest
    {
        public string? Id { get; set; }
        public StandardSlide? Slide { get; set; }
    }

    private class UpdatePostRequest
    {
        public string? Id { get; set; }
        public DiscussionPost? Post { get; set; }
    }

    private class GetByLessonRequest
    {
        public string? LessonId { get; set; }
    }

    private class UpdateQuizRequest
    {
        public string? Id { get; set; }
        public Quiz? Quiz { get; set; }
    }

    private class UpdateQuestionRequest
    {
        public string? Id { get; set; }
        public QuizQuestion? Question { get; set; }
    }

    private class CreateAttemptDto
    {
        public string UserId { get; set; } = string.Empty;
        public string QuizId { get; set; } = string.Empty;
    }

    private class GetAttemptRequest
    {
        public string? AttemptId { get; set; }
    }

    private class AddAnswerRequest
    {
        public string? AttemptId { get; set; }
        public AddAnswerDto? Dto { get; set; }
    }

    private class AddAnswerDto
    {
        public string QuestionId { get; set; } = string.Empty;
        public string SelectedOption { get; set; } = string.Empty;
    }

    private class GetUserAttemptsRequest
    {
        public string? QuizId { get; set; }
        public string? UserId { get; set; }
    }

    private class GetQuizResultsRequest
    {
        public string? QuizId { get; set; }
        public string? UserId { get; set; }
    }

    private class StartLessonDto
    {
        public string UserId { get; set; } = string.Empty;
        public string LessonId { get; set; } = string.Empty;
    }

    private class CompleteLessonDto
    {
        public string UserId { get; set; } = string.Empty;
        public string LessonId { get; set; } = string.Empty;
    }

    private class ViewSlideDto
    {
        public string UserId { get; set; } = string.Empty;
        public string SlideId { get; set; } = string.Empty;
    }

    private class CompleteSlideDto
    {
        public string UserId { get; set; } = string.Empty;
        public string SlideId { get; set; } = string.Empty;
        public int TimeSpent { get; set; }
    }
}

