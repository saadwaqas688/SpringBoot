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
    public IMongoCollection<Discussion> Discussions => _database.GetCollection<Discussion>("discussions");
    public IMongoCollection<User> Users => _database.GetCollection<User>("users");
    public IMongoCollection<UserCourse> UserCourses => _database.GetCollection<UserCourse>("user_courses");
    public IMongoCollection<UserLessonProgress> UserLessonProgress => _database.GetCollection<UserLessonProgress>("user_lesson_progress");
    public IMongoCollection<UserSlideProgress> UserSlideProgress => _database.GetCollection<UserSlideProgress>("user_slide_progress");
    public IMongoCollection<UserActivityLog> UserActivityLogs => _database.GetCollection<UserActivityLog>("user_activity_logs");
    public IMongoCollection<UserQuizAttempt> UserQuizAttempts => _database.GetCollection<UserQuizAttempt>("user_quiz_attempts");
    public IMongoCollection<UserQuizAnswer> UserQuizAnswers => _database.GetCollection<UserQuizAnswer>("user_quiz_answers");

    public async Task CreateIndexesAsync()
    {
        // Helper method to create index safely (ignores if already exists)
        async Task CreateIndexSafely<T>(IMongoCollection<T> collection, CreateIndexModel<T> indexModel, string indexName)
        {
            try
            {
                await collection.Indexes.CreateOneAsync(indexModel);
            }
            catch (MongoCommandException ex) when (ex.CodeName == "IndexOptionsConflict" || ex.CodeName == "IndexKeySpecsConflict" || ex.CodeName == "IndexAlreadyExists")
            {
                // Index already exists or conflicts, which is fine
                // We can ignore this error
            }
            catch (MongoWriteException ex) when (ex.WriteError?.Code == 85 || ex.WriteError?.Code == 86)
            {
                // Index already exists (error codes 85/86)
                // We can ignore this error
            }
        }

        // Course indexes
        await CreateIndexSafely(Courses,
            new CreateIndexModel<Course>(Builders<Course>.IndexKeys.Ascending(c => c.Status)),
            "Course_Status");

        // Lesson indexes
        await CreateIndexSafely(Lessons,
            new CreateIndexModel<Lesson>(Builders<Lesson>.IndexKeys.Ascending(l => l.CourseId)),
            "Lesson_CourseId");
        await CreateIndexSafely(Lessons,
            new CreateIndexModel<Lesson>(Builders<Lesson>.IndexKeys.Combine(
                Builders<Lesson>.IndexKeys.Ascending(l => l.CourseId),
                Builders<Lesson>.IndexKeys.Ascending(l => l.Order))),
            "Lesson_CourseId_Order");

        // Slide indexes
        await CreateIndexSafely(StandardSlides,
            new CreateIndexModel<StandardSlide>(Builders<StandardSlide>.IndexKeys.Ascending(s => s.LessonId)),
            "StandardSlide_LessonId");

        // Discussion indexes
        await CreateIndexSafely(DiscussionPosts,
            new CreateIndexModel<DiscussionPost>(Builders<DiscussionPost>.IndexKeys.Ascending(d => d.LessonId)),
            "DiscussionPost_LessonId");
        await CreateIndexSafely(DiscussionPosts,
            new CreateIndexModel<DiscussionPost>(Builders<DiscussionPost>.IndexKeys.Ascending(d => d.ParentPostId)),
            "DiscussionPost_ParentPostId");

        // Quiz indexes
        await CreateIndexSafely(QuizQuestions,
            new CreateIndexModel<QuizQuestion>(Builders<QuizQuestion>.IndexKeys.Ascending(q => q.QuizId)),
            "QuizQuestion_QuizId");

        // Discussion indexes - unique index to ensure one-to-one relationship with Lesson
        await CreateIndexSafely(Discussions,
            new CreateIndexModel<Discussion>(Builders<Discussion>.IndexKeys.Ascending(d => d.LessonId),
                new CreateIndexOptions { Unique = true }),
            "Discussion_LessonId_Unique");

        // User Course indexes
        await CreateIndexSafely(UserCourses,
            new CreateIndexModel<UserCourse>(Builders<UserCourse>.IndexKeys.Combine(
                Builders<UserCourse>.IndexKeys.Ascending(u => u.UserId),
                Builders<UserCourse>.IndexKeys.Ascending(u => u.CourseId)),
            new CreateIndexOptions { Unique = true }),
            "UserCourse_UserId_CourseId");

        // Progress indexes
        await CreateIndexSafely(UserLessonProgress,
            new CreateIndexModel<UserLessonProgress>(Builders<UserLessonProgress>.IndexKeys.Combine(
                Builders<UserLessonProgress>.IndexKeys.Ascending(u => u.UserId),
                Builders<UserLessonProgress>.IndexKeys.Ascending(u => u.LessonId)),
            new CreateIndexOptions { Unique = true }),
            "UserLessonProgress_UserId_LessonId");

        await CreateIndexSafely(UserSlideProgress,
            new CreateIndexModel<UserSlideProgress>(Builders<UserSlideProgress>.IndexKeys.Combine(
                Builders<UserSlideProgress>.IndexKeys.Ascending(u => u.UserId),
                Builders<UserSlideProgress>.IndexKeys.Ascending(u => u.SlideId)),
            new CreateIndexOptions { Unique = true }),
            "UserSlideProgress_UserId_SlideId");

        // Quiz attempt indexes
        await CreateIndexSafely(UserQuizAnswers,
            new CreateIndexModel<UserQuizAnswer>(Builders<UserQuizAnswer>.IndexKeys.Ascending(a => a.AttemptId)),
            "UserQuizAnswer_AttemptId");
    }
}

