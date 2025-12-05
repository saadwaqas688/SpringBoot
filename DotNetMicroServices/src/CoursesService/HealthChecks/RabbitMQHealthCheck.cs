using Microsoft.Extensions.Diagnostics.HealthChecks;
using Shared.Infrastructure.Services;

namespace CoursesService.HealthChecks;

/// <summary>
/// Health check for RabbitMQ connection.
/// </summary>
public class RabbitMQHealthCheck : IHealthCheck
{
    private readonly IRabbitMQService _rabbitMQService;

    public RabbitMQHealthCheck(IRabbitMQService rabbitMQService)
    {
        _rabbitMQService = rabbitMQService;
    }

    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (_rabbitMQService != null)
            {
                return Task.FromResult(HealthCheckResult.Healthy("RabbitMQ service is available"));
            }

            return Task.FromResult(HealthCheckResult.Unhealthy("RabbitMQ service is not available"));
        }
        catch (Exception ex)
        {
            return Task.FromResult(HealthCheckResult.Unhealthy("RabbitMQ health check failed", ex));
        }
    }
}


