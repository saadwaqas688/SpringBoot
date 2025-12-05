using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UserAccountService.Extensions;
using Shared.Application.Extensions;

namespace UserAccountService.Configuration;

/// <summary>
/// Configuration for dependency injection.
/// Extracted from Program.cs for better organization.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Registers all services for UserAccountService.
    /// </summary>
    public static IServiceCollection AddUserAccountServices(this IServiceCollection services, IConfiguration configuration)
    {
        // MongoDB Configuration
        services.AddUserAccountMongoDb(configuration);

        // Register services
        services.AddUserAccountServices();

        // JWT Authentication
        services.AddJwtAuthentication(configuration);

        // RabbitMQ Service
        services.AddRabbitMQService(configuration);

        // CORS Configuration
        services.AddCorsPolicy(configuration, services.BuildServiceProvider().GetRequiredService<Microsoft.Extensions.Hosting.IHostEnvironment>());

        return services;
    }
}


