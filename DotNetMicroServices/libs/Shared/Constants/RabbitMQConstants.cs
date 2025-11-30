namespace Shared.Constants;

public static class RabbitMQConstants
{
    // Queue Names
    public const string UserServiceQueue = "USER_SERVICE_QUEUE";
    public const string UserAccountServiceQueue = "USER_ACCOUNT_SERVICE_QUEUE";
    public const string CoursesServiceQueue = "COURSES_SERVICE_QUEUE";
    
    // Message Patterns - User Service
    public static class User
    {
        public const string HealthCheck = "user.health-check";
        public const string GetAll = "user.get-all";
        public const string GetById = "user.get-by-id";
        public const string GetByEmail = "user.get-by-email";
        public const string Create = "user.create";
        public const string Update = "user.update";
        public const string Delete = "user.delete";
    }
    
    // Message Patterns - UserAccount Service
    public static class UserAccount
    {
        public const string HealthCheck = "useraccount.health-check";
        public const string SignUp = "useraccount.signup";
        public const string SignIn = "useraccount.signin";
        public const string UpdateProfile = "useraccount.update-profile";
        public const string GetCurrentUser = "useraccount.get-current-user";
        public const string GetAllUsers = "useraccount.get-all-users";
        public const string RefreshToken = "useraccount.refresh-token";
        public const string ForgotPassword = "useraccount.forgot-password";
        public const string ResetPassword = "useraccount.reset-password";
    }
    
    // Message Patterns - Courses Service
    public static class Courses
    {
        // Course endpoints
        public const string HealthCheck = "courses.health-check";
        public const string GetAll = "courses.get-all";
        public const string GetById = "courses.get-by-id";
        public const string Create = "courses.create";
        public const string Update = "courses.update";
        public const string Delete = "courses.delete";
        public const string GetByStatus = "courses.get-by-status";
        public const string Search = "courses.search";
        public const string AssignUser = "courses.assign-user";
        public const string RemoveUser = "courses.remove-user";
        public const string GetAssignedUsers = "courses.get-assigned-users";
        public const string GetCourseUserProgress = "courses.get-course-user-progress";
        
        // Lesson endpoints
        public const string GetLessonsByCourse = "lessons.get-by-course";
        public const string GetLessonById = "lessons.get-by-id";
        public const string CreateLesson = "lessons.create";
        public const string UpdateLesson = "lessons.update";
        public const string DeleteLesson = "lessons.delete";
        
        // Slide endpoints
        public const string GetSlidesByLesson = "slides.get-by-lesson";
        public const string GetSlideById = "slides.get-by-id";
        public const string CreateSlide = "slides.create";
        public const string UpdateSlide = "slides.update";
        public const string DeleteSlide = "slides.delete";
        
        // Discussion endpoints
        public const string GetPostsByLesson = "posts.get-by-lesson";
        public const string GetPostById = "posts.get-by-id";
        public const string CreatePost = "posts.create";
        public const string UpdatePost = "posts.update";
        public const string DeletePost = "posts.delete";
        public const string GetComments = "posts.get-comments";
        
        // Quiz endpoints
        public const string GetQuizByLesson = "quizzes.get-by-lesson";
        public const string GetQuizById = "quizzes.get-by-id";
        public const string CreateQuiz = "quizzes.create";
        public const string UpdateQuiz = "quizzes.update";
        public const string DeleteQuiz = "quizzes.delete";
        
        // Quiz Question endpoints
        public const string GetQuestionsByQuiz = "quiz-questions.get-by-quiz";
        public const string GetQuestionById = "quiz-questions.get-by-id";
        public const string CreateQuestion = "quiz-questions.create";
        public const string UpdateQuestion = "quiz-questions.update";
        public const string DeleteQuestion = "quiz-questions.delete";
        
        // Quiz Attempt endpoints
        public const string CreateAttempt = "quiz-attempts.create";
        public const string GetAttemptById = "quiz-attempts.get-by-id";
        public const string AddAnswer = "quiz-attempts.add-answer";
        public const string SubmitAttempt = "quiz-attempts.submit";
        public const string GetUserAttempts = "quiz-attempts.get-user-attempts";
        public const string GetQuizResults = "quiz-attempts.get-results";
        
        // Progress endpoints
        public const string GetCourseProgress = "progress.get-course-progress";
        public const string StartLesson = "progress.start-lesson";
        public const string CompleteLesson = "progress.complete-lesson";
        public const string ViewSlide = "progress.view-slide";
        public const string CompleteSlide = "progress.complete-slide";
        public const string LogActivity = "activity.log";
        public const string GetUserActivities = "activity.get-user-activities";
        public const string GetCourseActivities = "activity.get-course-activities";
        
        // User endpoints
        public const string GetUserCourses = "users.get-courses";
        public const string GetUserProgressSummary = "users.get-progress-summary";
        
        // Admin endpoints
        public const string GetCourseAnalytics = "admin.analytics.courses";
        public const string GetUserAnalytics = "admin.analytics.users";
        public const string GetEngagementAnalytics = "admin.analytics.engagement";
        public const string GetPerformanceAnalytics = "admin.analytics.performance";
    }
}



