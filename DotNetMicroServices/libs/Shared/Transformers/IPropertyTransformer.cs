namespace Shared.Transformers;

/// <summary>
/// Interface for property transformers that modify property values during model binding.
/// Similar to NestJS's Transform function signature.
/// 
/// Transformers are applied before validation, allowing you to:
/// - Trim strings
/// - Convert to lowercase/uppercase
/// - Convert string IDs to MongoDB ObjectId
/// - Parse JSON strings to objects
/// - Apply any custom transformation logic
/// </summary>
public interface IPropertyTransformer
{
    /// <summary>
    /// Transforms the input value.
    /// </summary>
    /// <param name="value">The original value from the request</param>
    /// <param name="propertyName">The name of the property being transformed (for error messages)</param>
    /// <returns>The transformed value</returns>
    object? Transform(object? value, string propertyName);
}



