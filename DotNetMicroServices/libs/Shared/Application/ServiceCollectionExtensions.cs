using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using Shared.Application.Middleware;
using Shared.Application.Options;
using Shared.Infrastructure.Services;
using Shared.Application.Validators;
using System.Text;

namespace Shared.Application.Extensions;

/// <summary>
/// Extension methods for configuring services in the dependency injection container.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Configures and validates JWT authentication settings.
    /// </summary>
    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtSection = configuration.GetSection(Options.JwtSettings.SectionName);
        services.Configure<Options.JwtSettings>(jwtSection);
        services.AddSingleton<IValidateOptions<Options.JwtSettings>, Validators.JwtSettingsValidator>();

        var jwtSettings = jwtSection.Get<Options.JwtSettings>() ?? new Options.JwtSettings();

        // Validate configuration at startup
        if (string.IsNullOrWhiteSpace(jwtSettings.SecretKey) || jwtSettings.SecretKey.Length < 32)
        {
            throw new InvalidOperationException(
                "JWT SecretKey is required and must be at least 32 characters long. " +
                "Please configure it in appsettings.json or use User Secrets.");
        }

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings.Issuer,
                ValidAudience = jwtSettings.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey))
            };
        });

        services.AddAuthorization();
        return services;
    }

    /// <summary>
    /// Configures and validates RabbitMQ service.
    /// </summary>
    public static IServiceCollection AddRabbitMQService(this IServiceCollection services, IConfiguration configuration)
    {
        var rabbitMQSection = configuration.GetSection(Options.RabbitMQSettings.SectionName);
        services.Configure<Options.RabbitMQSettings>(rabbitMQSection);
        services.AddSingleton<IValidateOptions<Options.RabbitMQSettings>, Validators.RabbitMQSettingsValidator>();

        var rabbitMQSettings = rabbitMQSection.Get<Options.RabbitMQSettings>() ?? new Options.RabbitMQSettings();

        services.AddSingleton<Infrastructure.Services.IRabbitMQService>(sp =>
        {
            var logger = sp.GetRequiredService<ILogger<Infrastructure.Services.RabbitMQService>>();
            return new Infrastructure.Services.RabbitMQService(
                rabbitMQSettings.HostName,
                rabbitMQSettings.Port,
                rabbitMQSettings.UserName,
                rabbitMQSettings.Password,
                logger);
        });

        return services;
    }

    /// <summary>
    /// Configures and validates MongoDB connection.
    /// </summary>
    public static IServiceCollection AddMongoDb(this IServiceCollection services, IConfiguration configuration)
    {
        var mongoDbSection = configuration.GetSection(Options.MongoDbSettings.SectionName);
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        var mongoDbSettings = new Options.MongoDbSettings
        {
            ConnectionString = connectionString ?? "mongodb://localhost:27017",
            DatabaseName = mongoDbSection["DatabaseName"] ?? "DefaultDB"
        };

        services.Configure<Options.MongoDbSettings>(options =>
        {
            options.ConnectionString = mongoDbSettings.ConnectionString;
            options.DatabaseName = mongoDbSettings.DatabaseName;
        });
        services.AddSingleton<IValidateOptions<Options.MongoDbSettings>, Validators.MongoDbSettingsValidator>();

        services.AddSingleton<IMongoClient>(sp => new MongoClient(mongoDbSettings.ConnectionString));

        return services;
    }

    /// <summary>
    /// Configures CORS policy with proper restrictions.
    /// </summary>
    public static IServiceCollection AddCorsPolicy(this IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
    {
        var corsSection = configuration.GetSection(Options.CorsSettings.SectionName);
        var corsSettings = corsSection.Get<Options.CorsSettings>() ?? new Options.CorsSettings();

        services.AddCors(options =>
        {
            options.AddPolicy("DefaultPolicy", policy =>
            {
                if (environment.IsDevelopment())
                {
                    // In development, allow all origins if no specific origins configured
                    if (corsSettings.AllowedOrigins == null || corsSettings.AllowedOrigins.Length == 0)
                    {
                        policy.AllowAnyOrigin()
                              .AllowAnyMethod()
                              .AllowAnyHeader();
                    }
                    else
                    {
                        policy.WithOrigins(corsSettings.AllowedOrigins)
                              .WithMethods(corsSettings.AllowedMethods.Length > 0 ? corsSettings.AllowedMethods : new[] { "GET", "POST", "PUT", "DELETE", "PATCH", "OPTIONS" })
                              .WithHeaders(corsSettings.AllowedHeaders.Length > 0 ? corsSettings.AllowedHeaders : new[] { "*" })
                              .AllowCredentials();
                    }
                }
                else
                {
                    // In production, require specific origins
                    if (corsSettings.AllowedOrigins == null || corsSettings.AllowedOrigins.Length == 0)
                    {
                        throw new InvalidOperationException(
                            "CORS AllowedOrigins must be configured in production. " +
                            "Please set Cors:AllowedOrigins in appsettings.json");
                    }

                    policy.WithOrigins(corsSettings.AllowedOrigins)
                          .WithMethods(corsSettings.AllowedMethods.Length > 0 ? corsSettings.AllowedMethods : new[] { "GET", "POST", "PUT", "DELETE", "PATCH", "OPTIONS" })
                          .WithHeaders(corsSettings.AllowedHeaders.Length > 0 ? corsSettings.AllowedHeaders : new[] { "Content-Type", "Authorization" })
                          .AllowCredentials();
                }
            });
        });

        return services;
    }

    /// <summary>
    /// Adds global exception handler middleware.
    /// </summary>
    public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder app)
    {
        return app.UseMiddleware<Middleware.GlobalExceptionHandlerMiddleware>();
    }
}

