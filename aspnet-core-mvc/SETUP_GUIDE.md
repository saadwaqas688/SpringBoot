# ASP.NET Core MVC Setup Guide

This document provides a complete guide to set up the ASP.NET Core MVC version of the Course Management API.

## Project Structure Created

The following structure has been created:

```
aspnet-core-mvc/
├── Models/
│   ├── User.cs
│   ├── Course.cs
│   ├── Discussion.cs
│   ├── Post.cs
│   └── CourseEnrollment.cs
├── DTOs/
│   ├── CreatePostRequest.cs
│   └── UpdatePostRequest.cs
├── Program.cs
├── appsettings.json
└── CourseManagementAPI.csproj
```

## Files Still Needed

You need to create the following files to complete the application:

### DTOs (in DTOs folder)

- `SignupRequest.cs`
- `SigninRequest.cs`
- `AuthResponse.cs`
- `EnrollUsersRequest.cs`
- `EnrolledUserDto.cs`

### Repositories (in Repositories folder)

- `IUserRepository.cs` and `UserRepository.cs`
- `ICourseRepository.cs` and `CourseRepository.cs`
- `IDiscussionRepository.cs` and `DiscussionRepository.cs`
- `IPostRepository.cs` and `PostRepository.cs`
- `ICourseEnrollmentRepository.cs` and `CourseEnrollmentRepository.cs`

### Services (in Services folder)

- `IAuthService.cs` and `AuthService.cs`
- `ICourseService.cs` and `CourseService.cs`
- `IDiscussionService.cs` and `DiscussionService.cs`
- `IPostService.cs` and `PostService.cs`
- `IEnrollmentService.cs` and `EnrollmentService.cs`
- `IJwtService.cs` and `JwtService.cs`

### Controllers (in Controllers folder)

- `AuthController.cs`
- `CourseController.cs`
- `DiscussionController.cs`
- `PostController.cs`
- `EnrollmentController.cs`
- `UserController.cs`
- `HomeController.cs`

### Middleware (in Middleware folder)

- `ErrorHandlingMiddleware.cs`

## Quick Start

1. **Install .NET 8.0 SDK** if not already installed

2. **Restore packages:**

   ```bash
   cd d:\backend\aspnet-core-mvc
   dotnet restore
   ```

3. **Build the project:**

   ```bash
   dotnet build
   ```

4. **Run the application:**
   ```bash
   dotnet run
   ```

## Next Steps

1. Create all the missing files listed above
2. Implement the repository pattern for MongoDB
3. Implement JWT authentication service
4. Implement all business logic services
5. Create all API controllers
6. Test the endpoints

## Key Differences from Spring Boot

1. **Dependency Injection**: Uses built-in DI container (no need for @Autowired)
2. **Configuration**: Uses `appsettings.json` instead of `application.properties`
3. **Validation**: Uses Data Annotations (`[Required]`, `[MaxLength]`, etc.)
4. **MongoDB**: Uses MongoDB.Driver directly instead of Spring Data MongoDB
5. **JWT**: Uses System.IdentityModel.Tokens.Jwt

## MongoDB Collections

The application uses the following collections:

- `users`
- `courses`
- `discussions`
- `posts`
- `courseEnrollments`

## Authentication Flow

1. User signs up/signs in
2. Server generates JWT token
3. Client includes token in `Authorization: Bearer <token>` header
4. Server validates token on each request

## Example Controller

```csharp
[ApiController]
[Route("api/[controller]")]
public class PostController : ControllerBase
{
    private readonly IPostService _postService;
    private readonly IJwtService _jwtService;

    public PostController(IPostService postService, IJwtService jwtService)
    {
        _postService = postService;
        _jwtService = jwtService;
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreatePost([FromBody] CreatePostRequest request)
    {
        var userId = _jwtService.GetUserIdFromToken(Request.Headers["Authorization"]);
        // Implementation
    }
}
```

## Need Help?

Refer to the Spring Boot version for business logic implementation details. The structure and patterns are similar, just using C#/.NET syntax instead of Java/Spring.
