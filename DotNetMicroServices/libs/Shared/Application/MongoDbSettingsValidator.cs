using Microsoft.Extensions.Options;
using Shared.Application.Options;

namespace Shared.Application.Validators;

/// <summary>
/// Validator for MongoDB settings configuration.
/// </summary>
public class MongoDbSettingsValidator : IValidateOptions<MongoDbSettings>
{
    public ValidateOptionsResult Validate(string? name, MongoDbSettings options)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(options.ConnectionString))
        {
            errors.Add("MongoDB ConnectionString is required");
        }

        if (string.IsNullOrWhiteSpace(options.DatabaseName))
        {
            errors.Add("MongoDB DatabaseName is required");
        }

        if (errors.Count > 0)
        {
            return ValidateOptionsResult.Fail(errors);
        }

        return ValidateOptionsResult.Success;
    }
}

