using Microsoft.Extensions.Options;
using Shared.Application.Options;

namespace Shared.Application.Validators;

/// <summary>
/// Validator for RabbitMQ settings configuration.
/// </summary>
public class RabbitMQSettingsValidator : IValidateOptions<RabbitMQSettings>
{
    public ValidateOptionsResult Validate(string? name, RabbitMQSettings options)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(options.HostName))
        {
            errors.Add("RabbitMQ HostName is required");
        }

        if (options.Port < 1 || options.Port > 65535)
        {
            errors.Add("RabbitMQ Port must be between 1 and 65535");
        }

        if (string.IsNullOrWhiteSpace(options.UserName))
        {
            errors.Add("RabbitMQ UserName is required");
        }

        if (string.IsNullOrWhiteSpace(options.Password))
        {
            errors.Add("RabbitMQ Password is required");
        }

        if (errors.Count > 0)
        {
            return ValidateOptionsResult.Fail(errors);
        }

        return ValidateOptionsResult.Success;
    }
}

