using Shared.Services;

namespace UserAccountService.Services;

public class UserAccountMessageHandler
{
    private readonly ILogger<UserAccountMessageHandler> _logger;

    public UserAccountMessageHandler(ILogger<UserAccountMessageHandler> logger)
    {
        _logger = logger;
    }

    public async Task<bool> HandleMessage(string messageJson, string routingKey)
    {
        try
        {
            _logger.LogInformation("Received message with routing key: {RoutingKey}", routingKey);
            // Handle different message types here if needed
            return await Task.FromResult(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling message with routing key: {RoutingKey}", routingKey);
            return false;
        }
    }
}

