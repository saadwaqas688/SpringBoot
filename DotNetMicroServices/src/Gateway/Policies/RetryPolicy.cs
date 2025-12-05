using Polly;
using Polly.Extensions.Http;
using System.Net.Http;

namespace Gateway.Policies;

/// <summary>
/// Retry policies for HTTP client resilience.
/// Uses Polly for retry logic.
/// </summary>
public static class RetryPolicy
{
    /// <summary>
    /// Creates a retry policy for transient HTTP errors.
    /// </summary>
    public static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
            .WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                onRetry: (outcome, timespan, retryCount, context) =>
                {
                    // Log retry attempts
                    Console.WriteLine($"Retry {retryCount} after {timespan.TotalSeconds} seconds");
                });
    }

    /// <summary>
    /// Creates a circuit breaker policy to prevent cascading failures.
    /// </summary>
    public static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .CircuitBreakerAsync(
                handledEventsAllowedBeforeBreaking: 5,
                durationOfBreak: TimeSpan.FromSeconds(30));
    }
}


