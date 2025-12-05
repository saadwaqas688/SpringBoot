using CoursesService.Infrastructure.Data;
using CoursesService.Infrastructure.Repositories;
using CoursesService.Application.Services;
using CoursesService.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Shared.Application.Extensions;
using Shared.Application.Options;

namespace CoursesService.Extensions;

/// <summary>
/// Extension methods for configuring CoursesService-specific services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Configures MongoDB context for CoursesService.
    /// </summary>
    public static IServiceCollection AddCoursesMongoDb(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMongoDb(configuration);

        var mongoDbSection = configuration.GetSection(MongoDbSettings.SectionName);
        var databaseName = mongoDbSection["DatabaseName"] ?? "CoursesDB";

        services.AddScoped<CoursesDbContext>(sp =>
        {
            var mongoClient = sp.GetRequiredService<IMongoClient>();
            return new CoursesDbContext(mongoClient, databaseName);
        });

        return services;
    }

    /// <summary>
    /// Registers all repositories for CoursesService.
    /// </summary>
    public static IServiceCollection AddCoursesRepositories(this IServiceCollection services)
    {
        services.AddScoped<ICourseRepository>(sp =>
        {
            var context = sp.GetRequiredService<CoursesDbContext>();
            var logger = sp.GetRequiredService<ILogger<CourseRepository>>();
            return new CourseRepository(context.Courses, logger);
        });

        services.AddScoped<ILessonRepository>(sp =>
        {
            var context = sp.GetRequiredService<CoursesDbContext>();
            var logger = sp.GetRequiredService<ILogger<LessonRepository>>();
            return new LessonRepository(context.Lessons, logger);
        });

        services.AddScoped<IStandardSlideRepository>(sp =>
        {
            var context = sp.GetRequiredService<CoursesDbContext>();
            var logger = sp.GetRequiredService<ILogger<StandardSlideRepository>>();
            return new StandardSlideRepository(context.StandardSlides, logger);
        });

        services.AddScoped<IDiscussionPostRepository>(sp =>
        {
            var context = sp.GetRequiredService<CoursesDbContext>();
            var logger = sp.GetRequiredService<ILogger<DiscussionPostRepository>>();
            return new DiscussionPostRepository(context.DiscussionPosts, logger);
        });

        services.AddScoped<IDiscussionRepository>(sp =>
        {
            var context = sp.GetRequiredService<CoursesDbContext>();
            var logger = sp.GetRequiredService<ILogger<DiscussionRepository>>();
            return new DiscussionRepository(context.Discussions, logger);
        });

        services.AddScoped<IQuizRepository>(sp =>
        {
            var context = sp.GetRequiredService<CoursesDbContext>();
            var logger = sp.GetRequiredService<ILogger<QuizRepository>>();
            return new QuizRepository(context.Quizzes, logger);
        });

        services.AddScoped<IQuizQuestionRepository>(sp =>
        {
            var context = sp.GetRequiredService<CoursesDbContext>();
            var logger = sp.GetRequiredService<ILogger<QuizQuestionRepository>>();
            return new QuizQuestionRepository(context.QuizQuestions, logger);
        });

        services.AddScoped<IUserCourseRepository>(sp =>
        {
            var context = sp.GetRequiredService<CoursesDbContext>();
            var logger = sp.GetRequiredService<ILogger<UserCourseRepository>>();
            return new UserCourseRepository(context.UserCourses, logger);
        });

        services.AddScoped<IUserLessonProgressRepository>(sp =>
        {
            var context = sp.GetRequiredService<CoursesDbContext>();
            var logger = sp.GetRequiredService<ILogger<UserLessonProgressRepository>>();
            return new UserLessonProgressRepository(context.UserLessonProgress, logger);
        });

        services.AddScoped<IUserSlideProgressRepository>(sp =>
        {
            var context = sp.GetRequiredService<CoursesDbContext>();
            var logger = sp.GetRequiredService<ILogger<UserSlideProgressRepository>>();
            return new UserSlideProgressRepository(context.UserSlideProgress, logger);
        });

        services.AddScoped<IUserActivityLogRepository>(sp =>
        {
            var context = sp.GetRequiredService<CoursesDbContext>();
            var logger = sp.GetRequiredService<ILogger<UserActivityLogRepository>>();
            return new UserActivityLogRepository(context.UserActivityLogs, logger);
        });

        services.AddScoped<IUserQuizAttemptRepository>(sp =>
        {
            var context = sp.GetRequiredService<CoursesDbContext>();
            var logger = sp.GetRequiredService<ILogger<UserQuizAttemptRepository>>();
            return new UserQuizAttemptRepository(context.UserQuizAttempts, logger);
        });

        services.AddScoped<IUserQuizAnswerRepository>(sp =>
        {
            var context = sp.GetRequiredService<CoursesDbContext>();
            var logger = sp.GetRequiredService<ILogger<UserQuizAnswerRepository>>();
            return new UserQuizAnswerRepository(context.UserQuizAnswers, logger);
        });

        services.AddScoped<IUserRepository>(sp =>
        {
            var context = sp.GetRequiredService<CoursesDbContext>();
            var logger = sp.GetRequiredService<ILogger<UserRepository>>();
            return new UserRepository(context.Users, logger);
        });

        return services;
    }

    /// <summary>
    /// Registers all services for CoursesService.
    /// </summary>
    public static IServiceCollection AddCoursesServices(this IServiceCollection services)
    {
        services.AddScoped<IQuizFileParserService, QuizFileParserService>();
        services.AddScoped<IDiscussionPostService, DiscussionPostService>();
        services.AddScoped<ILessonService, LessonService>();
        services.AddScoped<ICourseService, CourseService>();

        services.AddScoped<CoursesMessageHandler>(sp =>
        {
            return new CoursesMessageHandler(
                sp.GetRequiredService<ICourseService>(),
                sp.GetRequiredService<IDiscussionPostService>(),
                sp.GetRequiredService<ILessonService>(),
                sp.GetRequiredService<ILessonRepository>(),
                sp.GetRequiredService<IStandardSlideRepository>(),
                sp.GetRequiredService<IDiscussionRepository>(),
                sp.GetRequiredService<IQuizRepository>(),
                sp.GetRequiredService<IQuizQuestionRepository>(),
                sp.GetRequiredService<IUserQuizAttemptRepository>(),
                sp.GetRequiredService<IUserQuizAnswerRepository>(),
                sp.GetRequiredService<IUserCourseRepository>(),
                sp.GetRequiredService<IUserLessonProgressRepository>(),
                sp.GetRequiredService<IUserSlideProgressRepository>(),
                sp.GetRequiredService<IUserActivityLogRepository>(),
                sp.GetRequiredService<ILogger<CoursesMessageHandler>>());
        });

        return services;
    }
}

