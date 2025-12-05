namespace Shared.Core.Common;

public class RabbitMQMessage<T>
{
    public string RoutingKey { get; set; } = string.Empty;
    public T? Data { get; set; }
}



