using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Shared.Application.Extensions;
using Shared.Application.Options;
using Shared.Application.Validators;
using UserAccountService.Infrastructure.Data;
using UserAccountService.Models;
using UserAccountService.Application.Services;
using UserAccountService.Infrastructure.Services;

namespace UserAccountService.Extensions;

/// <summary>
/// Extension methods for configuring UserAccountService-specific services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Configures MongoDB context for UserAccountService.
    /// </summary>
    public static IServiceCollection AddUserAccountMongoDb(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMongoDb(configuration);

        var mongoDbSection = configuration.GetSection(MongoDbSettings.SectionName);
        var databaseName = mongoDbSection["DatabaseName"] ?? "UserAccountDB";

        services.AddScoped<MongoDbContext>(sp =>
        {
            var mongoClient = sp.GetRequiredService<IMongoClient>();
            return new MongoDbContext(mongoClient, databaseName);
        });

        return services;
    }

    /// <summary>
    /// Registers UserAccountService-specific services.
    /// </summary>
    public static IServiceCollection AddUserAccountServices(this IServiceCollection services)
    {
        services.AddScoped<Application.Services.IUserAccountService, Application.Services.UserAccountService>();
        services.AddScoped<Infrastructure.Services.IAuthService, Infrastructure.Services.AuthService>();
        services.AddScoped<Infrastructure.Services.UserAccountMessageHandler>();
        return services;
    }
}

