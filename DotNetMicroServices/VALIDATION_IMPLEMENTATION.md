# Global Validation Implementation - .NET (NestJS-Style)

This document explains the global validation implementation that mimics NestJS's ValidationPipe behavior in the .NET microservices project.

## Overview

We've implemented **automatic request validation** using a global action filter that validates all incoming requests before they reach controller actions. This eliminates the need for manual `ModelState.IsValid` checks in every controller method.

---

## Architecture

### 1. **ValidateModelAttribute Filter**

**Location:** `libs/Shared/Filters/ValidateModelAttribute.cs`

This is a global action filter that:

- Intercepts all incoming HTTP requests
- Validates DTOs using Data Annotations before controller actions execute
- Formats validation errors into user-friendly messages
- Returns consistent `ApiResponse` format for all validation errors

**Key Features:**

- ✅ **Automatic** - No manual checks needed in controllers
- ✅ **Global** - Applied to all endpoints automatically
- ✅ **Consistent** - Same error format everywhere
- ✅ **User-friendly** - Formatted error messages

### 2. **ValidationHelper**

**Location:** `libs/Shared/Filters/ValidationHelper.cs`

Utility class providing helper methods for:

- Converting property names to title case (e.g., "firstName" → "First Name")
- Capitalizing text
- Formatting error messages

---

## Implementation Details

### Step 1: Filter Creation

The `ValidateModelAttribute` class extends `ActionFilterAttribute` and overrides `OnActionExecuting`:

```csharp
public class ValidateModelAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        // Check if ModelState is valid
        if (!context.ModelState.IsValid)
        {
            // Extract and format errors
            var errors = ExtractValidationErrors(context.ModelState);

            // Return BadRequest with ApiResponse format
            context.Result = new BadRequestObjectResult(
                ApiResponse<object>.ErrorResponse(
                    message: "Validation failed",
                    errors: errors
                )
            );
        }
    }
}
```

**How It Works:**

1. ASP.NET Core automatically populates `ModelState` when:

   - Request body is bound to a DTO with `[FromBody]`
   - DTO has Data Annotations attributes (`[Required]`, `[EmailAddress]`, etc.)
   - Controller has `[ApiController]` attribute

2. The filter checks `ModelState.IsValid` before the controller action executes

3. If invalid, it formats errors and returns a `BadRequest` response

4. If valid, the request proceeds normally to the controller action

### Step 2: Global Registration

Applied globally in `Program.cs` for each service:

```csharp
builder.Services.AddControllers(options =>
{
    // Apply global validation filter
    options.Filters.Add<Shared.Filters.ValidateModelAttribute>();
})
.AddJsonOptions(options => { /* ... */ });
```

**Services Updated:**

- ✅ `UserAccountService/Program.cs`
- ✅ `Gateway/Program.cs`
- ✅ `CoursesService/Program.cs`

### Step 3: Error Message Formatting

The filter formats error messages to be user-friendly:

**Before (Technical):**

```
"The Email field is not a valid e-mail address."
```

**After (User-friendly):**

```
"Email: Email is not a valid e-mail address"
```

**Property Name Formatting:**

- `firstName` → `First Name`
- `emailAddress` → `Email Address`
- `dateOfBirth` → `Date Of Birth`
- `User.Email` → `Email (in User)` (nested properties)

---

## How It Works: Request Flow

```
1. HTTP Request arrives at API endpoint
   ↓
2. ASP.NET Core model binding:
   - Deserializes JSON to DTO instance
   - Applies Data Annotations validation
   - Populates ModelState with results
   ↓
3. ValidateModelAttribute filter executes (OnActionExecuting):
   - Checks ModelState.IsValid
   ↓
4a. If INVALID:
   - Extracts all validation errors
   - Formats error messages
   - Returns BadRequest (400) with ApiResponse
   - Request stops here (never reaches controller)
   ↓
4b. If VALID:
   - Request proceeds to controller action
   - Controller receives validated DTO
```

---

## Comparison: Before vs After

### Before (Manual Validation)

