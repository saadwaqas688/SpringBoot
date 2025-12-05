namespace Shared.Infrastructure.Services;

public interface IRabbitMQService
{
    Task<T?> SendMessageAsync<T>(string queueName, string routingKey, object message);
    void StartListening(string queueName, Func<string, string, Task<object?>> messageHandler);
    void Dispose();
}

