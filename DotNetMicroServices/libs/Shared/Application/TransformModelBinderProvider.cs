using Microsoft.AspNetCore.Mvc.ModelBinding;
using Shared.Application.Attributes;
using System.Reflection;

namespace Shared.Application.ModelBinders;

/// <summary>
/// Model binder provider that creates TransformModelBinder for models that have properties with [Transform] attribute.
/// This provider is registered in Program.cs to enable automatic transformation.
/// </summary>
public class TransformModelBinderProvider : IModelBinderProvider
{
    /// <summary>
    /// Gets a model binder for the specified context if the model has properties with [Transform] attribute.
    /// </summary>
    /// <param name="context">The model binder provider context</param>
    /// <returns>A model binder if the model needs transformation, otherwise null</returns>
    public IModelBinder? GetBinder(ModelBinderProviderContext context)
    {
        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        // Only apply to complex types (not primitives, strings, etc.)
        if (!context.Metadata.IsComplexType)
        {
            return null;
        }

        // Check if the model type has any properties with [Transform] attribute
        var modelType = context.Metadata.ModelType;
        var hasTransformAttributes = modelType
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Any(p => p.GetCustomAttribute<TransformAttribute>() != null);

        // If model has [Transform] attributes, use our custom binder
        if (hasTransformAttributes)
        {
            return new TransformModelBinder();
        }

        return null; // Use default binder
    }
}