```csharp
[HttpPost("users")]
public async Task<ActionResult<ApiResponse<UserInfoDto>>> CreateUser([FromBody] CreateUserDto dto)
{
    // ❌ Manual check required in every method
    if (!ModelState.IsValid)
    {
        var errors = ModelState
            .Where(x => x.Value?.Errors.Count > 0)
            .SelectMany(x => x.Value!.Errors.Select(e =>
                $"{x.Key}: {e.ErrorMessage}"))
            .ToList();

        return BadRequest(ApiResponse<UserInfoDto>.ErrorResponse(
            $"Validation failed: {string.Join(", ", errors)}"));
    }

    // Business logic here...
}
```

**Problems:**

- ❌ Code duplication in every controller method
- ❌ Inconsistent error formatting
- ❌ Easy to forget validation checks
- ❌ More code to maintain

### After (Automatic Validation)

```csharp
[HttpPost("users")]
public async Task<ActionResult<ApiResponse<UserInfoDto>>> CreateUser([FromBody] CreateUserDto dto)
{
    // ✅ Validation already happened automatically!
    // If we reach here, the DTO is guaranteed to be valid

    // Business logic here...
}
```

**Benefits:**

- ✅ No manual checks needed
- ✅ Consistent error format
- ✅ Impossible to forget validation
- ✅ Less code, easier maintenance

---

## DTO Validation Example

DTOs use Data Annotations for validation rules:

```csharp
public class CreateUserDto
{
    [Required]
    [StringLength(255)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(255)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MinLength(6)]
    public string Password { get; set; } = string.Empty;

    [StringLength(20)]
    public string? MobilePhone { get; set; }  // Optional field
}
```

**Validation Rules:**

- `[Required]` - Field must be provided
- `[EmailAddress]` - Must be valid email format
- `[StringLength(255)]` - Maximum length validation
- `[MinLength(6)]` - Minimum length validation
- No `[Required]` = Optional field

---

## Error Response Format

All validation errors return the same format:

```json
{
  "success": false,
  "message": "Validation failed",
  "data": null,
  "errors": [
    "First Name is required",
    "Email: Email is not a valid e-mail address",
    "Password: Password must be at least 6 characters"
  ]
}
```

**Features:**

- Consistent format across all endpoints
- Multiple errors returned in array
- User-friendly error messages
- Field names formatted for readability

---

## NestJS vs .NET Implementation Comparison

| Feature                      | NestJS                              | .NET (This Implementation)                         |
| ---------------------------- | ----------------------------------- | -------------------------------------------------- |
| **Global Validation**        | ✅ `useGlobalPipes(ValidationPipe)` | ✅ `options.Filters.Add<ValidateModelAttribute>()` |
| **Automatic**                | ✅ Yes                              | ✅ Yes                                             |
| **Error Formatting**         | ✅ Custom exception factory         | ✅ Custom error extraction                         |
| **Property Name Formatting** | ✅ Automatic                        | ✅ Automatic (ToTitleCase)                         |
| **Nested Validation**        | ✅ Supported                        | ✅ Supported                                       |
| **Whitelist Unknown Props**  | ✅ `whitelist: true`                | ⚠️ Not implemented (can be added)                  |
| **Type Transformation**      | ✅ `transform: true`                | ⚠️ Manual (ASP.NET handles automatically)          |

---

## Best Practices Followed

### 1. **Separation of Concerns**

- Validation logic in filter (not controllers)
- Error formatting in helper class
- Reusable across all services

### 2. **DRY Principle**

- Validation defined once in DTOs
- Error formatting logic centralized
- No code duplication

### 3. **Consistency**

- Same validation approach everywhere
- Consistent error response format
- Uniform error messages

### 4. **Maintainability**

- Easy to update error formatting
- Single place to modify validation behavior
- Clear, documented code

### 5. **Developer Experience**

- No manual checks needed
- Self-documenting DTOs
- IntelliSense support

---

## Usage Examples

### Example 1: Required Field Missing

**Request:**

```json
{
  "lastName": "Doe",
  "email": "john@example.com"
}
```

