using System.Text;

namespace Shared.Application.Filters;

/// <summary>
/// Helper class for validation-related utilities.
/// Provides static methods for formatting validation errors and property names.
/// This class can be used independently or by the ValidateModelAttribute filter.
/// </summary>
public static class ValidationHelper
{
    /// <summary>
    /// Converts a property name from camelCase/PascalCase to a human-readable format.
    /// This is useful for displaying field names in error messages.
    /// 
    /// Examples:
    /// - "firstName" -> "First Name"
    /// - "emailAddress" -> "Email Address"
    /// - "dateOfBirth" -> "Date Of Birth"
    /// - "userId" -> "User Id"
    /// </summary>
    /// <param name="propertyName">The property name to format (camelCase, PascalCase, or snake_case)</param>
    /// <returns>Formatted property name with spaces and proper capitalization</returns>
    public static string ToTitleCase(string propertyName)
    {
        if (string.IsNullOrWhiteSpace(propertyName))
        {
            return "Field";
        }

        // Handle nested properties (e.g., "User.Email" -> "Email (in User)")
        if (propertyName.Contains('.'))
        {
            var parts = propertyName.Split('.');
            var parent = ToTitleCase(parts[0]);
            var child = ToTitleCase(parts[^1]);
            return $"{child} (in {parent})";
        }

        // Handle snake_case
        if (propertyName.Contains('_'))
        {
            var parts = propertyName.Split('_');
            return string.Join(" ", parts.Select(ToTitleCase));
        }

        // Convert camelCase/PascalCase to "Title Case"
        var result = new StringBuilder();
        for (int i = 0; i < propertyName.Length; i++)
        {
            var currentChar = propertyName[i];

            // If this is an uppercase letter and not the first character, add a space before it
            if (char.IsUpper(currentChar) && i > 0)
            {
                // Check if previous character was lowercase (new word boundary)
                if (char.IsLower(propertyName[i - 1]))
                {
                    result.Append(' ');
                }
            }

            // Add the character with appropriate casing
            if (i == 0)
            {
                result.Append(char.ToUpper(currentChar));
            }
            else
            {
                result.Append(currentChar);
            }
        }

        return result.ToString();
    }

    /// <summary>
    /// Capitalizes the first letter of a string.
    /// Useful for formatting error messages.
    /// </summary>
    /// <param name="text">The text to capitalize</param>
    /// <returns>Text with first letter capitalized</returns>
    public static string CapitalizeFirst(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return text;
        }

        return char.ToUpper(text[0]) + text.Substring(1).ToLower();
    }
}



