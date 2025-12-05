using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Shared.Common;
using System.Text;

namespace Shared.Filters;

/// <summary>
/// Global validation filter that automatically validates all incoming requests using Data Annotations.
/// This filter mimics NestJS's ValidationPipe behavior by:
/// 1. Checking ModelState.IsValid before controller actions execute
/// 2. Formatting validation errors into user-friendly messages
/// 3. Returning consistent ApiResponse format
/// 
/// This eliminates the need for manual ModelState.IsValid checks in every controller.
/// </summary>
public class ValidateModelAttribute : ActionFilterAttribute
{
    /// <summary>
    /// Executes before the controller action method is called.
    /// This is where we intercept the request and validate the model.
    /// </summary>
    /// <param name="context">The action executing context containing ModelState and request information</param>
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        // Check if ModelState is valid
        // ASP.NET Core automatically populates ModelState when:
        // 1. Request body is bound to a DTO with [FromBody]
        // 2. DTO has Data Annotations attributes ([Required], [EmailAddress], etc.)
        // 3. Controller has [ApiController] attribute (which enables automatic model validation)
        if (!context.ModelState.IsValid)
        {
            // Extract all validation errors from ModelState
            // ModelState contains errors for each property that failed validation
            var errors = ExtractValidationErrors(context.ModelState);

            // Create a formatted error message
            // Similar to NestJS's exception factory, we format errors into a readable string
            var errorMessage = FormatErrorMessage(errors);

            // Return BadRequest (400) with ApiResponse format
            // This ensures consistent error response format across all endpoints
            context.Result = new BadRequestObjectResult(
                ApiResponse<object>.ErrorResponse(
                    message: "Validation failed",
                    errors: errors
                )
            );

            // Set the HTTP status code explicitly
            // BadRequestObjectResult already sets 400, but we're being explicit
            if (context.HttpContext.Response != null)
            {
                context.HttpContext.Response.StatusCode = 400;
            }
        }

        // If ModelState is valid, the filter does nothing and the request proceeds to the controller action
        // This is the default behavior - we only intercept when validation fails
    }

    /// <summary>
    /// Extracts validation errors from ModelState and formats them into a list of error messages.
    /// This method handles:
    /// - Simple property errors (e.g., "Email is required")
    /// - Nested object errors (e.g., "Name is required (in User)")
    /// - Multiple errors for the same property
    /// 
    /// Similar to NestJS's extractErrorMessages function.
    /// </summary>
    /// <param name="modelState">The ModelStateDictionary containing validation errors</param>
    /// <returns>List of formatted error messages</returns>
    private static List<string> ExtractValidationErrors(Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateDictionary modelState)
    {
        var errors = new List<string>();

        // Iterate through all entries in ModelState
        // Each entry represents a property (or nested property) that has validation errors
        foreach (var entry in modelState)
        {
            var propertyName = entry.Key;
            var state = entry.Value;

            // Check if this property has any validation errors
            if (state?.Errors != null && state.Errors.Count > 0)
            {
                // Process each error for this property
                foreach (var error in state.Errors)
                {
                    // Format the error message
                    // Property names are converted to a more readable format
                    // Example: "firstName" -> "First Name"
                    var formattedPropertyName = FormatPropertyName(propertyName);
                    var errorMessage = FormatSingleError(formattedPropertyName, error.ErrorMessage);

                    errors.Add(errorMessage);
                }
            }
        }

        return errors;
    }

    /// <summary>
    /// Formats a single validation error message.
    /// Converts technical error messages to user-friendly ones.
    /// Similar to NestJS's error message transformation.
    /// </summary>
    /// <param name="propertyName">The name of the property that failed validation (already formatted)</param>
    /// <param name="errorMessage">The original error message from Data Annotations</param>
    /// <returns>Formatted, user-friendly error message</returns>
    private static string FormatSingleError(string propertyName, string errorMessage)
    {
        // If error message is empty, provide a default message
        if (string.IsNullOrWhiteSpace(errorMessage))
        {
            return $"{propertyName} is invalid";
        }

        // Replace common technical messages with user-friendly ones
        // This mimics NestJS's error message transformation
        var formatted = errorMessage
            .Replace("field", propertyName, StringComparison.OrdinalIgnoreCase)
            .Replace("is required", $"{propertyName} is required", StringComparison.OrdinalIgnoreCase)
            .Replace("should not be empty", $"{propertyName} is required", StringComparison.OrdinalIgnoreCase)
            .Replace("must not be empty", $"{propertyName} is required", StringComparison.OrdinalIgnoreCase);

        // If the error message doesn't already include the property name, prepend it
        // This ensures all error messages are clear about which field has the issue
        if (!formatted.Contains(propertyName, StringComparison.OrdinalIgnoreCase))
        {
            formatted = $"{propertyName}: {formatted}";
        }

        // Capitalize first letter for better readability
        if (formatted.Length > 0)
        {
            formatted = char.ToUpper(formatted[0]) + formatted.Substring(1);
        }

        return formatted;
    }

    /// <summary>
    /// Formats property names from camelCase/PascalCase to a more readable format.
    /// Examples:
    /// - "firstName" -> "First Name"
    /// - "emailAddress" -> "Email Address"
    /// - "dateOfBirth" -> "Date Of Birth"
    /// 
    /// This improves error message readability for end users.
    /// </summary>
    /// <param name="propertyName">The original property name (may be camelCase, PascalCase, or with dots for nested properties)</param>
    /// <returns>Formatted property name with spaces and proper capitalization</returns>
    private static string FormatPropertyName(string propertyName)
    {
        if (string.IsNullOrWhiteSpace(propertyName))
        {
            return "Field";
        }

        // Handle nested properties (e.g., "User.Email" -> "Email (in User)")
        if (propertyName.Contains('.'))
        {
            var parts = propertyName.Split('.');
            var parent = FormatPropertyName(parts[0]);
            var child = FormatPropertyName(parts[^1]);
            return $"{child} (in {parent})";
        }

        // Convert camelCase/PascalCase to "Title Case"
        // Example: "firstName" -> "First Name"
        var result = new StringBuilder();
        for (int i = 0; i < propertyName.Length; i++)
        {
            var currentChar = propertyName[i];

            // If this is an uppercase letter and not the first character, add a space before it
            if (char.IsUpper(currentChar) && i > 0)
            {
                result.Append(' ');
            }

            // Add the character (uppercase for first character, lowercase for others in the word)
            if (i == 0)
            {
                result.Append(char.ToUpper(currentChar));
            }
            else if (char.IsUpper(currentChar) && i > 0 && !char.IsUpper(propertyName[i - 1]))
            {
                // If previous char was lowercase and current is uppercase, it's a new word
                result.Append(currentChar);
            }
            else
            {
                result.Append(char.ToLower(currentChar));
            }
        }

        return result.ToString();
    }

    /// <summary>
    /// Formats the complete error message from a list of individual errors.
    /// Creates a single string that can be used as the main error message,
    /// while individual errors are also included in the Errors list.
    /// </summary>
    /// <param name="errors">List of individual validation error messages</param>
    /// <returns>Formatted error message string</returns>
    private static string FormatErrorMessage(List<string> errors)
    {
        if (errors == null || errors.Count == 0)
        {
            return "Validation failed";
        }

        if (errors.Count == 1)
        {
            return errors[0];
        }

        // For multiple errors, create a summary message
        return $"Validation failed: {string.Join(", ", errors)}";
    }
}



