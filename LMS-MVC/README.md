# Learning Management System (LMS)

A full-stack Learning Management System built with ASP.NET Core Web API (MongoDB) and React.

## Features

- **Authentication & Authorization**: User registration, login, and role-based access control (USER, ADMIN)
- **Course Management**: Full CRUD operations for courses
- **Lesson Management**: Create and manage lessons within courses
- **User Enrollment**: Users can enroll in courses
- **Progress Tracking**: Track lesson completion progress
- **Discussion Forums**: Discussion posts for lessons
- **RESTful API**: Clean REST API architecture
- **Modern Frontend**: React with TypeScript

## Project Structure

```
LMS-API/          # ASP.NET Core Web API with MongoDB
LMS-Client/       # React TypeScript Frontend
```

## Prerequisites

- .NET 10.0 SDK
- MongoDB (Local or MongoDB Atlas)
- Node.js 16+ and npm
- Visual Studio 2022 or VS Code

## Backend Setup (LMS-API)

1. Navigate to the API directory:
   ```bash
   cd LMS-API/LMS.API
   ```

2. Update `appsettings.json` with your MongoDB connection string:
   ```json
   {
     "ConnectionStrings": {
       "MongoDB": "mongodb://localhost:27017"
     },
     "MongoDB": {
       "DatabaseName": "LMS"
     }
   }
   ```

3. Restore packages:
   ```bash
   dotnet restore
   ```

4. Run the API:
   ```bash
   dotnet run
   ```

The API will be available at `https://localhost:7000` or `http://localhost:5000`

## Frontend Setup (LMS-Client)

1. Navigate to the client directory:
   ```bash
   cd LMS-Client
   ```

2. Install dependencies:
   ```bash
   npm install
   ```

3. Update the API base URL in `src/services/api.ts` if needed:
   ```typescript
   const API_BASE_URL = 'https://localhost:7000/api';
   ```

4. Start the development server:
   ```bash
   npm start
   ```

The React app will be available at `http://localhost:3000`

## API Endpoints

### Authentication
- `POST /api/auth/register` - Register a new user
- `POST /api/auth/login` - Login user

### Courses
- `GET /api/course` - Get all courses
- `GET /api/course/{id}` - Get course by ID
- `POST /api/course` - Create course (ADMIN only)
- `PUT /api/course/{id}` - Update course (ADMIN only)
- `DELETE /api/course/{id}` - Delete course (ADMIN only)
- `GET /api/course/my-courses` - Get user's enrolled courses

### Lessons
- `GET /api/lesson` - Get all lessons
- `GET /api/lesson/{id}` - Get lesson by ID
- `GET /api/lesson/course/{courseId}` - Get lessons by course
- `POST /api/lesson` - Create lesson (ADMIN only)
- `PUT /api/lesson/{id}` - Update lesson (ADMIN only)
- `DELETE /api/lesson/{id}` - Delete lesson (ADMIN only)

### Enrollments
- `POST /api/enrollment` - Enroll in a course
- `GET /api/enrollment/my-enrollments` - Get user's enrollments

### Discussion
- `GET /api/discussion/content/{contentId}` - Get posts by content
- `POST /api/discussion` - Create post
- `PUT /api/discussion/{id}` - Update post
- `DELETE /api/discussion/{id}` - Delete post

### Progress
- `PUT /api/lessonprogress/lesson/{lessonId}/course/{courseId}` - Update progress
- `GET /api/lessonprogress/lesson/{lessonId}` - Get progress
- `GET /api/lessonprogress/my-progress` - Get user's progress

## Default Roles

- **USER**: Can enroll in courses, view lessons, participate in discussions
- **ADMIN**: Full access including creating/editing/deleting courses, lessons, and content

## Notes

- The API uses JWT authentication
- MongoDB is used for data persistence
- AutoMapper is used for object-to-object mapping
- Repository pattern for data access abstraction
- Service layer for business logic

## License

This project is provided as-is for educational purposes.
