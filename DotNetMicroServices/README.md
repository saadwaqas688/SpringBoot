# Todo Microservices - .NET Implementation

A learning project demonstrating a microservices architecture using .NET, similar to the NestJS microservices structure. This project implements a Course Management application with multiple microservices.

## Architecture

This project follows a microservices architecture pattern with:

- **Gateway Service** (Port 5000): API Gateway that routes requests to appropriate microservices
- **UserAccount Service** (Port 5003): Manages user accounts, authentication, and authorization
- **Courses Service** (Port 5004): Manages courses, lessons, quizzes, and progress tracking
- **Shared Library**: Common models, DTOs, utilities, and services shared across microservices

## Project Structure

```
DotNetMicroServices/
├── src/
│   ├── Gateway/              # API Gateway Service
│   │   ├── Controllers/      # API Controllers
│   │   ├── Services/         # Gateway Services (HTTP clients)
│   │   ├── Middleware/       # Custom middleware
│   │   └── Filters/          # Exception filters
│   ├── UserAccountService/  # User Account Microservice
│   │   ├── Controllers/      # User Account API Controllers
│   │   ├── Services/         # User Account Business Logic
│   │   ├── Models/           # User Account Models
│   │   └── DTOs/             # Data Transfer Objects
│   └── CoursesService/       # Courses Microservice
│       ├── Controllers/      # Courses API Controllers
│       ├── Services/         # Courses Business Logic
│       ├── Models/           # Courses Models
│       └── DTOs/             # Data Transfer Objects
├── libs/
│   └── Shared/               # Shared Library
│       ├── Models/           # Shared Models
│       ├── DTOs/             # Shared DTOs
│       ├── Common/           # Common classes (ApiResponse, etc.)
│       ├── Constants/       # Constants
│       ├── Utils/            # Utility functions
│       └── Services/        # Shared Services (HttpClient, RabbitMQ)
├── Dockerfile.gateway        # Gateway Dockerfile
├── Dockerfile.useraccountservice    # UserAccountService Dockerfile
├── Dockerfile.coursesservice.dev   # CoursesService Dockerfile
└── docker-compose.yml        # Docker Compose configuration
```

## Prerequisites

- .NET SDK (10.0 or later)
- Docker Desktop (optional, for containerized deployment)

## Getting Started

### Running Locally

1. **Restore dependencies:**

   ```bash
   dotnet restore
   ```

2. **Build the solution:**

   ```bash
   dotnet build
   ```

3. **Run services individually:**

   Terminal 1 - Gateway:

   ```bash
   cd src/Gateway
   dotnet run
   ```

   Terminal 2 - UserAccountService:

   ```bash
   cd src/UserAccountService
   dotnet run
   ```

   Terminal 3 - CoursesService:

   ```bash
   cd src/CoursesService
   dotnet run
   ```

### Running with Docker Compose

1. **Build and run all services:**

   ```bash
   docker-compose up --build
   ```

2. **Run in detached mode:**

   ```bash
   docker-compose up -d --build
   ```

3. **Stop services:**
   ```bash
   docker-compose down
   ```

## API Endpoints

### Gateway Service (http://localhost:5000)

#### Todo Endpoints

- `GET /api/todo` - Get all todos (optional query: `?userId={guid}`)
- `GET /api/todo/{id}` - Get todo by ID
- `POST /api/todo` - Create a new todo
- `PUT /api/todo/{id}` - Update a todo
- `DELETE /api/todo/{id}` - Delete a todo

#### User Account Endpoints

- `POST /api/auth/signup` - Sign up a new user
- `POST /api/auth/signin` - Sign in a user
- `PUT /api/auth/profile` - Update user profile (requires authentication)
- `GET /api/auth/profile` - Get current user profile (requires authentication)

#### Courses Endpoints

- `GET /api/courses` - Get all courses
- `GET /api/courses/{id}` - Get course by ID
- `POST /api/courses` - Create a new course
- `PUT /api/courses/{id}` - Update a course
- `DELETE /api/courses/{id}` - Delete a course

### Direct Service Access

#### UserAccountService (http://localhost:5003)

- Authentication and user account management endpoints

#### CoursesService (http://localhost:5004)

- Courses, lessons, quizzes, and progress tracking endpoints

## Example API Calls

### Sign Up a User

```bash
curl -X POST http://localhost:5000/api/auth/signup \
  -H "Content-Type: application/json" \
  -d '{
    "email": "john@example.com",
    "password": "SecurePassword123!",
    "firstName": "John",
    "lastName": "Doe"
  }'
```

### Sign In

```bash
curl -X POST http://localhost:5000/api/auth/signin \
  -H "Content-Type: application/json" \
  -d '{
    "email": "john@example.com",
    "password": "SecurePassword123!"
  }'
```

### Create a Course (requires authentication)

```bash
curl -X POST http://localhost:5000/api/courses \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer <token-from-signin>" \
  -d '{
    "title": "Introduction to Microservices",
    "description": "Learn microservices architecture",
    "status": "published"
  }'
```

## Swagger Documentation

When running in Development mode, Swagger UI is available at:

- Gateway: http://localhost:5000/swagger
- UserAccountService: http://localhost:5003/swagger
- CoursesService: http://localhost:5004/swagger

## Key Features

- **Microservices Architecture**: Separate services for different domains
- **API Gateway Pattern**: Single entry point for all client requests
- **Shared Library**: Common code shared across services
- **Docker Support**: Containerized deployment
- **RESTful APIs**: Standard REST endpoints
- **Error Handling**: Consistent error responses
- **CORS Support**: Cross-origin requests enabled
- **Swagger Integration**: API documentation

## Learning Objectives

This project demonstrates:

1. **Microservices Architecture**: How to structure and organize microservices
2. **Service Communication**: HTTP-based inter-service communication
3. **API Gateway Pattern**: Centralized routing and request handling
4. **Shared Libraries**: Code reuse across services
5. **Docker Containerization**: Containerized microservices
6. **.NET Best Practices**: Clean architecture, dependency injection, etc.

## Project Similarities to NestJS Structure

This .NET implementation mirrors the NestJS project structure:

- Similar folder organization (Controllers, Services, Models, DTOs)
- Shared library pattern (libs/shared)
- API Gateway pattern
- Service-to-service communication
- Docker support
- Similar project layout and naming conventions

## Notes

- This is a **learning project** with in-memory data storage (no database)
- Data is not persisted between service restarts
- For production use, add a database (SQL Server, PostgreSQL, MongoDB, etc.)
- Consider adding authentication/authorization
- Add logging and monitoring
- Implement service discovery
- Add message queue for async communication

## License

This is a learning project for educational purposes.
