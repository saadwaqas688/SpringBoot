using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using CoursesService.Extensions;
using Shared.Application.Extensions;

namespace CoursesService.Configuration;

/// <summary>
/// Configuration for dependency injection.
/// Extracted from Program.cs for better organization.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Registers all services for CoursesService.
    /// </summary>
    public static IServiceCollection AddCoursesServices(this IServiceCollection services, IConfiguration configuration)
    {
        // MongoDB Configuration
        services.AddCoursesMongoDb(configuration);

        // Register Repositories and Services
        services.AddCoursesRepositories();
        services.AddCoursesServices();

        // RabbitMQ Service
        services.AddRabbitMQService(configuration);

        // CORS Configuration
        services.AddCorsPolicy(configuration, services.BuildServiceProvider().GetRequiredService<Microsoft.Extensions.Hosting.IHostEnvironment>());

        return services;
    }
}


