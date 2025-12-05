using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Application.Extensions;

namespace Gateway.Configuration;

/// <summary>
/// Configuration for dependency injection.
/// Extracted from Program.cs for better organization.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Registers all services for Gateway.
    /// </summary>
    public static IServiceCollection AddGatewayServices(this IServiceCollection services, IConfiguration configuration)
    {
        // RabbitMQ Service
        services.AddRabbitMQService(configuration);

        // Register HttpClient
        services.AddHttpClient();

        // Register Gateway services
        services.AddScoped<Infrastructure.Services.IUserAccountGatewayService, Infrastructure.Services.UserAccountGatewayService>();
        services.AddScoped<Infrastructure.Services.ICoursesGatewayService, Infrastructure.Services.CoursesGatewayService>();
        services.AddScoped<Infrastructure.Services.IJwtTokenService, Infrastructure.Services.JwtTokenService>();

        // JWT Authentication
        services.AddJwtAuthentication(configuration);

        // CORS Configuration
        services.AddCorsPolicy(configuration, services.BuildServiceProvider().GetRequiredService<Microsoft.Extensions.Hosting.IHostEnvironment>());

        return services;
    }
}

