using Microsoft.Extensions.Diagnostics.HealthChecks;
using MongoDB.Driver;

namespace UserAccountService.HealthChecks;

/// <summary>
/// Health check for MongoDB connection.
/// </summary>
public class MongoDbHealthCheck : IHealthCheck
{
    private readonly IMongoClient _mongoClient;
    private readonly string _databaseName;

    public MongoDbHealthCheck(IMongoClient mongoClient, string databaseName)
    {
        _mongoClient = mongoClient;
        _databaseName = databaseName;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var database = _mongoClient.GetDatabase(_databaseName);
            await database.RunCommandAsync<object>(
                new MongoDB.Bson.BsonDocument("ping", 1),
                cancellationToken: cancellationToken);

            return HealthCheckResult.Healthy("MongoDB connection is healthy");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("MongoDB connection is unhealthy", ex);
        }
    }
}


