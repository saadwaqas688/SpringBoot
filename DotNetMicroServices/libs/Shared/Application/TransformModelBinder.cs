using Microsoft.AspNetCore.Mvc.ModelBinding;
using Shared.Application.Attributes;
using Shared.Application.Transformers;
using System.Reflection;

namespace Shared.Application.ModelBinders;

/// <summary>
/// Custom model binder that applies transformations to properties marked with [Transform] attribute.
/// This binder runs after the default model binding and applies transformations before validation.
/// 
/// Similar to NestJS's class-transformer behavior where @Transform decorators are applied
/// before class-validator decorators.
/// </summary>
public class TransformModelBinder : IModelBinder
{
    /// <summary>
    /// Binds the model and applies transformations to properties with [Transform] attribute.
    /// </summary>
    /// <param name="bindingContext">The model binding context</param>
    /// <returns>Task representing the async operation</returns>
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        // Get the model instance (already bound by default binder)
        var model = bindingContext.Result.Model;
        
        if (model == null)
        {
            return Task.CompletedTask;
        }

        // Apply transformations to all properties with [Transform] attribute
        ApplyTransformations(model, bindingContext.ModelMetadata);

        // Mark as successful (transformations applied)
        bindingContext.Result = ModelBindingResult.Success(model);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Recursively applies transformations to the model and its properties.
    /// </summary>
    /// <param name="model">The model instance to transform</param>
    /// <param name="modelMetadata">Metadata about the model</param>
    private void ApplyTransformations(object model, ModelMetadata modelMetadata)
    {
        if (model == null)
        {
            return;
        }

        var modelType = model.GetType();

        // Get all properties of the model
        var properties = modelType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var property in properties)
        {
            // Check if property has [Transform] attributes (can have multiple)
            var transformAttributes = property.GetCustomAttributes<TransformAttribute>().ToList();
            if (transformAttributes.Count == 0)
            {
                // If no transform attribute, check nested objects
                var propertyValue = property.GetValue(model);
                if (propertyValue != null && IsComplexType(property.PropertyType))
                {
                    ApplyTransformations(propertyValue, modelMetadata);
                }
                continue;
            }

            // Get current property value
            var currentValue = property.GetValue(model);

            // Apply all transformers in order (first to last)
            object? transformedValue = currentValue;
            foreach (var transformAttribute in transformAttributes)
            {
                // Create transformer instance
                IPropertyTransformer? transformer = null;
                try
                {
                    if (transformAttribute.Parameters != null && transformAttribute.Parameters.Length > 0)
                    {
                        transformer = (IPropertyTransformer?)Activator.CreateInstance(
                            transformAttribute.TransformerType,
                            transformAttribute.Parameters);
                    }
                    else
                    {
                        transformer = (IPropertyTransformer?)Activator.CreateInstance(
                            transformAttribute.TransformerType);
                    }
                }
                catch (Exception ex)
                {
                    // Log error but don't fail - transformation is optional
                    // In production, you might want to log this
                    continue;
                }

                if (transformer == null)
                {
                    continue;
                }

                // Apply transformation
                try
                {
                    transformedValue = transformer.Transform(transformedValue, property.Name);
                }
                catch (Exception ex)
                {
                    // If transformation fails, we could add to ModelState errors
                    // For now, we'll let validation handle it
                    // In production, you might want to add custom error handling
                    break;
                }
            }

            // Set final transformed value back to property
            if (property.CanWrite)
            {
                property.SetValue(model, transformedValue);
            }
        }
    }

    /// <summary>
    /// Checks if a type is a complex type (not a primitive or simple type).
    /// </summary>
    private bool IsComplexType(Type type)
    {
        if (type == typeof(string) || type.IsPrimitive || type.IsValueType)
        {
            return false;
        }

        if (type.IsArray || (type.IsGenericType && typeof(System.Collections.IEnumerable).IsAssignableFrom(type)))
        {
            return false;
        }

        return true;
    }
}

