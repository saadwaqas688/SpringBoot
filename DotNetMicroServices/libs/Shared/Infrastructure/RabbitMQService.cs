using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace Shared.Infrastructure.Services;

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
        IConnection? connection = null;
        IModel? channel = null;
        
        for (int attempt = 1; attempt <= maxRetries; attempt++)
        {
            try
            {
                connection = factory.CreateConnection();
                channel = connection.CreateModel();
                
                // Declare exchange
                channel.ExchangeDeclare(_exchangeName, ExchangeType.Topic, durable: true);
                
                _connection = connection;
                _channel = channel;
                
                _logger.LogInformation("RabbitMQ connection established");
                return;
            }
            catch (Exception ex)
            {
                // Clean up on failure
                channel?.Close();
                channel?.Dispose();
                connection?.Close();
                connection?.Dispose();
                
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
        
        // This should never be reached, but compiler needs it
        throw new InvalidOperationException("Failed to initialize RabbitMQ connection");
    }

    public async Task<T?> SendMessageAsync<T>(string queueName, string routingKey, object message)
    {
        try
        {
            // Ensure queue exists (listener should have already created and bound it)
            _channel.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);

            // Create response queue (temporary, auto-delete)
            var replyQueueName = _channel.QueueDeclare(exclusive: true, autoDelete: true).QueueName;
            var correlationId = Guid.NewGuid().ToString();
            var tcs = new TaskCompletionSource<T?>();
            var consumer = new EventingBasicConsumer(_channel);
            string? consumerTag = null;

            // Set up response consumer
            consumer.Received += (model, ea) =>
            {
                _logger.LogInformation("Received response message. Correlation ID: {ReceivedCorrelationId}, Expected: {ExpectedCorrelationId}, Queue: {Queue}", 
                    ea.BasicProperties.CorrelationId, correlationId, replyQueueName);
                
                if (ea.BasicProperties.CorrelationId == correlationId)
                {
                    var body = ea.Body.ToArray();
                    var responseJson = Encoding.UTF8.GetString(body);
                    
                    _logger.LogInformation("Response received for correlation ID {CorrelationId}. Response length: {Length}", 
                        correlationId, responseJson.Length);
                    
                    try
                    {
                        var response = JsonSerializer.Deserialize<T>(responseJson, _jsonOptions);
                        tcs.SetResult(response);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to deserialize response. Response JSON: {ResponseJson}", responseJson);
                        tcs.SetResult(default(T));
                    }
                }
                else
                {
                    _logger.LogWarning("Ignoring response with mismatched correlation ID. Received: {Received}, Expected: {Expected}", 
                        ea.BasicProperties.CorrelationId, correlationId);
                }
            };

            consumerTag = _channel.BasicConsume(queue: replyQueueName, autoAck: true, consumer: consumer);
            _logger.LogInformation("Reply queue consumer started. Queue: {ReplyQueue}, ConsumerTag: {ConsumerTag}, CorrelationId: {CorrelationId}", 
                replyQueueName, consumerTag, correlationId);

            // Small delay to ensure consumer is ready (helps with timing issues)
            await Task.Delay(50);

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

            _logger.LogInformation("Message sent to exchange {Exchange} with routing key {RoutingKey}, correlation ID: {CorrelationId}, reply queue: {ReplyQueue}", 
                _exchangeName, routingKey, correlationId, replyQueueName);

            // Wait for response with timeout
            var timeoutTask = Task.Delay(TimeSpan.FromSeconds(30));
            var completedTask = await Task.WhenAny(tcs.Task, timeoutTask);

            if (completedTask == timeoutTask)
            {
                _logger.LogWarning("Timeout waiting for response from {QueueName} with routing key {RoutingKey}. Correlation ID: {CorrelationId}", 
                    queueName, routingKey, correlationId);
                
                // Cancel consumer if still active
                if (!string.IsNullOrEmpty(consumerTag))
                {
                    try
                    {
                        _channel.BasicCancel(consumerTag);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Error canceling consumer");
                    }
                }
                
                return default(T);
            }

            var result = await tcs.Task;
            
            // Cancel consumer after receiving response
            if (!string.IsNullOrEmpty(consumerTag))
            {
                try
                {
                    _channel.BasicCancel(consumerTag);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Error canceling consumer after response");
                }
            }
            
            return result;
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
                    _logger.LogInformation("Received message on queue {QueueName} with routing key {RoutingKey}. Correlation ID: {CorrelationId}, ReplyTo: {ReplyTo}", 
                        queueName, routingKey, ea.BasicProperties.CorrelationId, ea.BasicProperties.ReplyTo);
                    
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
                        
                        _logger.LogInformation("Response sent to queue {ReplyQueue} for correlation ID {CorrelationId}. Response length: {Length}", 
                            ea.BasicProperties.ReplyTo, ea.BasicProperties.CorrelationId, responseJson.Length);
                    }
                    else
                    {
                        _logger.LogWarning("Response is null or ReplyTo is empty. Response: {IsNull}, ReplyTo: {ReplyTo}", 
                            response == null, ea.BasicProperties.ReplyTo);
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
