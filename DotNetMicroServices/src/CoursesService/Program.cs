using CoursesService.Infrastructure.Data;
using CoursesService.Extensions;
using CoursesService.Application.Services;
using CoursesService.Infrastructure.Services;
using MongoDB.Driver;
using Shared.Constants;
using Shared.Application.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers(options =>
{
    // Apply global validation filter - automatically validates all incoming requests
    // This mimics NestJS's ValidationPipe behavior:
    // - Validates DTOs using Data Annotations before controller actions execute
    // - Returns consistent ApiResponse format for validation errors
    // - Eliminates need for manual ModelState.IsValid checks in controllers
    options.Filters.Add<Shared.Application.Filters.ValidateModelAttribute>();
})
    .AddMvcOptions(options =>
    {
        // Register TransformModelBinderProvider to enable [Transform] attribute
        // This allows field transformation similar to NestJS's @Transform decorator
        options.ModelBinderProviders.Insert(0, new Shared.Application.ModelBinders.TransformModelBinderProvider());
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient();

// MongoDB Configuration using Options pattern
builder.Services.AddCoursesMongoDb(builder.Configuration);

// Register Repositories and Services using extension methods
builder.Services.AddCoursesRepositories();
builder.Services.AddCoursesServices();

// Register RabbitMQ Service using Options pattern
builder.Services.AddRabbitMQService(builder.Configuration);

// CORS Configuration using Options pattern
builder.Services.AddCorsPolicy(builder.Configuration, builder.Environment);

var app = builder.Build();

// Configure the HTTP request pipeline
// Global exception handler must be first
app.UseGlobalExceptionHandler();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Courses Service API v1");
    c.RoutePrefix = "swagger";
});

app.UseHttpsRedirection();
app.UseCors("DefaultPolicy"); // Changed from "AllowAll" to "DefaultPolicy"

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

// Ensure database indexes are created (with error handling)
try
{
    using (var scope = app.Services.CreateScope())
    {
        var context = scope.ServiceProvider.GetRequiredService<CoursesDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        
        logger.LogInformation("Attempting to create MongoDB indexes...");
        await context.CreateIndexesAsync();
        logger.LogInformation("MongoDB indexes created successfully");
    }
}
catch (MongoException ex)
{
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "Failed to create MongoDB indexes. The application will continue, but some features may not work correctly. " +
                        "Please ensure MongoDB is running and accessible.");
}
catch (Exception ex)
{
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "An unexpected error occurred while creating MongoDB indexes. The application will continue.");
}

// Start RabbitMQ listener (optional - app will still run if RabbitMQ is unavailable)
try
{
    var rabbitMQService = app.Services.GetRequiredService<Shared.Services.IRabbitMQService>();
    
    rabbitMQService.StartListening(RabbitMQConstants.CoursesServiceQueue, async (messageJson, routingKey) =>
    {
        // Create a scope for each message to resolve scoped services
        using var scope = app.Services.CreateScope();
        var messageHandler = scope.ServiceProvider.GetRequiredService<CoursesMessageHandler>();
        var result = await messageHandler.HandleMessage(messageJson, routingKey);
        return result;
    });
    
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    logger.LogInformation("RabbitMQ listener started successfully");
}
catch (Exception ex)
{
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    logger.LogWarning(ex, "Failed to start RabbitMQ listener. The application will continue without RabbitMQ messaging. " +
                          "Please ensure RabbitMQ is running if you need messaging functionality.");
}

// Register RabbitMQService disposal on application shutdown
app.Lifetime.ApplicationStopping.Register(() =>
{
    try
    {
        var rabbitMQService = app.Services.GetRequiredService<Shared.Services.IRabbitMQService>();
        if (rabbitMQService is IDisposable disposable)
        {
            disposable.Dispose();
        }
    }
    catch (Exception ex)
    {
        var logger = app.Services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Error disposing RabbitMQ service during shutdown");
    }
});

// Run the application
app.Run();

