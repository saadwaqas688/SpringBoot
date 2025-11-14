# Course Management API - ASP.NET Core MVC

This is the ASP.NET Core MVC version of the Course Management API, equivalent to the Spring Boot version.

## Features

- User Authentication (Signup, Signin, Admin Signup)
- Course Management (CRUD operations)
- Discussion Management (CRUD operations)
- Post Management (CRUD operations)
- Enrollment Management
- JWT-based Authentication
- MongoDB Database
- Swagger/OpenAPI Documentation

## Prerequisites

- .NET 10.0 SDK (or .NET 8.0+)
- MongoDB (running on localhost:27017 by default)

## Setup

1. Update `appsettings.json` with your MongoDB connection string
2. Update JWT key in `appsettings.json` (use a secure key in production)
3. Run the application:
   ```bash
   dotnet run
   ```
4. Access Swagger UI at: `https://localhost:5001/swagger`

## Project Structure

```
CourseManagementAPI/
├── Models/          # Entity models
├── DTOs/            # Data Transfer Objects
├── Repositories/    # Data access layer
├── Services/        # Business logic
├── Controllers/     # API endpoints
├── Middleware/      # Custom middleware
└── Program.cs       # Application entry point
```

## API Endpoints

### Authentication

- POST `/api/auth/signup` - User registration
- POST `/api/auth/signin` - User login
- POST `/api/auth/signup-admin` - Admin registration

### Courses

- GET `/api/courses` - Get all courses
- GET `/api/courses/{id}` - Get course by ID
- POST `/api/courses` - Create course
- PUT `/api/courses/{id}` - Update course
- DELETE `/api/courses/{id}` - Delete course

### Discussions

- GET `/api/discussions` - Get all discussions
- GET `/api/discussions/{id}` - Get discussion by ID
- GET `/api/discussions/course/{courseId}` - Get discussions by course
- POST `/api/discussions` - Create discussion
- PUT `/api/discussions/{id}` - Update discussion
- DELETE `/api/discussions/{id}` - Delete discussion

### Posts

- GET `/api/posts/discussion/{discussionId}` - Get posts by discussion
- GET `/api/posts/{id}` - Get post by ID
- POST `/api/posts` - Create post
- PUT `/api/posts/{id}` - Update post
- DELETE `/api/posts/{id}` - Delete post

### Enrollments

- POST `/api/enrollments` - Enroll users to course
- GET `/api/enrollments/course/{courseId}` - Get enrolled users
- GET `/api/enrollments/my-courses` - Get my enrolled courses

## Authentication

All endpoints (except auth) require JWT authentication. Include the token in the Authorization header:

```
Authorization: Bearer <your-token>
```
