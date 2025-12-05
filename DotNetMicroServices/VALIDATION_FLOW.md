# Request Payload Validation Flow

This document explains how request payload validation works in this microservices application, from frontend to backend.

## Current Validation Flow

### 1. **Frontend Validation (React/TypeScript)**

**Location:** `frontend/src/components/users/AddUserModal.tsx`

**Current Implementation:**

- **Client-side validation** using custom JavaScript validation functions
- Validation happens **before** the API call is made
- Errors are displayed inline in the form

**Example:**

```typescript
const validate = (): boolean => {
  const newErrors: Partial<Record<keyof CreateUserRequest, string>> = {};

  if (!formData.firstName.trim()) {
    newErrors.firstName = "First name is required";
  }
  if (!formData.lastName.trim()) {
    newErrors.lastName = "Last name is required";
  }
  if (!formData.email.trim()) {
    newErrors.email = "Email is required";
  } else if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(formData.email)) {
    newErrors.email = "Invalid email format";
  }
  if (!formData.password) {
    newErrors.password = "Password is required";
  } else if (formData.password.length < 6) {
    newErrors.password = "Password must be at least 6 characters";
  }

  setErrors(newErrors);
  return Object.keys(newErrors).length === 0;
};
```

**Pros:**

- Immediate feedback to users
- Reduces unnecessary API calls
- Better user experience

**Cons:**

- Can be bypassed (users can disable JavaScript)
- Duplicated validation logic (frontend + backend)
- Not a security measure (always validate on backend)

---

### 2. **Gateway Layer (ASP.NET Core)**

**Location:** `src/Gateway/Controllers/AuthController.cs`

**Current Implementation:**

- **No automatic validation** - Gateway DTOs don't have Data Annotations
- Gateway acts as a pass-through to microservices via RabbitMQ
- No ModelState validation checks

**Example:**

```csharp
[HttpPost("users")]
public async Task<ActionResult<ApiResponse<UserInfoDto>>> CreateUser([FromBody] CreateUserDto dto)
{
    // No validation check here!
    var response = await _userAccountGatewayService.CreateUserAsync(dto);
    return StatusCode(response.Success ? 200 : 400, response);
}
```

**Issue:** Gateway DTOs (`Gateway.DTOs.CreateUserDto`) don't have validation attributes.

---

### 3. **Service Layer (UserAccountService)**

**Location:** `src/UserAccountService/Controllers/AuthController.cs`

**Current Implementation:**

- DTOs have **Data Annotations** for validation
- **BUT** controllers don't check `ModelState.IsValid`
- Validation attributes are present but not enforced

**Example DTO:**

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
}
```

**Example Controller (Missing Validation Check):**

```csharp
[HttpPost("users")]
public async Task<ActionResult<ApiResponse<UserInfoDto>>> CreateUser([FromBody] CreateUserDto dto)
{
    try
    {
        // ❌ Missing: if (!ModelState.IsValid) return BadRequest(...)

        // Check if user already exists
        var existingUser = await _userAccountService.GetUserByEmailAsync(dto.Email);
        // ... rest of the code
    }
}
```

---

## How ASP.NET Core Validation Works

ASP.NET Core has **automatic model validation** that runs when:

1. A request comes in with `[FromBody]`, `[FromForm]`, `[FromQuery]`, etc.
2. The model has Data Annotations attributes
3. The controller has `[ApiController]` attribute (which you have)

**However**, you need to **explicitly check** `ModelState.IsValid` in your action methods, OR use a **global filter** to handle it automatically.

---

## Recommended Improvements

### Option 1: Add ModelState Validation in Controllers (Recommended)

**Update controllers to check ModelState:**

```csharp
[HttpPost("users")]
public async Task<ActionResult<ApiResponse<UserInfoDto>>> CreateUser([FromBody] CreateUserDto dto)
{
    // ✅ Add this check
    if (!ModelState.IsValid)
    {
        var errors = ModelState
            .Where(x => x.Value?.Errors.Count > 0)
            .Select(x => new {
                Field = x.Key,
                Errors = x.Value?.Errors.Select(e => e.ErrorMessage)
            });

        return BadRequest(ApiResponse<UserInfoDto>.ErrorResponse(
            $"Validation failed: {string.Join(", ", errors.SelectMany(e => e.Errors))}"));
    }

    try
    {
        // ... rest of the code
    }
}
```

### Option 2: Use Global Action Filter (Better - DRY)

**Create a validation filter:**

```csharp
// Create: src/UserAccountService/Filters/ValidateModelAttribute.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Shared.Common;

namespace UserAccountService.Filters;

public class ValidateModelAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.ModelState.IsValid)
        {
            var errors = context.ModelState
                .Where(x => x.Value?.Errors.Count > 0)
                .SelectMany(x => x.Value!.Errors.Select(e =>
                    $"{x.Key}: {e.ErrorMessage}"))
                .ToList();

            context.Result = new BadRequestObjectResult(
                ApiResponse<object>.ErrorResponse($"Validation failed: {string.Join(", ", errors)}"));
        }
    }
}
```

**Apply globally in Program.cs:**

```csharp
builder.Services.AddControllers(options =>
{
    options.Filters.Add<ValidateModelAttribute>();
})
.AddJsonOptions(options => { /* ... */ });
```

### Option 3: Use FluentValidation (Most Robust)

**Install FluentValidation:**

```bash
dotnet add package FluentValidation.AspNetCore
```

**Create validators:**

```csharp
public class CreateUserDtoValidator : AbstractValidator<CreateUserDto>
{
    public CreateUserDtoValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required")
            .MaximumLength(255).WithMessage("First name cannot exceed 255 characters");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format")
            .MaximumLength(255).WithMessage("Email cannot exceed 255 characters");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters");
    }
}
```

**Register in Program.cs:**

```csharp
builder.Services.AddFluentValidation(fv =>
    fv.RegisterValidatorsFromAssemblyContaining<CreateUserDtoValidator>());
```

---

## Current Validation Summary

| Layer                     | Validation Status | Notes                                |
| ------------------------- | ----------------- | ------------------------------------ |
| **Frontend**              | ✅ **Active**     | Custom JavaScript validation         |
| **Gateway**               | ❌ **Not Active** | No Data Annotations, no checks       |
| **Service (DTOs)**        | ⚠️ **Partial**    | Has Data Annotations but not checked |
| **Service (Controllers)** | ❌ **Not Active** | Missing ModelState.IsValid checks    |

---

## Security Note

⚠️ **Important:** Frontend validation is for **user experience only**, not security. Always validate on the backend because:

- Users can disable JavaScript
- Users can send direct API requests
- Malicious users can bypass frontend validation

---

## Next Steps

1. **Immediate:** Add `ModelState.IsValid` checks in all controllers
2. **Short-term:** Add Data Annotations to Gateway DTOs
3. **Long-term:** Consider FluentValidation for complex validation rules


