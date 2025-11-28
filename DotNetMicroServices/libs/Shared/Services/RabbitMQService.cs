using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace Shared.Services;

public class RabbitMQService : IRabbitMQService, IDisposable
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly ILogger<RabbitMQService> _logger;
    private readonly string _exchangeName = "microservices_exchange";
    private bool _disposed = false;
    private static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true,
        WriteIndented = false
    };

    public RabbitMQService(string hostName, int port, string userName, string password, ILogger<RabbitMQService> logger)
    {
        _logger = logger;
        
        var factory = new ConnectionFactory
        {
            HostName = hostName,
            Port = port,
            UserName = userName,
            Password = password,
            AutomaticRecoveryEnabled = true,
            NetworkRecoveryInterval = TimeSpan.FromSeconds(10)
        };

        // Retry connection with exponential backoff
        const int maxRetries = 5;
        var retryDelay = TimeSpan.FromSeconds(2);
        
        for (int attempt = 1; attempt <= maxRetries; attempt++)
        {
            try
            {
                _connection = factory.CreateConnection();
                _channel = _connection.CreateModel();
                
                // Declare exchange
                _channel.ExchangeDeclare(_exchangeName, ExchangeType.Topic, durable: true);
                
                _logger.LogInformation("RabbitMQ connection established");
                return;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to establish RabbitMQ connection (attempt {Attempt}/{MaxRetries})", attempt, maxRetries);
                
                if (attempt == maxRetries)
                {
                    _logger.LogError("Failed to establish RabbitMQ connection after {MaxRetries} attempts", maxRetries);
                    throw;
                }
                
                Thread.Sleep(retryDelay);
                retryDelay = TimeSpan.FromSeconds(retryDelay.TotalSeconds * 2); // Exponential backoff
            }
        }
    }

    public async Task<T?> SendMessageAsync<T>(string queueName, string routingKey, object message)
    {
        try
        {
            // Ensure queue exists (listener should have already created and bound it)
            _channel.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);

            // Create response queue
            var replyQueueName = _channel.QueueDeclare().QueueName;
            var correlationId = Guid.NewGuid().ToString();
            var tcs = new TaskCompletionSource<T?>();
            var consumer = new EventingBasicConsumer(_channel);

            // Set up response consumer
            consumer.Received += (model, ea) =>
            {
                if (ea.BasicProperties.CorrelationId == correlationId)
                {
                    var body = ea.Body.ToArray();
                    var responseJson = Encoding.UTF8.GetString(body);
                    
                    try
                    {
                        var response = JsonSerializer.Deserialize<T>(responseJson, _jsonOptions);
                        tcs.SetResult(response);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to deserialize response");
                        tcs.SetResult(default(T));
                    }
                }
            };

            _channel.BasicConsume(queue: replyQueueName, autoAck: true, consumer: consumer);

            // Serialize and send message
            var messageJson = JsonSerializer.Serialize(message, _jsonOptions);
            var messageBody = Encoding.UTF8.GetBytes(messageJson);

            var properties = _channel.CreateBasicProperties();
            properties.CorrelationId = correlationId;
            properties.ReplyTo = replyQueueName;
            properties.Persistent = true;

            _channel.BasicPublish(
                exchange: _exchangeName,
                routingKey: routingKey,
                basicProperties: properties,
                body: messageBody);

            _logger.LogDebug("Message sent to queue {QueueName} with routing key {RoutingKey}", queueName, routingKey);

            // Wait for response with timeout
            var timeoutTask = Task.Delay(TimeSpan.FromSeconds(30));
            var completedTask = await Task.WhenAny(tcs.Task, timeoutTask);

            if (completedTask == timeoutTask)
            {
                _logger.LogWarning("Timeout waiting for response from {QueueName}", queueName);
                return default(T);
            }

            return await tcs.Task;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending message to queue {QueueName}", queueName);
            return default(T);
        }
    }

    public void StartListening(string queueName, Func<string, string, Task<object?>> messageHandler)
    {
        try
        {
            // Declare queue
            _channel.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
            
            // Determine routing key pattern based on queue name
            string routingKeyPattern;
            if (queueName.Contains("USER_ACCOUNT", StringComparison.OrdinalIgnoreCase))
            {
                routingKeyPattern = "useraccount.*";
            }
            else if (queueName.Contains("COURSES", StringComparison.OrdinalIgnoreCase))
            {
                // Bind to all course-related routing keys with multiple patterns
                var patterns = new[] { "courses.*", "lessons.*", "slides.*", "posts.*", "quizzes.*", "quiz-questions.*", "quiz-attempts.*", "progress.*", "activity.*", "users.*", "admin.*" };
                foreach (var pattern in patterns)
                {
                    _channel.QueueBind(queue: queueName, exchange: _exchangeName, routingKey: pattern);
                }
                routingKeyPattern = "courses.*"; // Default for logging
            }
            else if (queueName.Contains("USER", StringComparison.OrdinalIgnoreCase))
            {
                routingKeyPattern = "user.*";
            }
            else
            {
                routingKeyPattern = $"{queueName.ToLower()}.*";
            }
            
            // Bind queue to exchange with routing key pattern (if not already bound for CoursesService)
            if (!queueName.Contains("COURSES", StringComparison.OrdinalIgnoreCase))
            {
                _channel.QueueBind(queue: queueName, exchange: _exchangeName, routingKey: routingKeyPattern);
            }

            var consumer = new EventingBasicConsumer(_channel);
            
            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var messageJson = Encoding.UTF8.GetString(body);
                var routingKey = ea.RoutingKey;
                var replyProperties = _channel.CreateBasicProperties();
                replyProperties.CorrelationId = ea.BasicProperties.CorrelationId;

                try
                {
                    _logger.LogDebug("Received message on queue {QueueName} with routing key {RoutingKey}", queueName, routingKey);
                    
                    var response = await messageHandler(messageJson, routingKey);
                    
                    if (response != null && !string.IsNullOrEmpty(ea.BasicProperties.ReplyTo))
                    {
                        var responseJson = JsonSerializer.Serialize(response, _jsonOptions);
                        var responseBody = Encoding.UTF8.GetBytes(responseJson);
                        
                        _channel.BasicPublish(
                            exchange: "",
                            routingKey: ea.BasicProperties.ReplyTo,
                            basicProperties: replyProperties,
                            body: responseBody);
                        
                        _logger.LogDebug("Response sent for message with correlation ID {CorrelationId}", ea.BasicProperties.CorrelationId);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing message on queue {QueueName}", queueName);
                    
                    // Send error response if reply-to is set
                    if (!string.IsNullOrEmpty(ea.BasicProperties.ReplyTo))
                    {
                        var errorResponse = new
                        {
                            Success = false,
                            Message = "Error processing message",
                            Errors = new[] { ex.Message }
                        };
                        var errorJson = JsonSerializer.Serialize(errorResponse, _jsonOptions);
                        var errorBody = Encoding.UTF8.GetBytes(errorJson);
                        
                        _channel.BasicPublish(
                            exchange: "",
                            routingKey: ea.BasicProperties.ReplyTo,
                            basicProperties: replyProperties,
                            body: errorBody);
                    }
                }
                finally
                {
                    _channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                }
            };

            _channel.BasicConsume(queue: queueName, autoAck: false, consumer: consumer);
            _logger.LogInformation("Started listening on queue {QueueName}", queueName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error starting listener on queue {QueueName}", queueName);
            throw;
        }
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            _channel?.Close();
            _channel?.Dispose();
            _connection?.Close();
            _connection?.Dispose();
            _disposed = true;
            _logger.LogInformation("RabbitMQ connection closed");
        }
    }
}
