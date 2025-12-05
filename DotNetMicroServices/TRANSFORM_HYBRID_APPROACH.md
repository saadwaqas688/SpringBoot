# Field Transformation: Hybrid Approach Implementation

## Overview

We've implemented a **hybrid approach** that combines the best of both worlds:

- ✅ **Property Setters** for simple transformations (trim, lowercase) - **Most idiomatic .NET**
- ✅ **Model Binders** for complex transformations (ObjectId, JSON parsing) - **Flexible and reusable**

---

## Current Implementation

### Simple Transformations → Property Setters

**Example: Email Normalization**

```csharp
public class CreateUserDto
{
    private string _email = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(255)]
    public string Email
    {
        get => _email;
        set => _email = value?.Trim().ToLowerInvariant() ?? string.Empty;
    }
}
```

**Benefits:**

- ✅ Most idiomatic .NET approach
- ✅ Fast (no reflection overhead)
- ✅ Simple and readable
- ✅ Works automatically with model binding

**Used For:**

- String trimming
- Case conversion (lowercase, uppercase)
- Simple normalization

---

### Complex Transformations → Model Binders

**Example: MongoDB ObjectId Conversion**

```csharp
public class GetCourseDto
{
    [Required]
    [Transform(typeof(CommonTransformers.ToMongoObjectIdTransformer))]
    public ObjectId CourseId { get; set; }
}
```

**Benefits:**

- ✅ Reusable transformers
- ✅ Declarative syntax
- ✅ Handles complex scenarios
- ✅ Easy to test

**Used For:**

- MongoDB ObjectId conversion
- JSON string parsing
- Array transformations
- Complex type conversions

---

## Refactored DTOs

### ✅ UserAccountService

- `CreateUserDto` - Uses property setters for FirstName, LastName, Email
- `SignUpDto` - Uses property setters for Name, Email
- `UpdateUserDto` - Uses property setters for FirstName, LastName, Email (handles null/empty)

### ✅ Gateway

- `CreateUserDto` - Uses property setters for FirstName, LastName, Email
- `SignUpDto` - Uses property setters for Name, Email
- `UpdateUserDto` - Uses property setters for FirstName, LastName, Email (handles null/empty)

---

## When to Use Each Approach

### Use Property Setters When:

| Scenario                           | Example                    |
| ---------------------------------- | -------------------------- |
| Simple string transformations      | Trim, lowercase, uppercase |
| Single transformation per property | `value?.Trim()`            |
| Performance is important           | No reflection overhead     |
| Want idiomatic .NET code           | Standard .NET pattern      |

### Use Model Binders When:

| Scenario                      | Example             |
| ----------------------------- | ------------------- |
| Complex type conversions      | String → ObjectId   |
| Multiple transformations      | Need chaining       |
| Reusable transformation logic | Shared transformers |
| JSON parsing                  | String → Object     |

---

## Code Examples

### Property Setter Pattern

```csharp
// Required string field
private string _email = string.Empty;
public string Email
{
    get => _email;
    set => _email = value?.Trim().ToLowerInvariant() ?? string.Empty;
}

// Optional string field
private string? _firstName;
public string? FirstName
{
    get => _firstName;
    set => _firstName = string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
```

### Model Binder Pattern

```csharp
// Complex transformation
[Transform(typeof(CommonTransformers.ToMongoObjectIdTransformer))]
public ObjectId CourseId { get; set; }

// JSON parsing
[Transform(typeof(CommonTransformers.JsonStringToObjectTransformer))]
public object? Metadata { get; set; }
```

---

## Performance Comparison

| Approach            | Overhead   | Speed      | Use Case                |
| ------------------- | ---------- | ---------- | ----------------------- |
| **Property Setter** | None       | ⚡ Fastest | Simple transformations  |
| **Model Binder**    | Reflection | 🐢 Slower  | Complex transformations |

---

## Migration Summary

### What Changed:

1. ✅ **Removed** `[Transform]` attributes from simple transformations
2. ✅ **Added** property setters with transformation logic
3. ✅ **Kept** model binder infrastructure for complex cases
4. ✅ **Updated** all DTOs to use hybrid approach

### What Stayed:

1. ✅ Model binder infrastructure (for complex transformations)
2. ✅ `TransformModelBinderProvider` registration
3. ✅ Complex transformers (ToMongoObjectIdTransformer, etc.)

---

## Benefits of Hybrid Approach

### ✅ More Idiomatic .NET

- Property setters are the standard .NET way
- Other developers will immediately understand
- No custom infrastructure needed for simple cases

### ✅ Better Performance

- No reflection overhead for simple transformations
- Direct property access is fastest
- Model binders only used when needed

### ✅ Flexibility

- Simple transformations: Property setters
- Complex transformations: Model binders
- Best tool for each job

### ✅ Maintainability

- Less code to maintain
- Clear separation of concerns
- Easy to understand and modify

---

## Best Practices

### 1. Use Property Setters for Simple Cases

```csharp
// ✅ Good: Simple transformation
private string _email = string.Empty;
public string Email
{
    get => _email;
    set => _email = value?.Trim().ToLowerInvariant() ?? string.Empty;
}
```

### 2. Use Model Binders for Complex Cases

```csharp
// ✅ Good: Complex transformation
[Transform(typeof(CommonTransformers.ToMongoObjectIdTransformer))]
public ObjectId CourseId { get; set; }
```

### 3. Handle Null/Empty for Optional Fields

```csharp
// ✅ Good: Optional field with transformation
private string? _firstName;
public string? FirstName
{
    get => _firstName;
    set => _firstName = string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
```

---

## Summary

✅ **Implemented:**

- Property setters for simple transformations (trim, lowercase)
- Model binders for complex transformations (ObjectId, JSON)
- Hybrid approach throughout all DTOs

✅ **Benefits:**

- More idiomatic .NET code
- Better performance for simple cases
- Flexibility for complex cases
- Easier to maintain

✅ **Result:**

- Best of both worlds
- Follows .NET best practices
- Maintains flexibility for future needs
