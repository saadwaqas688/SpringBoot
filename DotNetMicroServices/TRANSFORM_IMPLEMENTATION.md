# Field Transformation Implementation - .NET (NestJS-Style)

This document explains the field transformation implementation that mimics NestJS's `@Transform` decorator from `class-transformer`.

## Overview

We've implemented **automatic field transformation** using a custom model binder that applies transformations to properties marked with `[Transform]` attribute. This allows you to transform values before validation, similar to NestJS's `@Transform` decorator.

---

## Architecture

### 1. **TransformAttribute**

**Location:** `libs/Shared/Attributes/TransformAttribute.cs`

A custom attribute that marks properties for transformation:

```csharp
[Transform(typeof(CommonTransformers.TrimTransformer))]
public string Name { get; set; }
```

**Features:**

- ✅ Multiple transforms per property (executed in order)
- ✅ Custom transformer support
- ✅ Similar syntax to NestJS `@Transform`

### 2. **IPropertyTransformer Interface**

**Location:** `libs/Shared/Transformers/IPropertyTransformer.cs`

Interface that all transformers must implement:

```csharp
public interface IPropertyTransformer
{
    object? Transform(object? value, string propertyName);
}
```

### 3. **CommonTransformers**

**Location:** `libs/Shared/Transformers/CommonTransformers.cs`

Pre-built transformers for common use cases:

- `TrimTransformer` - Trims whitespace
- `ToLowerTransformer` - Converts to lowercase
- `ToUpperTransformer` - Converts to uppercase
- `EmptyToNullTransformer` - Converts empty strings to null
- `ToMongoObjectIdTransformer` - Converts string to MongoDB ObjectId
- `JsonStringToObjectTransformer` - Parses JSON strings
- `RemoveEmptyArrayItemsTransformer` - Removes empty items from arrays

### 4. **TransformModelBinder**

**Location:** `libs/Shared/ModelBinders/TransformModelBinder.cs`

Custom model binder that applies transformations during model binding (before validation).

---

## How It Works

### Request Flow

```
1. HTTP Request arrives
   ↓
2. Default Model Binding
   - JSON deserialized to DTO instance
   ↓
3. TransformModelBinder executes
   - Finds properties with [Transform] attribute
   - Applies transformers in order
   - Updates property values
   ↓
4. ValidateModelAttribute executes
   - Validates transformed values
   ↓
5. Controller Action
   - Receives transformed and validated DTO
```

**Key Point:** Transformations happen **before validation**, just like in NestJS.

---

## Usage Examples

### Example 1: Trim and Lowercase Email

```csharp
public class CreateUserDto
{
    [Required]
    [EmailAddress]
    [Transform(typeof(CommonTransformers.TrimTransformer))]
    [Transform(typeof(CommonTransformers.ToLowerTransformer))] // Applied after Trim
    public string Email { get; set; } = string.Empty;
}
```

**Request:**

```json
{
  "email": "  John@Example.COM  "
}
```

**After Transformation:**

```csharp
Email = "john@example.com"  // Trimmed and lowercased
```

**After Validation:**

- ✅ Email format is valid
- ✅ No whitespace issues

### Example 2: Trim String Fields

```csharp
public class CreateUserDto
{
    [Required]
    [StringLength(255)]
    [Transform(typeof(CommonTransformers.TrimTransformer))]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [StringLength(255)]
    [Transform(typeof(CommonTransformers.TrimTransformer))]
    public string LastName { get; set; } = string.Empty;
}
```

**Request:**

```json
{
  "firstName": "  John  ",
  "lastName": "  Doe  "
}
```

**After Transformation:**

```csharp
FirstName = "John"  // Trimmed
LastName = "Doe"     // Trimmed
```

### Example 3: Convert String to MongoDB ObjectId

```csharp
public class GetCourseDto
{
    [Required]
    [Transform(typeof(CommonTransformers.ToMongoObjectIdTransformer))]
    public ObjectId CourseId { get; set; }
}
```

**Request:**

```json
{
  "courseId": "507f1f77bcf86cd799439011"
}
```

**After Transformation:**

```csharp
CourseId = ObjectId("507f1f77bcf86cd799439011")  // Converted to ObjectId
```

### Example 4: Empty String to Null

```csharp
public class UpdateUserDto
{
    [StringLength(20)]
    [Transform(typeof(CommonTransformers.EmptyToNullTransformer))]
    public string? MobilePhone { get; set; }
}
```

**Request:**

```json
{
  "mobilePhone": ""
}
```

**After Transformation:**

```csharp
MobilePhone = null  // Empty string converted to null
```

---

## Creating Custom Transformers

### Step 1: Implement IPropertyTransformer

```csharp
using Shared.Transformers;

public class CustomPhoneTransformer : IPropertyTransformer
{
    public object? Transform(object? value, string propertyName)
    {
        if (value is string str && !string.IsNullOrWhiteSpace(str))
        {
            // Remove all non-digit characters
            return new string(str.Where(char.IsDigit).ToArray());
        }
        return value;
    }
}
```

### Step 2: Use in DTO

```csharp
public class CreateUserDto
{
    [Required]
    [Transform(typeof(CustomPhoneTransformer))]
    public string MobilePhone { get; set; } = string.Empty;
}
```

---

## NestJS vs .NET Comparison

