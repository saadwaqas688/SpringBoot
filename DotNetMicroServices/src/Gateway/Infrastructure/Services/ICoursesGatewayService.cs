using Shared.Core.Common;

namespace Gateway.Infrastructure.Services;

public interface ICoursesGatewayService
{
    // Course endpoints
    Task<ApiResponse<string>> HealthCheckAsync();
    Task<ApiResponse<PagedResponse<object>>> GetAllCoursesAsync(int page, int pageSize);
    Task<ApiResponse<object>> GetCourseByIdAsync(string id);
    Task<ApiResponse<object>> CreateCourseAsync(object course);
    Task<ApiResponse<object>> UpdateCourseAsync(string id, object course);
    Task<ApiResponse<bool>> DeleteCourseAsync(string id);
    Task<ApiResponse<List<object>>> GetCoursesByStatusAsync(string status);
    Task<ApiResponse<List<object>>> SearchCoursesAsync(string searchTerm);
    Task<ApiResponse<object>> AssignUserToCourseAsync(string courseId, object dto);
    Task<ApiResponse<bool>> RemoveUserFromCourseAsync(string courseId, string userId);
    Task<ApiResponse<List<object>>> GetAssignedUsersAsync(string courseId, int page, int pageSize);
    Task<ApiResponse<object>> GetCourseUserProgressAsync(string courseId, string userId);
    
    // Lesson endpoints
    Task<ApiResponse<List<object>>> GetLessonsByCourseAsync(string courseId, int page, int pageSize);
    Task<ApiResponse<object>> GetLessonByIdAsync(string id);
    Task<ApiResponse<object>> CreateLessonAsync(object lesson);
    Task<ApiResponse<object>> UpdateLessonAsync(string id, object lesson);
    Task<ApiResponse<bool>> DeleteLessonAsync(string id);
    
    // Slide endpoints
    Task<ApiResponse<PagedResponse<object>>> GetSlidesByLessonAsync(string lessonId, int page, int pageSize);
    Task<ApiResponse<object>> GetSlideByIdAsync(string id);
    Task<ApiResponse<object>> CreateSlideAsync(object slide);
    Task<ApiResponse<object>> UpdateSlideAsync(string id, object slide);
    Task<ApiResponse<bool>> DeleteSlideAsync(string id);
    
    // Discussion endpoints
    Task<ApiResponse<List<object>>> GetAllDiscussionsAsync(string? userId = null, string? userRole = null);
    Task<ApiResponse<List<object>>> GetPostsByDiscussionAsync(string discussionId);
    Task<ApiResponse<List<object>>> GetPostsByLessonAsync(string lessonId, int page, int pageSize);
    Task<ApiResponse<object>> GetPostByIdAsync(string id);
    Task<ApiResponse<object>> CreatePostAsync(object post);
    Task<ApiResponse<object>> UpdatePostAsync(string id, object post);
    Task<ApiResponse<bool>> DeletePostAsync(string id);
    Task<ApiResponse<List<object>>> GetCommentsAsync(string postId, int page, int pageSize);
    
    // Quiz endpoints
    Task<ApiResponse<PagedResponse<object>>> GetAllQuizzesAsync(int page, int pageSize);
    Task<ApiResponse<object>> GetQuizByLessonAsync(string lessonId);
    Task<ApiResponse<object>> GetQuizByIdAsync(string id);
    Task<ApiResponse<object>> CreateQuizAsync(object quiz);
    Task<ApiResponse<object>> UpdateQuizAsync(string id, object quiz);
    Task<ApiResponse<bool>> DeleteQuizAsync(string id);
    
    // Quiz Question endpoints
    Task<ApiResponse<List<object>>> GetQuestionsByQuizAsync(string quizId, int page, int pageSize);
    Task<ApiResponse<object>> GetQuestionByIdAsync(string id);
    Task<ApiResponse<object>> CreateQuestionAsync(object question);
    Task<ApiResponse<object>> UpdateQuestionAsync(string id, object question);
    Task<ApiResponse<bool>> DeleteQuestionAsync(string id);
    
    // Quiz Attempt endpoints
    Task<ApiResponse<object>> CreateAttemptAsync(object dto);
    Task<ApiResponse<object>> GetAttemptByIdAsync(string attemptId);
    Task<ApiResponse<object>> AddAnswerAsync(string attemptId, object dto);
    Task<ApiResponse<object>> SubmitAttemptAsync(string attemptId);
    Task<ApiResponse<List<object>>> GetUserAttemptsAsync(string quizId, string userId);
    Task<ApiResponse<object>> GetQuizResultsAsync(string quizId, string userId);
    
    // Progress endpoints
    Task<ApiResponse<object>> GetCourseProgressAsync(string userId, string courseId);
    Task<ApiResponse<object>> StartLessonAsync(object dto);
    Task<ApiResponse<object>> CompleteLessonAsync(object dto);
    Task<ApiResponse<object>> ViewSlideAsync(object dto);
    Task<ApiResponse<object>> CompleteSlideAsync(object dto);
    Task<ApiResponse<object>> LogActivityAsync(object activity);
    Task<ApiResponse<List<object>>> GetUserActivitiesAsync(string userId, int page, int pageSize);
    Task<ApiResponse<List<object>>> GetCourseActivitiesAsync(string courseId, int page, int pageSize);
    
    // User endpoints
    Task<ApiResponse<List<object>>> GetUserCoursesAsync(string userId, int page, int pageSize);
    Task<ApiResponse<object>> GetUserProgressAsync(string userId);
    Task<ApiResponse<PagedResponse<object>>> GetAllUsersAsync(int page, int pageSize, string? searchTerm = null);
    
    // Admin endpoints
    Task<ApiResponse<object>> GetCourseAnalyticsAsync();
    Task<ApiResponse<object>> GetUserAnalyticsAsync();
    Task<ApiResponse<object>> GetEngagementAnalyticsAsync();
    Task<ApiResponse<object>> GetPerformanceAnalyticsAsync();
    
    // Upload endpoints (handles both images and videos)
    Task<ApiResponse<string>> UploadImageAsync(IFormFile file);
    Task<ApiResponse<bool>> DeleteImageAsync(string fileName);
    
    // Quiz upload endpoint
    Task<ApiResponse<object>> UploadQuizFileAsync(string lessonId, IFormFile file, int quizScore);
    
    // Discussion endpoints
    Task<ApiResponse<object>> GetDiscussionByLessonAsync(string lessonId);
    Task<ApiResponse<object>> GetDiscussionByIdAsync(string id);
    Task<ApiResponse<object>> CreateDiscussionAsync(object discussion);
    Task<ApiResponse<object>> UpdateDiscussionAsync(string id, object discussion);
    Task<ApiResponse<bool>> DeleteDiscussionAsync(string id);
}

