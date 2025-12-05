using Microsoft.Extensions.Options;
using Shared.Application.Options;

namespace Shared.Application.Validators;

/// <summary>
/// Validator for JWT settings configuration.
/// </summary>
public class JwtSettingsValidator : IValidateOptions<JwtSettings>
{
    public ValidateOptionsResult Validate(string? name, JwtSettings options)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(options.SecretKey))
        {
            errors.Add("JWT SecretKey is required");
        }
        else if (options.SecretKey.Length < 32)
        {
            errors.Add("JWT SecretKey must be at least 32 characters long for security");
        }

        if (string.IsNullOrWhiteSpace(options.Issuer))
        {
            errors.Add("JWT Issuer is required");
        }

        if (string.IsNullOrWhiteSpace(options.Audience))
        {
            errors.Add("JWT Audience is required");
        }

        if (options.ExpirationMinutes < 1 || options.ExpirationMinutes > 1440)
        {
            errors.Add("JWT ExpirationMinutes must be between 1 and 1440 minutes (24 hours)");
        }

        if (errors.Count > 0)
        {
            return ValidateOptionsResult.Fail(errors);
        }

        return ValidateOptionsResult.Success;
    }
}