**Response (400 Bad Request):**

```json
{
  "success": false,
  "message": "Validation failed",
  "errors": ["First Name is required"]
}
```

### Example 2: Multiple Validation Errors

**Request:**

```json
{
  "firstName": "",
  "email": "invalid-email",
  "password": "123"
}
```

**Response (400 Bad Request):**

```json
{
  "success": false,
  "message": "Validation failed",
  "errors": [
    "First Name is required",
    "Email: Email is not a valid e-mail address",
    "Password: Password must be at least 6 characters"
  ]
}
```

### Example 3: Valid Request

**Request:**

```json
{
  "firstName": "John",
  "lastName": "Doe",
  "email": "john@example.com",
  "password": "secure123"
}
```

**Response:** Proceeds to controller action (validation passed)

---

## Advanced Features

### 1. **Nested Property Validation**

For nested DTOs, errors are formatted with context:

```csharp
public class CreateOrderDto
{
    [Required]
    public UserDto User { get; set; }
}

public class UserDto
{
    [Required]
    public string Email { get; set; }
}
```

**Error Format:**

```
"Email (in User) is required"
```

### 2. **Custom Error Messages**

You can customize error messages in Data Annotations:

```csharp
[Required(ErrorMessage = "First name cannot be empty")]
[StringLength(255, ErrorMessage = "First name cannot exceed 255 characters")]
public string FirstName { get; set; }
```

---

## Testing Validation

### Test Invalid Request

```bash
curl -X POST http://localhost:5000/api/Auth/users \
  -H "Content-Type: application/json" \
  -d '{"email": "invalid"}'
```

**Expected:** 400 Bad Request with validation errors

### Test Valid Request

```bash
curl -X POST http://localhost:5000/api/Auth/users \
  -H "Content-Type: application/json" \
  -d '{
    "firstName": "John",
    "lastName": "Doe",
    "email": "john@example.com",
    "password": "secure123"
  }'
```

**Expected:** 200 OK (or appropriate success response)

---

## Migration Guide

### Removing Manual Validation Checks

**Before:**

```csharp
if (!ModelState.IsValid)
{
    // ... error handling
}
```

**After:**

```csharp
// Remove the check - validation happens automatically!
// Add a comment if needed:
// Note: Validation is handled automatically by ValidateModelAttribute filter
```

### Updating DTOs

Ensure all DTOs have proper Data Annotations:

```csharp
[Required]
[StringLength(255)]
public string Name { get; set; }
```

---

## Troubleshooting

### Issue: Validation not working

**Check:**

1. Filter is registered in `Program.cs`
2. Controller has `[ApiController]` attribute
3. DTO has Data Annotations
4. Request has `Content-Type: application/json` header

### Issue: Errors not formatted correctly

**Check:**

1. `ValidationHelper.ToTitleCase()` method
2. Error extraction logic in `ExtractValidationErrors()`

### Issue: Filter not executing

**Check:**

1. Filter is added to `options.Filters` in `Program.cs`
2. No other filters are short-circuiting the request
3. Controller action is being called (check logs)

---

## Future Enhancements

### Potential Improvements:

1. **Whitelist Unknown Properties**

   - Strip properties not defined in DTO
   - Similar to NestJS `whitelist: true`

2. **Custom Validators**

   - Create custom validation attributes
   - Complex business rule validation

3. **FluentValidation Integration**

   - More powerful validation rules
   - Better error messages
   - Conditional validation

4. **Validation Groups**
   - Different validation rules for different scenarios
   - Create vs Update validation

---

## Summary

✅ **Implemented:**

- Global validation filter (NestJS-style)
- Automatic validation for all endpoints
- User-friendly error messages
- Consistent error response format
- Property name formatting
- Nested property support

✅ **Benefits:**

- No manual validation checks needed
- Consistent error handling
- Better developer experience
- Easier maintenance
- Follows .NET best practices

✅ **Next Steps:**

- Remove any remaining manual `ModelState.IsValid` checks
- Ensure all DTOs have proper Data Annotations
- Test validation with various scenarios


