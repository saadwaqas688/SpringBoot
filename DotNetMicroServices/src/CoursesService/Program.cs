using CoursesService.Services;
using CoursesService.Data;
using CoursesService.Repositories;
using Shared.Services;
using Shared.Constants;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpContextAccessor();

// MongoDB Configuration
var mongoConnectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? "mongodb://localhost:27017";
var mongoDatabaseName = builder.Configuration["MongoDb:DatabaseName"] ?? "CoursesDB";

builder.Services.AddSingleton<IMongoClient>(sp => new MongoClient(mongoConnectionString));
builder.Services.AddScoped<CoursesDbContext>(sp =>
{
    var mongoClient = sp.GetRequiredService<IMongoClient>();
    return new CoursesDbContext(mongoClient, mongoDatabaseName);
});

// Register Repositories
builder.Services.AddScoped<ICourseRepository>(sp =>
{
    var context = sp.GetRequiredService<CoursesDbContext>();
    var logger = sp.GetRequiredService<ILogger<CourseRepository>>();
    return new CourseRepository(context.Courses, logger);
});

builder.Services.AddScoped<ILessonRepository>(sp =>
{
    var context = sp.GetRequiredService<CoursesDbContext>();
    var logger = sp.GetRequiredService<ILogger<LessonRepository>>();
    return new LessonRepository(context.Lessons, logger);
});

builder.Services.AddScoped<IStandardSlideRepository>(sp =>
{
    var context = sp.GetRequiredService<CoursesDbContext>();
    var logger = sp.GetRequiredService<ILogger<StandardSlideRepository>>();
    return new StandardSlideRepository(context.StandardSlides, logger);
});

builder.Services.AddScoped<IDiscussionPostRepository>(sp =>
{
    var context = sp.GetRequiredService<CoursesDbContext>();
    var logger = sp.GetRequiredService<ILogger<DiscussionPostRepository>>();
    return new DiscussionPostRepository(context.DiscussionPosts, logger);
});

builder.Services.AddScoped<IQuizRepository>(sp =>
{
    var context = sp.GetRequiredService<CoursesDbContext>();
    var logger = sp.GetRequiredService<ILogger<QuizRepository>>();
    return new QuizRepository(context.Quizzes, logger);
});

builder.Services.AddScoped<IQuizQuestionRepository>(sp =>
{
    var context = sp.GetRequiredService<CoursesDbContext>();
    var logger = sp.GetRequiredService<ILogger<QuizQuestionRepository>>();
    return new QuizQuestionRepository(context.QuizQuestions, logger);
});

builder.Services.AddScoped<IUserCourseRepository>(sp =>
{
    var context = sp.GetRequiredService<CoursesDbContext>();
    var logger = sp.GetRequiredService<ILogger<UserCourseRepository>>();
    return new UserCourseRepository(context.UserCourses, logger);
});

builder.Services.AddScoped<IUserLessonProgressRepository>(sp =>
{
    var context = sp.GetRequiredService<CoursesDbContext>();
    var logger = sp.GetRequiredService<ILogger<UserLessonProgressRepository>>();
    return new UserLessonProgressRepository(context.UserLessonProgress, logger);
});

builder.Services.AddScoped<IUserSlideProgressRepository>(sp =>
{
    var context = sp.GetRequiredService<CoursesDbContext>();
    var logger = sp.GetRequiredService<ILogger<UserSlideProgressRepository>>();
    return new UserSlideProgressRepository(context.UserSlideProgress, logger);
});

builder.Services.AddScoped<IUserActivityLogRepository>(sp =>
{
    var context = sp.GetRequiredService<CoursesDbContext>();
    var logger = sp.GetRequiredService<ILogger<UserActivityLogRepository>>();
    return new UserActivityLogRepository(context.UserActivityLogs, logger);
});

builder.Services.AddScoped<IUserQuizAttemptRepository>(sp =>
{
    var context = sp.GetRequiredService<CoursesDbContext>();
    var logger = sp.GetRequiredService<ILogger<UserQuizAttemptRepository>>();
    return new UserQuizAttemptRepository(context.UserQuizAttempts, logger);
});

builder.Services.AddScoped<IUserQuizAnswerRepository>(sp =>
{
    var context = sp.GetRequiredService<CoursesDbContext>();
    var logger = sp.GetRequiredService<ILogger<UserQuizAnswerRepository>>();
    return new UserQuizAnswerRepository(context.UserQuizAnswers, logger);
});

builder.Services.AddScoped<IUserRepository>(sp =>
{
    var context = sp.GetRequiredService<CoursesDbContext>();
    var logger = sp.GetRequiredService<ILogger<UserRepository>>();
    return new UserRepository(context.Users, logger);
});

// Register Services
builder.Services.AddScoped<ICourseService, CourseService>();
builder.Services.AddScoped<CoursesMessageHandler>(sp =>
{
    return new CoursesMessageHandler(
        sp.GetRequiredService<ICourseService>(),
        sp.GetRequiredService<ILessonRepository>(),
        sp.GetRequiredService<IStandardSlideRepository>(),
        sp.GetRequiredService<IDiscussionPostRepository>(),
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

// Register RabbitMQ Service
var rabbitMQConfig = builder.Configuration.GetSection("RabbitMQ");
builder.Services.AddSingleton<IRabbitMQService>(sp =>
{
    var logger = sp.GetRequiredService<ILogger<RabbitMQService>>();
    return new RabbitMQService(
        rabbitMQConfig["HostName"] ?? "localhost",
        int.Parse(rabbitMQConfig["Port"] ?? "5672"),
        rabbitMQConfig["UserName"] ?? "guest",
        rabbitMQConfig["Password"] ?? "guest",
        logger);
});

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Ensure database indexes are created
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<CoursesDbContext>();
    await context.CreateIndexesAsync();
}

// Configure the HTTP request pipeline
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Courses Service API v1");
    c.RoutePrefix = "swagger";
});

app.UseHttpsRedirection();
app.UseCors("AllowAll");

// Enable static files for uploaded images
var wwwrootPath = Path.Combine(builder.Environment.ContentRootPath, "wwwroot");
if (!Directory.Exists(wwwrootPath))
{
    Directory.CreateDirectory(wwwrootPath);
}

app.UseStaticFiles();
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(wwwrootPath),
    RequestPath = ""
});

app.MapControllers();

// Start RabbitMQ listener
var rabbitMQService = app.Services.GetRequiredService<IRabbitMQService>();

rabbitMQService.StartListening(RabbitMQConstants.CoursesServiceQueue, async (messageJson, routingKey) =>
{
    // Create a scope for each message to resolve scoped services
    using var scope = app.Services.CreateScope();
    var messageHandler = scope.ServiceProvider.GetRequiredService<CoursesMessageHandler>();
    var result = await messageHandler.HandleMessage(messageJson, routingKey);
    return result;
});

// Run the application
app.Run();

