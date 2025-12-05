using MongoDB.Bson;
using System.Text.Json;

namespace Shared.Transformers;

/// <summary>
/// Common property transformers for typical use cases.
/// These transformers can be used with the [Transform] attribute.
/// </summary>
public static class CommonTransformers
{
    /// <summary>
    /// Trims whitespace from string values.
    /// Example: "  John  " → "John"
    /// </summary>
    public class TrimTransformer : IPropertyTransformer
    {
        public object? Transform(object? value, string propertyName)
        {
            if (value is string str)
            {
                return str.Trim();
            }
            return value;
        }
    }

    /// <summary>
    /// Converts string values to lowercase.
    /// Example: "John@Example.com" → "john@example.com"
    /// </summary>
    public class ToLowerTransformer : IPropertyTransformer
    {
        public object? Transform(object? value, string propertyName)
        {
            if (value is string str)
            {
                return str.ToLowerInvariant();
            }
            return value;
        }
    }

    /// <summary>
    /// Converts string values to uppercase.
    /// Example: "admin" → "ADMIN"
    /// </summary>
    public class ToUpperTransformer : IPropertyTransformer
    {
        public object? Transform(object? value, string propertyName)
        {
            if (value is string str)
            {
                return str.ToUpperInvariant();
            }
            return value;
        }
    }

    /// <summary>
    /// Converts empty strings to null.
    /// Example: "" → null
    /// </summary>
    public class EmptyToNullTransformer : IPropertyTransformer
    {
        public object? Transform(object? value, string propertyName)
        {
            if (value is string str && string.IsNullOrWhiteSpace(str))
            {
                return null;
            }
            return value;
        }
    }

    /// <summary>
    /// Converts string MongoDB ObjectId to ObjectId instance.
    /// Example: "507f1f77bcf86cd799439011" → ObjectId instance
    /// </summary>
    public class ToMongoObjectIdTransformer : IPropertyTransformer
    {
        public object? Transform(object? value, string propertyName)
        {
            if (value == null)
            {
                return null;
            }

            // If already ObjectId, return as-is
            if (value is ObjectId)
            {
                return value;
            }

            // Convert string to ObjectId
            if (value is string str)
            {
                if (string.IsNullOrWhiteSpace(str))
                {
                    return null;
                }

                if (ObjectId.TryParse(str, out var objectId))
                {
                    return objectId;
                }

                throw new ArgumentException($"'{propertyName}' is not a valid MongoDB ObjectId");
            }

            // Handle arrays of ObjectIds
            if (value is string[] stringArray)
            {
                var objectIds = new List<ObjectId>();
                foreach (var item in stringArray)
                {
                    if (ObjectId.TryParse(item, out var objId))
                    {
                        objectIds.Add(objId);
                    }
                    else
                    {
                        throw new ArgumentException($"'{item}' in '{propertyName}' is not a valid MongoDB ObjectId");
                    }
                }
                return objectIds.ToArray();
            }

            return value;
        }
    }

    /// <summary>
    /// Parses JSON string to object.
    /// Example: '{"key":"value"}' → object
    /// </summary>
    public class JsonStringToObjectTransformer : IPropertyTransformer
    {
        public object? Transform(object? value, string propertyName)
        {
            if (value is string str && !string.IsNullOrWhiteSpace(str))
            {
                try
                {
                    return JsonSerializer.Deserialize<object>(str);
                }
                catch (JsonException)
                {
                    throw new ArgumentException($"'{propertyName}' is not a valid JSON string");
                }
            }
            return value;
        }
    }

    /// <summary>
    /// Removes null or empty values from arrays.
    /// Example: [1, null, 2, "", 3] → [1, 2, 3]
    /// </summary>
    public class RemoveEmptyArrayItemsTransformer : IPropertyTransformer
    {
        public object? Transform(object? value, string propertyName)
        {
            if (value is System.Collections.IEnumerable enumerable && value is not string)
            {
                var list = new List<object?>();
                foreach (var item in enumerable)
                {
                    if (item != null && !(item is string str && string.IsNullOrWhiteSpace(str)))
                    {
                        list.Add(item);
                    }
                }
                return list.ToArray();
            }
            return value;
        }
    }
}



