# Project Structure

This document outlines the complete structure of the Todo Microservices project.

## Root Directory

```
DotNetMicroServices/
├── src/                          # Source code for microservices
│   ├── Gateway/                  # API Gateway Service
│   ├── TodoService/             # Todo Microservice
│   └── UserService/              # User Microservice
├── libs/                         # Shared libraries
│   └── Shared/                   # Shared library with common code
├── Dockerfile.gateway            # Dockerfile for Gateway
├── Dockerfile.todoservice        # Dockerfile for TodoService
├── Dockerfile.userservice        # Dockerfile for UserService
├── docker-compose.yml            # Docker Compose configuration
├── TodoMicroservices.slnx        # Solution file
├── README.md                     # Main documentation
└── PROJECT_STRUCTURE.md          # This file
```

## Gateway Service (src/Gateway/)

```
Gateway/
├── Controllers/
│   ├── TodoController.cs        # Routes todo requests to TodoService
│   └── UserController.cs        # Routes user requests to UserService
├── Services/
│   ├── ITodoGatewayService.cs   # Interface for Todo gateway service
│   ├── TodoGatewayService.cs   # HTTP client service for TodoService
│   ├── IUserGatewayService.cs  # Interface for User gateway service
│   └── UserGatewayService.cs   # HTTP client service for UserService
├── Middleware/                  # Custom middleware (empty, ready for use)
├── Filters/                     # Exception filters (empty, ready for use)
├── Program.cs                   # Application entry point
├── appsettings.json            # Configuration
└── Gateway.csproj              # Project file
```

## TodoService (src/TodoService/)

```
TodoService/
├── Controllers/
│   └── TodoController.cs        # Todo CRUD endpoints
├── Services/
│   ├── ITodoService.cs          # Todo service interface
│   └── TodoService.cs           # Todo business logic (in-memory)
├── Models/
│   └── Todo.cs                  # Todo model
├── DTOs/
│   ├── CreateTodoDto.cs         # DTO for creating todos
│   └── UpdateTodoDto.cs         # DTO for updating todos
├── Program.cs                   # Application entry point
├── appsettings.json            # Configuration
└── TodoService.csproj          # Project file
```

## UserService (src/UserService/)

```
UserService/
├── Controllers/
│   └── UserController.cs        # User CRUD endpoints
├── Services/
│   ├── IUserService.cs          # User service interface
│   └── UserService.cs           # User business logic (in-memory)
├── Models/
│   └── User.cs                  # User model
├── DTOs/
│   ├── CreateUserDto.cs         # DTO for creating users
│   └── UpdateUserDto.cs         # DTO for updating users
├── Program.cs                   # Application entry point
├── appsettings.json            # Configuration
└── UserService.csproj          # Project file
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
- **TodoService**: Handles all todo-related operations
- **UserService**: Handles all user-related operations

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
- **TodoService**: 5001
- **UserService**: 5002

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
