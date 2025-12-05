using System;

namespace Shared.Application.Attributes;

/// <summary>
/// Attribute that transforms a property value during model binding.
/// Similar to NestJS's @Transform decorator from class-transformer.
/// 
/// Usage:
/// [Transform(typeof(TrimTransformer))]
/// public string Name { get; set; }
/// 
/// Multiple transforms can be applied (they execute in order):
/// [Transform(typeof(TrimTransformer))]
/// [Transform(typeof(ToLowerTransformer))]
/// public string Email { get; set; }
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
public class TransformAttribute : Attribute
{
    /// <summary>
    /// The type of transformer to use. Must implement IPropertyTransformer.
    /// </summary>
    public Type TransformerType { get; }

    /// <summary>
    /// Optional parameters to pass to the transformer constructor.
    /// </summary>
    public object[]? Parameters { get; }

    /// <summary>
    /// Initializes a new instance of the TransformAttribute.
    /// </summary>
    /// <param name="transformerType">The type of transformer to use (must implement IPropertyTransformer)</param>
    /// <param name="parameters">Optional parameters for the transformer constructor</param>
    public TransformAttribute(Type transformerType, params object[]? parameters)
    {
        TransformerType = transformerType;
        Parameters = parameters;
    }
}

