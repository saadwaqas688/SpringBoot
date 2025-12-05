using Gateway.Infrastructure.Services;
using Shared.Application.Extensions;
using Shared.Core.Common;

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
    .AddJsonOptions(options =>
    {
        // Allow case-insensitive property matching (camelCase from frontend -> PascalCase in C#)
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
    })
    .AddMvcOptions(options =>
    {
        // Register TransformModelBinderProvider to enable [Transform] attribute
        // This allows field transformation similar to NestJS's @Transform decorator
        options.ModelBinderProviders.Insert(0, new Shared.Application.ModelBinders.TransformModelBinderProvider());
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register RabbitMQ Service using Options pattern
builder.Services.AddRabbitMQService(builder.Configuration);

// Register HttpClient for direct HTTP calls (for file uploads)
builder.Services.AddHttpClient();

// Register Gateway services
builder.Services.AddScoped<IUserAccountGatewayService, UserAccountGatewayService>();
builder.Services.AddScoped<ICoursesGatewayService, CoursesGatewayService>();
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();

// JWT Authentication Configuration using Options pattern
builder.Services.AddJwtAuthentication(builder.Configuration);

// CORS Configuration using Options pattern
builder.Services.AddCorsPolicy(builder.Configuration, builder.Environment);

var app = builder.Build();

// Configure the HTTP request pipeline
// Global exception handler must be first
app.UseGlobalExceptionHandler();

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseRouting();
app.UseCors("DefaultPolicy"); // Changed from "AllowAll" to "DefaultPolicy"
app.UseAuthentication();
app.UseAuthorization();

// Swagger configuration - after routing
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Gateway API v1");
    c.RoutePrefix = "swagger"; // Swagger UI at /swagger
});

// Map endpoints
app.MapGet("/", () => Results.Redirect("/swagger"));
app.MapControllers();

app.Run();
