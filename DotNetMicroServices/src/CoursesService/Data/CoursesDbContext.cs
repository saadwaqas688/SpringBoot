using MongoDB.Driver;
using CoursesService.Models;

namespace CoursesService.Data;

public class CoursesDbContext
{
    private readonly IMongoDatabase _database;

    public CoursesDbContext(IMongoClient mongoClient, string databaseName)
    {
        _database = mongoClient.GetDatabase(databaseName);
    }

    public IMongoCollection<Course> Courses => _database.GetCollection<Course>("courses");
    public IMongoCollection<Lesson> Lessons => _database.GetCollection<Lesson>("lessons");
    public IMongoCollection<StandardSlide> StandardSlides => _database.GetCollection<StandardSlide>("standard_slides");
    public IMongoCollection<DiscussionPost> DiscussionPosts => _database.GetCollection<DiscussionPost>("discussion_posts");
    public IMongoCollection<Quiz> Quizzes => _database.GetCollection<Quiz>("quizzes");
    public IMongoCollection<QuizQuestion> QuizQuestions => _database.GetCollection<QuizQuestion>("quiz_questions");
    public IMongoCollection<User> Users => _database.GetCollection<User>("users");
    public IMongoCollection<UserCourse> UserCourses => _database.GetCollection<UserCourse>("user_courses");
    public IMongoCollection<UserLessonProgress> UserLessonProgress => _database.GetCollection<UserLessonProgress>("user_lesson_progress");
    public IMongoCollection<UserSlideProgress> UserSlideProgress => _database.GetCollection<UserSlideProgress>("user_slide_progress");
    public IMongoCollection<UserActivityLog> UserActivityLogs => _database.GetCollection<UserActivityLog>("user_activity_logs");
    public IMongoCollection<UserQuizAttempt> UserQuizAttempts => _database.GetCollection<UserQuizAttempt>("user_quiz_attempts");
    public IMongoCollection<UserQuizAnswer> UserQuizAnswers => _database.GetCollection<UserQuizAnswer>("user_quiz_answers");

    public async Task CreateIndexesAsync()
    {
        // Course indexes
        await Courses.Indexes.CreateOneAsync(
            new CreateIndexModel<Course>(Builders<Course>.IndexKeys.Ascending(c => c.Status)));

        // Lesson indexes
        await Lessons.Indexes.CreateOneAsync(
            new CreateIndexModel<Lesson>(Builders<Lesson>.IndexKeys.Ascending(l => l.CourseId)));
        await Lessons.Indexes.CreateOneAsync(
            new CreateIndexModel<Lesson>(Builders<Lesson>.IndexKeys.Combine(
                Builders<Lesson>.IndexKeys.Ascending(l => l.CourseId),
                Builders<Lesson>.IndexKeys.Ascending(l => l.Order))));

        // Slide indexes
        await StandardSlides.Indexes.CreateOneAsync(
            new CreateIndexModel<StandardSlide>(Builders<StandardSlide>.IndexKeys.Ascending(s => s.LessonId)));

        // Discussion indexes
        await DiscussionPosts.Indexes.CreateOneAsync(
            new CreateIndexModel<DiscussionPost>(Builders<DiscussionPost>.IndexKeys.Ascending(d => d.LessonId)));
        await DiscussionPosts.Indexes.CreateOneAsync(
            new CreateIndexModel<DiscussionPost>(Builders<DiscussionPost>.IndexKeys.Ascending(d => d.ParentPostId)));

        // Quiz indexes
        await QuizQuestions.Indexes.CreateOneAsync(
            new CreateIndexModel<QuizQuestion>(Builders<QuizQuestion>.IndexKeys.Ascending(q => q.QuizId)));

        // User Course indexes
        await UserCourses.Indexes.CreateOneAsync(
            new CreateIndexModel<UserCourse>(Builders<UserCourse>.IndexKeys.Combine(
                Builders<UserCourse>.IndexKeys.Ascending(u => u.UserId),
                Builders<UserCourse>.IndexKeys.Ascending(u => u.CourseId)),
            new CreateIndexOptions { Unique = true }));

        // Progress indexes
        await UserLessonProgress.Indexes.CreateOneAsync(
            new CreateIndexModel<UserLessonProgress>(Builders<UserLessonProgress>.IndexKeys.Combine(
                Builders<UserLessonProgress>.IndexKeys.Ascending(u => u.UserId),
                Builders<UserLessonProgress>.IndexKeys.Ascending(u => u.LessonId)),
            new CreateIndexOptions { Unique = true }));

        await UserSlideProgress.Indexes.CreateOneAsync(
            new CreateIndexModel<UserSlideProgress>(Builders<UserSlideProgress>.IndexKeys.Combine(
                Builders<UserSlideProgress>.IndexKeys.Ascending(u => u.UserId),
                Builders<UserSlideProgress>.IndexKeys.Ascending(u => u.SlideId)),
            new CreateIndexOptions { Unique = true }));

        // Quiz attempt indexes
        await UserQuizAnswers.Indexes.CreateOneAsync(
            new CreateIndexModel<UserQuizAnswer>(Builders<UserQuizAnswer>.IndexKeys.Ascending(a => a.AttemptId)));
    }
}

