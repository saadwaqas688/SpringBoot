# Project Structure

This document outlines the complete structure of the Todo Microservices project.

## Root Directory

```
DotNetMicroServices/
├── src/                          # Source code for microservices
│   ├── Gateway/                  # API Gateway Service
│   ├── UserAccountService/       # User Account Microservice
│   └── CoursesService/           # Courses Microservice
├── libs/                         # Shared libraries
│   └── Shared/                   # Shared library with common code
├── Dockerfile.gateway            # Dockerfile for Gateway
├── Dockerfile.useraccountservice # Dockerfile for UserAccountService
├── Dockerfile.coursesservice.dev # Dockerfile for CoursesService
├── docker-compose.yml            # Docker Compose configuration
├── TodoMicroservices.slnx        # Solution file
├── README.md                     # Main documentation
└── PROJECT_STRUCTURE.md          # This file
```

## Gateway Service (src/Gateway/)

```
Gateway/
├── Controllers/
│   ├── AuthController.cs        # Routes authentication requests to UserAccountService
│   ├── CoursesController.cs     # Routes course requests to CoursesService
│   └── AdminController.cs       # Admin endpoints
├── Services/
│   ├── IUserAccountGatewayService.cs   # Interface for UserAccount gateway service
│   ├── UserAccountGatewayService.cs    # RabbitMQ client service for UserAccountService
│   ├── ICoursesGatewayService.cs       # Interface for Courses gateway service
│   └── CoursesGatewayService.cs        # RabbitMQ client service for CoursesService
├── Middleware/                  # Custom middleware (empty, ready for use)
├── Filters/                     # Exception filters (empty, ready for use)
├── Program.cs                   # Application entry point
├── appsettings.json            # Configuration
└── Gateway.csproj              # Project file
```

## UserAccountService (src/UserAccountService/)

```
UserAccountService/
├── Controllers/
│   └── AuthController.cs        # Authentication endpoints
├── Services/
│   ├── IAuthService.cs          # Auth service interface
│   ├── AuthService.cs           # Authentication business logic
│   └── UserAccountMessageHandler.cs  # RabbitMQ message handler
├── Models/
│   └── UserAccount.cs           # User account model
├── DTOs/
│   ├── SignUpDto.cs             # DTO for user signup
│   └── SignInDto.cs             # DTO for user signin
├── Data/
│   └── UserAccountDbContext.cs  # MongoDB context
├── Program.cs                   # Application entry point
├── appsettings.json            # Configuration
└── UserAccountService.csproj    # Project file
```

## CoursesService (src/CoursesService/)

```
CoursesService/
├── Controllers/
│   ├── CoursesController.cs     # Course CRUD endpoints
│   ├── LessonsController.cs     # Lesson endpoints
│   ├── QuizzesController.cs     # Quiz endpoints
│   └── ProgressController.cs   # Progress tracking endpoints
├── Services/
│   ├── ICourseService.cs        # Course service interface
│   ├── CourseService.cs         # Course business logic
│   └── CoursesMessageHandler.cs # RabbitMQ message handler
├── Models/
│   ├── Course.cs               # Course model
│   ├── Lesson.cs               # Lesson model
│   └── Quiz.cs                 # Quiz model
├── Repositories/
│   └── [Various repositories]  # MongoDB repositories
├── Data/
│   └── CoursesDbContext.cs      # MongoDB context
├── Program.cs                   # Application entry point
├── appsettings.json            # Configuration
└── CoursesService.csproj        # Project file
```

## Shared Library (libs/Shared/)

```
Shared/
├── Models/
│   ├── Todo.cs                  # Shared Todo model
│   └── User.cs                   # Shared User model
├── DTOs/
│   ├── CreateTodoDto.cs         # Shared DTOs
│   ├── UpdateTodoDto.cs
│   ├── CreateUserDto.cs
│   └── UpdateUserDto.cs
├── Common/
│   ├── ApiResponse.cs            # Standard API response wrapper
│   └── PagedResponse.cs         # Pagination response wrapper
├── Constants/
│   └── ServiceConstants.cs      # Service URLs and constants
├── Utils/
│   └── DateTimeHelper.cs         # DateTime utility functions
├── Services/
│   ├── IHttpClientService.cs    # HTTP client interface
│   └── HttpClientService.cs     # HTTP client implementation
└── Shared.csproj                # Project file
```

## Key Features

### 1. Microservices Architecture

- **Gateway**: Single entry point for all client requests
- **UserAccountService**: Handles user authentication, authorization, and account management
- **CoursesService**: Handles courses, lessons, quizzes, and progress tracking

### 2. Service Communication

- HTTP-based communication between Gateway and microservices
- Uses HttpClient for service-to-service calls
- Standard REST API endpoints

### 3. Shared Library Pattern

- Common models, DTOs, and utilities in Shared library
- Reduces code duplication
- Ensures consistency across services

### 4. API Response Standardization

- `ApiResponse<T>` wrapper for consistent responses
- Success/Error handling
- Standardized error messages

### 5. Docker Support

- Individual Dockerfiles for each service
- Docker Compose for orchestration
- Easy deployment and scaling

## Port Configuration

- **Gateway**: 5000
- **UserAccountService**: 5003
- **CoursesService**: 5004

## Data Storage

Currently using in-memory storage (List<T>) for simplicity. In production:

- Add database (SQL Server, PostgreSQL, MongoDB)
- Implement repository pattern
- Add Entity Framework or similar ORM

## Future Enhancements

1. **Database Integration**

   - Add Entity Framework Core
   - Implement repository pattern
   - Add database migrations

2. **Authentication & Authorization**

   - JWT tokens
   - Role-based access control
   - User authentication

3. **Message Queue**

   - RabbitMQ or Azure Service Bus
   - Async communication
   - Event-driven architecture

4. **Service Discovery**

   - Consul or Eureka
   - Dynamic service registration
   - Load balancing

5. **Logging & Monitoring**

   - Serilog or NLog
   - Application Insights
   - Health checks

6. **API Gateway Features**
   - Rate limiting
   - Request/Response transformation
   - Circuit breaker pattern

## Similarities to NestJS Structure

This .NET implementation mirrors the NestJS project structure:

- ✅ Similar folder organization (Controllers, Services, Models, DTOs)
- ✅ Shared library pattern (libs/shared)
- ✅ API Gateway pattern
- ✅ Service-to-service communication
- ✅ Docker support
- ✅ Similar project layout and naming conventions
- ✅ Dependency injection
- ✅ Middleware and filters support
