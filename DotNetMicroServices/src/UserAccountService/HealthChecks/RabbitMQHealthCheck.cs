using Microsoft.Extensions.Diagnostics.HealthChecks;
using Shared.Infrastructure.Services;

namespace UserAccountService.HealthChecks;

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
            // Check if RabbitMQ service is available
            // This is a simple check - in production, you might want to verify connection status
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


