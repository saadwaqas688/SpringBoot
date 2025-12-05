using System.ComponentModel.DataAnnotations;

namespace Shared.Application.Options;

/// <summary>
/// Configuration options for RabbitMQ connection.
/// </summary>
public class RabbitMQSettings
{
    public const string SectionName = "RabbitMQ";

    /// <summary>
    /// RabbitMQ server hostname.
    /// </summary>
    [Required(ErrorMessage = "RabbitMQ HostName is required")]
    public string HostName { get; set; } = "localhost";

    /// <summary>
    /// RabbitMQ server port. Default is 5672.
    /// </summary>
    [Range(1, 65535, ErrorMessage = "RabbitMQ Port must be between 1 and 65535")]
    public int Port { get; set; } = 5672;

    /// <summary>
    /// RabbitMQ username.
    /// </summary>
    [Required(ErrorMessage = "RabbitMQ UserName is required")]
    public string UserName { get; set; } = "guest";

    /// <summary>
    /// RabbitMQ password.
    /// </summary>
    [Required(ErrorMessage = "RabbitMQ Password is required")]
    public string Password { get; set; } = "guest";
}