| Feature                   | NestJS                 | .NET (This Implementation)  |
| ------------------------- | ---------------------- | --------------------------- |
| **Decorator**             | `@Transform()`         | `[Transform(typeof(...))]`  |
| **Multiple Transforms**   | ✅ Multiple decorators | ✅ Multiple attributes      |
| **Execution Order**       | ✅ Top to bottom       | ✅ Top to bottom            |
| **Before Validation**     | ✅ Yes                 | ✅ Yes                      |
| **Built-in Transformers** | ⚠️ Manual functions    | ✅ CommonTransformers class |
| **Custom Transformers**   | ✅ Function            | ✅ IPropertyTransformer     |

---

## Registration

The `TransformModelBinderProvider` must be registered in `Program.cs`:

```csharp
builder.Services.AddControllers()
    .AddMvcOptions(options =>
    {
        // Register TransformModelBinderProvider
        options.ModelBinderProviders.Insert(0,
            new Shared.ModelBinders.TransformModelBinderProvider());
    });
```

**Services Updated:**

- ✅ `UserAccountService/Program.cs`
- ✅ `Gateway/Program.cs`
- ✅ `CoursesService/Program.cs`

---

## Best Practices

### 1. **Order Matters**

Transformers execute in the order they're declared:

```csharp
// ✅ Good: Trim first, then lowercase
[Transform(typeof(CommonTransformers.TrimTransformer))]
[Transform(typeof(CommonTransformers.ToLowerTransformer))]
public string Email { get; set; }

// ❌ Bad: Lowercase first, then trim (whitespace still there)
[Transform(typeof(CommonTransformers.ToLowerTransformer))]
[Transform(typeof(CommonTransformers.TrimTransformer))]
public string Email { get; set; }
```

### 2. **Transform Before Validation**

Transformations happen before validation, so you can:

- Trim whitespace before length validation
- Convert to lowercase before email validation
- Convert empty strings to null before required validation

### 3. **Use for Data Normalization**

Transformations are perfect for:

- ✅ Normalizing email addresses (trim + lowercase)
- ✅ Cleaning phone numbers
- ✅ Converting string IDs to ObjectId
- ✅ Parsing JSON strings

### 4. **Don't Use for Business Logic**

Transformations should be **pure data transformations**, not business logic:

```csharp
// ✅ Good: Data normalization
[Transform(typeof(CommonTransformers.TrimTransformer))]
public string Name { get; set; }

// ❌ Bad: Business logic (should be in service layer)
[Transform(typeof(CalculateDiscountTransformer))]  // Don't do this!
public decimal Price { get; set; }
```

---

## Common Use Cases

### 1. **Email Normalization**

```csharp
[Required]
[EmailAddress]
[Transform(typeof(CommonTransformers.TrimTransformer))]
[Transform(typeof(CommonTransformers.ToLowerTransformer))]
public string Email { get; set; } = string.Empty;
```

### 2. **String Trimming**

```csharp
[Required]
[StringLength(255)]
[Transform(typeof(CommonTransformers.TrimTransformer))]
public string Name { get; set; } = string.Empty;
```

### 3. **MongoDB ObjectId Conversion**

```csharp
[Required]
[Transform(typeof(CommonTransformers.ToMongoObjectIdTransformer))]
public ObjectId CourseId { get; set; }
```

### 4. **Optional Field Normalization**

```csharp
[StringLength(20)]
[Transform(typeof(CommonTransformers.TrimTransformer))]
[Transform(typeof(CommonTransformers.EmptyToNullTransformer))]
public string? MobilePhone { get; set; }
```

---

## Error Handling

If a transformation fails, the error is not added to `ModelState` by default. The transformation is skipped, and validation proceeds with the original value.

For critical transformations (like ObjectId conversion), you can throw exceptions:

```csharp
public class ToMongoObjectIdTransformer : IPropertyTransformer
{
    public object? Transform(object? value, string propertyName)
    {
        if (!ObjectId.TryParse(value as string, out var objectId))
        {
            throw new ArgumentException($"'{propertyName}' is not a valid MongoDB ObjectId");
        }
        return objectId;
    }
}
```

---

## Testing

### Test Transformation

```csharp
[Fact]
public void Email_ShouldBeTrimmedAndLowercased()
{
    // Arrange
    var dto = new CreateUserDto
    {
        Email = "  John@Example.COM  "
    };

    // Act
    var binder = new TransformModelBinder();
    // ... apply transformations

    // Assert
    Assert.Equal("john@example.com", dto.Email);
}
```

---

## Summary

✅ **Implemented:**

- **Hybrid Approach**: Property setters for simple transformations, model binders for complex ones
- `[Transform]` attribute for complex transformations (ObjectId, JSON parsing)
- Custom model binder for complex transformations
- Common transformers (ToMongoObjectIdTransformer, JsonStringToObjectTransformer, etc.)
- Property setters for simple transformations (trim, lowercase)

✅ **Benefits:**

- **More idiomatic .NET** - Property setters are the standard .NET way
- **Better performance** - No reflection overhead for simple cases
- **Flexibility** - Best tool for each job
- **Clean, normalized data** - Automatic transformation before validation

✅ **Usage:**

**Simple Transformations (Property Setters):**

```csharp
private string _email = string.Empty;
public string Email
{
    get => _email;
    set => _email = value?.Trim().ToLowerInvariant() ?? string.Empty;
}
```

**Complex Transformations (Model Binder):**

```csharp
[Transform(typeof(CommonTransformers.ToMongoObjectIdTransformer))]
public ObjectId CourseId { get; set; }
```

See `TRANSFORM_HYBRID_APPROACH.md` for detailed implementation guide.
