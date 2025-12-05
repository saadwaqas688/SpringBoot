# Field Transformation - Quick Summary

## What is it?

Field transformation allows you to automatically modify property values **before validation**, similar to NestJS's `@Transform` decorator.

## How to Use

### Basic Example

```csharp
using Shared.Attributes;
using Shared.Transformers;

public class CreateUserDto
{
    // Trim whitespace before validation
    [Required]
    [Transform(typeof(CommonTransformers.TrimTransformer))]
    public string FirstName { get; set; } = string.Empty;

    // Trim + Lowercase email before validation
    [Required]
    [EmailAddress]
    [Transform(typeof(CommonTransformers.TrimTransformer))]
    [Transform(typeof(CommonTransformers.ToLowerTransformer))]
    public string Email { get; set; } = string.Empty;
}
```

## Available Transformers

| Transformer                        | Description                         | Example                                     |
| ---------------------------------- | ----------------------------------- | ------------------------------------------- |
| `TrimTransformer`                  | Trims whitespace                    | `"  John  "` → `"John"`                     |
| `ToLowerTransformer`               | Converts to lowercase               | `"John@Example.COM"` → `"john@example.com"` |
| `ToUpperTransformer`               | Converts to uppercase               | `"admin"` → `"ADMIN"`                       |
| `EmptyToNullTransformer`           | Converts empty strings to null      | `""` → `null`                               |
| `ToMongoObjectIdTransformer`       | Converts string to MongoDB ObjectId | `"507f1f77..."` → `ObjectId`                |
| `JsonStringToObjectTransformer`    | Parses JSON string                  | `'{"key":"value"}'` → `object`              |
| `RemoveEmptyArrayItemsTransformer` | Removes empty array items           | `[1, null, 2]` → `[1, 2]`                   |

## Multiple Transforms

Transformers execute **in order** (top to bottom):

```csharp
[Transform(typeof(CommonTransformers.TrimTransformer))]      // 1. Trim first
[Transform(typeof(CommonTransformers.ToLowerTransformer))]   // 2. Then lowercase
public string Email { get; set; }
```

## Execution Order

```
1. HTTP Request arrives
   ↓
2. Model Binding (JSON → DTO)
   ↓
3. Transformations Applied ← [Transform] attributes execute here
   ↓
4. Validation ← Data Annotations validate transformed values
   ↓
5. Controller Action
```

## Common Use Cases

### Email Normalization

```csharp
[Required]
[EmailAddress]
[Transform(typeof(CommonTransformers.TrimTransformer))]
[Transform(typeof(CommonTransformers.ToLowerTransformer))]
public string Email { get; set; } = string.Empty;
```

### String Trimming

```csharp
[Required]
[StringLength(255)]
[Transform(typeof(CommonTransformers.TrimTransformer))]
public string Name { get; set; } = string.Empty;
```

### MongoDB ObjectId

```csharp
[Required]
[Transform(typeof(CommonTransformers.ToMongoObjectIdTransformer))]
public ObjectId CourseId { get; set; }
```

## Registration

Already registered in all services:

- ✅ `UserAccountService/Program.cs`
- ✅ `Gateway/Program.cs`
- ✅ `CoursesService/Program.cs`

No additional setup needed!

## NestJS Comparison

| NestJS                                    | .NET                                              |
| ----------------------------------------- | ------------------------------------------------- |
| `@Transform(toMongoObjectId)`             | `[Transform(typeof(ToMongoObjectIdTransformer))]` |
| `@Transform(({ value }) => value.trim())` | `[Transform(typeof(TrimTransformer))]`            |
| Multiple decorators                       | Multiple attributes                               |

## Hybrid Approach

We use a **hybrid approach**:

- **Property Setters** for simple transformations (trim, lowercase) - Most idiomatic .NET
- **Model Binders** for complex transformations (ObjectId, JSON parsing) - Flexible and reusable

### Simple Transformations (Property Setters)

```csharp
private string _email = string.Empty;
public string Email
{
    get => _email;
    set => _email = value?.Trim().ToLowerInvariant() ?? string.Empty;
}
```

### Complex Transformations (Model Binder)

```csharp
[Transform(typeof(CommonTransformers.ToMongoObjectIdTransformer))]
public ObjectId CourseId { get; set; }
```

## Full Documentation

- `TRANSFORM_IMPLEMENTATION.md` - Detailed implementation guide
- `TRANSFORM_HYBRID_APPROACH.md` - Hybrid approach explanation
- `TRANSFORM_PRACTICE_ANALYSIS.md` - Best practices analysis
