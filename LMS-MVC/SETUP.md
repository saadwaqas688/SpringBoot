# LMS Setup Guide

## Quick Start

1. **Navigate to the project directory:**
   ```bash
   cd LMS-MVC
   ```

2. **Update the connection string in `appsettings.json`:**
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=LMS_DB;Trusted_Connection=True;MultipleActiveResultSets=true"
   }
   ```
   Adjust the connection string to match your SQL Server instance.

3. **Restore packages:**
   ```bash
   dotnet restore
   ```

4. **Run the application:**
   ```bash
   dotnet run
   ```

5. **Access the application:**
   - Open your browser and navigate to `https://localhost:5001` or `http://localhost:5000`
   - Register a new user account (choose ADMIN or USER role)
   - Login and start using the LMS

## Database

The database will be automatically created on first run using `EnsureCreated()`. For production environments, it's recommended to use Entity Framework migrations:

```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

## Features Implemented

✅ **Authentication & Authorization**
- User registration with role selection (USER, ADMIN)
- Login/Logout functionality
- Role-based access control

✅ **Course Management**
- View all courses
- Create, edit, delete courses (ADMIN only)
- Course details with enrollment

✅ **Lesson Management**
- Create, edit, delete lessons within courses (ADMIN only)
- View lessons by course

✅ **Lesson Content**
- Support for multiple content types: slide, video, quiz, discussion
- Create, edit, delete content (ADMIN only)

✅ **Enrollment**
- Users can enroll in courses
- View enrolled courses
- Unenroll from courses

✅ **Discussions**
- Discussion posts for lessons and content
- Reply to posts
- Edit/delete own posts

✅ **Progress Tracking**
- Track lesson completion
- View progress by course

## Architecture

- **Repository Pattern**: Data access abstraction
- **Service Layer**: Business logic separation
- **DTOs**: Clean data transfer objects
- **AutoMapper**: Object-to-object mapping
- **MVC Pattern**: Controllers, Views, Models

## Default Roles

- **USER**: Can enroll in courses, view lessons, participate in discussions
- **ADMIN**: Full access including creating/editing/deleting courses, lessons, and content

## Notes

- The application uses ASP.NET Core Identity for authentication
- Entity Framework Core with SQL Server for data persistence
- Bootstrap 5.3 for UI styling
- jQuery for client-side functionality

