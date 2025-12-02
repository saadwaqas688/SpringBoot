# API Endpoints Summary

## Services and Ports

### Gateway Service (Port 5000)

- **Swagger URL**: http://localhost:5000/swagger
- **Endpoints**: Auth endpoints, Courses health check (via RabbitMQ)

### CoursesService (Port 5004)

- **Swagger URL**: http://localhost:5004/swagger
- **Endpoints**: ALL course-related endpoints (65+ endpoints)

### UserAccountService (Port 5003)

- **Swagger URL**: http://localhost:5003/swagger
- **Endpoints**: Auth endpoints (direct access)

## Important Note

**All the new endpoints (Lessons, Slides, Discussions, Quizzes, Progress, etc.) are in CoursesService on port 5004, NOT in the Gateway!**

To see all endpoints:

1. Open http://localhost:5004/swagger (CoursesService)
2. Or access Gateway at http://localhost:5000/swagger (limited endpoints)

## Endpoint Locations

### CoursesService (Port 5004) - All these endpoints:

- ✅ Courses CRUD + Assignment endpoints
- ✅ Lessons CRUD
- ✅ Slides CRUD
- ✅ Discussion Posts CRUD
- ✅ Quizzes CRUD
- ✅ Quiz Questions CRUD
- ✅ Quiz Attempts
- ✅ Progress Tracking
- ✅ Activity Logs
- ✅ User Courses & Progress
- ✅ Admin Analytics

### Gateway (Port 5000) - Currently has:

- ✅ Auth endpoints (signup, signin, refresh-token, forgot-password, reset-password)
- ✅ Courses health check

### UserAccountService (Port 5003) - Has:

- ✅ Auth endpoints (direct HTTP access)




