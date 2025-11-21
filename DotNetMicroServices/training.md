# Complete Training Guide: .NET Microservices Project

## Table of Contents

1. [Introduction to .NET](#1-introduction-to-net)
2. [What are Microservices?](#2-what-are-microservices)
3. [How Microservices Work](#3-how-microservices-work)
4. [Microservices in This Project](#4-microservices-in-this-project)
5. [What is Docker?](#5-what-is-docker)
6. [Docker in This Project](#6-docker-in-this-project)
7. [What is an API Gateway?](#7-what-is-an-api-gateway)
8. [Project Entry Points](#8-project-entry-points)
9. [Project Architecture Deep Dive](#9-project-architecture-deep-dive)
10. [Service Communication Flow](#10-service-communication-flow)
11. [Running the Project](#11-running-the-project)
12. [Key Concepts Explained](#12-key-concepts-explained)

---

## 1. Introduction to .NET

### What is .NET?

.NET is a free, open-source, cross-platform framework developed by Microsoft for building various types of applications. It provides a unified platform for developing:

- **Web Applications** (ASP.NET Core)
- **Desktop Applications** (Windows Forms, WPF)
- **Mobile Applications** (Xamarin)
- **Cloud Services** (Azure)
- **Microservices** (ASP.NET Core Web APIs)

### Key Features of .NET

1. **Cross-Platform**: Runs on Windows, Linux, and macOS
2. **High Performance**: Optimized for speed and efficiency
3. **Modern Language Support**: C#, F#, VB.NET
4. **Rich Ecosystem**: Extensive libraries and frameworks
5. **Dependency Injection**: Built-in IoC container
6. **Middleware Pipeline**: Request/response processing
7. **Strong Typing**: Type-safe programming

### .NET in This Project

This project uses **.NET 10.0** with **ASP.NET Core**, which is specifically designed for building web APIs and microservices. Each service in this project is an ASP.NET Core Web API application.

### .NET Project Structure

A typical .NET project contains:

```
ProjectName/
├── Program.cs          # Application entry point
├── Controllers/         # API endpoints
├── Services/            # Business logic
├── Models/             # Data models
├── DTOs/               # Data Transfer Objects
├── appsettings.json    # Configuration
└── ProjectName.csproj  # Project file (dependencies)
```

### Program.cs - The Entry Point

In modern .NET (6.0+), `Program.cs` is the entry point. It uses a minimal hosting model:

```csharp
var builder = WebApplication.CreateBuilder(args);

// Configure services
builder.Services.AddControllers();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure middleware pipeline
app.UseSwagger();
app.MapControllers();

app.Run();
```

**Key Components:**

- `WebApplication.CreateBuilder()`: Creates the application builder
- `builder.Services`: Dependency injection container
- `app.Build()`: Builds the application
- `app.Run()`: Starts the web server

---

## 2. What are Microservices?

### Definition

**Microservices** is an architectural approach where a large application is built as a suite of small, independent services. Each service:

- Runs in its own process
- Communicates via well-defined APIs (usually HTTP/REST)
- Can be developed, deployed, and scaled independently
- Owns its own data
- Implements a specific business capability

### Monolithic vs Microservices

#### Monolithic Architecture

```
┌─────────────────────────────────┐
│     Single Application          │
│  ┌──────────┬──────────┐       │
│  │  Users   │  Todos   │       │
│  │  Module  │  Module  │       │
│  └──────────┴──────────┘       │
│         Shared Database         │
└─────────────────────────────────┘
```

**Characteristics:**

- Single codebase
- Single deployment unit
- All features in one application
- Shared database

**Problems:**

- Hard to scale individual features
- Technology lock-in
- Deployment affects entire app
- Difficult to maintain as it grows

#### Microservices Architecture

```
┌─────────────┐    ┌─────────────┐    ┌─────────────┐
│   Gateway   │    │  UserService│    │ TodoService│
│   (Port     │    │   (Port     │    │   (Port    │
│    5000)    │    │    5002)    │    │    5001)   │
└──────┬──────┘    └──────┬──────┘    └──────┬──────┘
       │                  │                   │
       └──────────────────┴───────────────────┘
                    HTTP/REST
```

**Characteristics:**

- Multiple independent services
- Each service has its own database (or data store)
- Services communicate via APIs
- Independent deployment and scaling

**Benefits:**

- ✅ Independent scaling
- ✅ Technology diversity
- ✅ Fault isolation
- ✅ Team autonomy
- ✅ Easier maintenance

### When to Use Microservices?

**Use Microservices when:**

- Application is large and complex
- Different parts have different scaling needs
- Teams can work independently
- Need technology flexibility

**Avoid Microservices when:**

- Application is small
- Team is small
- Simple requirements
- Performance is critical (network overhead)

---

## 3. How Microservices Work

### Core Principles

1. **Service Independence**

   - Each service is self-contained
   - Can be developed by different teams
   - Can use different technologies
   - Can be deployed independently

2. **Decentralized Data Management**

   - Each service owns its data
   - No shared database
   - Data consistency through APIs

3. **Communication via APIs**

   - REST APIs (HTTP/JSON)
   - Message queues (async)
   - gRPC (high performance)
   - GraphQL (flexible queries)

4. **Fault Tolerance**
   - Services can fail independently
   - Circuit breakers prevent cascading failures
   - Retry mechanisms
   - Graceful degradation

### Communication Patterns

#### 1. Synchronous Communication (Request-Response)

```
Client → Gateway → UserService
         ↓
      Response
```

- **HTTP/REST**: Most common
- **gRPC**: High performance
- **Pros**: Simple, immediate response
- **Cons**: Tight coupling, blocking

#### 2. Asynchronous Communication (Event-Driven)

```
Service A → Message Queue → Service B
```

- **Message Queues**: RabbitMQ, Kafka, Azure Service Bus
- **Pros**: Loose coupling, scalable
- **Cons**: Eventual consistency, complexity

### Service Discovery

Services need to find each other:

- **Client-Side Discovery**: Client knows service locations
- **Server-Side Discovery**: Load balancer routes requests
- **Service Registry**: Central registry (Consul, Eureka)

### API Gateway Pattern

A single entry point that:

- Routes requests to appropriate services
- Handles cross-cutting concerns (auth, logging)
- Aggregates responses
- Provides unified API

---

## 4. Microservices in This Project

### Project Overview

This project implements a **Todo List Application** using microservices architecture with three services:

1. **Gateway Service** (Port 5000)
2. **TodoService** (Port 5001)
3. **UserService** (Port 5002)

### Service Responsibilities

#### Gateway Service (`src/Gateway/`)

**Purpose**: Single entry point for all client requests

**Responsibilities:**

- Receives all client requests
- Routes requests to appropriate microservices
- Aggregates responses
- Handles CORS
- Provides Swagger documentation

**Key Files:**

- `Program.cs`: Entry point and configuration
- `Controllers/TodoController.cs`: Routes todo requests
- `Controllers/UserController.cs`: Routes user requests
- `Services/TodoGatewayService.cs`: Communicates with TodoService
- `Services/UserGatewayService.cs`: Communicates with UserService

**Example Flow:**

```
Client → GET /api/todo → Gateway → TodoService → Response → Gateway → Client
```

#### TodoService (`src/TodoService/`)

**Purpose**: Manages all todo-related operations

**Responsibilities:**

- Create, Read, Update, Delete todos
- Business logic for todos
- Data storage (currently in-memory)

**Key Files:**

- `Program.cs`: Service entry point
- `Controllers/TodoController.cs`: REST API endpoints
- `Services/TodoService.cs`: Business logic
- `Models/Todo.cs`: Todo data model

**Endpoints:**

- `GET /api/todo` - Get all todos
- `GET /api/todo/{id}` - Get todo by ID
- `POST /api/todo` - Create todo
- `PUT /api/todo/{id}` - Update todo
- `DELETE /api/todo/{id}` - Delete todo

#### UserService (`src/UserService/`)

**Purpose**: Manages all user-related operations

**Responsibilities:**

- Create, Read, Update, Delete users
- Business logic for users
- Data storage (currently in-memory)

**Key Files:**

- `Program.cs`: Service entry point
- `Controllers/UserController.cs`: REST API endpoints
- `Services/UserService.cs`: Business logic
- `Models/User.cs`: User data model

**Endpoints:**

- `GET /api/user` - Get all users
- `GET /api/user/{id}` - Get user by ID
- `GET /api/user/email/{email}` - Get user by email
- `POST /api/user` - Create user
- `PUT /api/user/{id}` - Update user
- `DELETE /api/user/{id}` - Delete user

### Shared Library (`libs/Shared/`)

**Purpose**: Common code shared across all services

**Contents:**

- **Models**: `Todo.cs`, `User.cs` - Shared data models
- **DTOs**: Data Transfer Objects for API requests/responses
- **Common**: `ApiResponse<T>` - Standardized API responses
- **Services**: `HttpClientService` - HTTP client wrapper
- **Constants**: Service URLs and configuration
- **Utils**: Helper functions (DateTimeHelper)

**Why Shared Library?**

- Reduces code duplication
- Ensures consistency
- Single source of truth
- Easier maintenance

### Communication Flow in This Project

```
┌─────────┐
│ Client  │
└────┬────┘
     │ HTTP Request
     ▼
┌─────────────────┐
│  Gateway        │  Port 5000
│  (Entry Point)  │
└────┬───────┬────┘
     │       │
     │       │ HTTP/REST
     │       │
     ▼       ▼
┌─────────┐ ┌──────────┐
│TodoService│ │UserService│
│ Port 5001│ │ Port 5002│
└─────────┘ └──────────┘
```

**Example: Creating a Todo**

1. Client sends `POST /api/todo` to Gateway (port 5000)
2. Gateway's `TodoController` receives request
3. `TodoGatewayService` makes HTTP call to `http://todoservice:5001/api/todo`
4. TodoService processes request and returns response
5. Gateway returns response to client

### Data Storage

**Current Implementation**: In-memory storage using `List<T>`

```csharp
// In TodoService/Services/TodoService.cs
private static readonly List<Todo> _todos = new();
```

**Why In-Memory?**

- Simple for learning
- No database setup required
- Fast for development

**Production Considerations:**

- Data is lost on restart
- Not suitable for production
- Should use database (SQL Server, PostgreSQL, MongoDB)

---

## 5. What is Docker?

### Definition

**Docker** is a platform for developing, shipping, and running applications using containerization. It packages applications and their dependencies into containers that can run consistently across different environments.

### Key Concepts

#### Container vs Virtual Machine

**Virtual Machine:**

```
┌─────────────────────────────────┐
│      Application                 │
├─────────────────────────────────┤
│      Guest OS                   │
├─────────────────────────────────┤
│      Hypervisor                 │
├─────────────────────────────────┤
│      Host OS                    │
└─────────────────────────────────┘
```

- Heavy (includes full OS)
- Slow startup
- High resource usage

**Container:**

```
┌─────────────────────────────────┐
│      Application                 │
├─────────────────────────────────┤
│      Docker Engine              │
├─────────────────────────────────┤
│      Host OS                    │
└─────────────────────────────────┘
```

- Lightweight (shares host OS)
- Fast startup
- Low resource usage

### Docker Components

1. **Docker Image**: Read-only template for creating containers
2. **Docker Container**: Running instance of an image
3. **Dockerfile**: Instructions for building an image
4. **Docker Compose**: Tool for defining and running multi-container applications

### Benefits of Docker

- ✅ **Consistency**: Works the same everywhere
- ✅ **Isolation**: Each container is independent
- ✅ **Portability**: Run on any Docker host
- ✅ **Scalability**: Easy to scale up/down
- ✅ **Resource Efficiency**: Lightweight compared to VMs

---

## 6. Docker in This Project

### Dockerfiles

Each service has its own Dockerfile that defines how to build the container image.

#### Dockerfile.gateway

```dockerfile
# Base image for running .NET apps
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 5000
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
# Copy project files
COPY ["src/Gateway/Gateway.csproj", "src/Gateway/"]
COPY ["libs/Shared/Shared.csproj", "libs/Shared/"]
# Restore dependencies
RUN dotnet restore "src/Gateway/Gateway.csproj"
# Copy all files
COPY . .
WORKDIR "/src/src/Gateway"
# Build the project
RUN dotnet build "Gateway.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "Gateway.csproj" -c Release -o /app/publish

# Final stage
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Gateway.dll"]
```

**Explanation:**

1. **Multi-stage build**: Reduces final image size
2. **Base image**: Runtime image (smaller, no SDK)
3. **Build image**: Contains SDK for compilation
4. **Publish**: Creates optimized release build
5. **Final**: Copies published files to runtime image

### Docker Compose

`docker-compose.yml` defines all services and their configuration:

```yaml
services:
  todoservice:
    build:
      context: .
      dockerfile: Dockerfile.todoservice
    container_name: todoservice
    ports:
      - "5001:5001" # Host:Container port mapping
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
      - ASPNETCORE_URLS=http://+:5001 # Listen on all interfaces
    networks:
      - microservices-network
    healthcheck:
      test:
        ["CMD", "curl", "-f", "http://localhost:5001/swagger/v1/swagger.json"]
      interval: 10s
      timeout: 5s
      retries: 5
      start_period: 30s

  userservice:
    # Similar configuration...

  gateway:
    build:
      context: .
      dockerfile: Dockerfile.gateway
    container_name: gateway
    ports:
      - "5000:5000"
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
      - ASPNETCORE_URLS=http://+:5000
      - ServiceUrls__TodoService=http://todoservice:5001
      - ServiceUrls__UserService=http://userservice:5002
    depends_on:
      todoservice:
        condition: service_healthy
      userservice:
        condition: service_healthy
    networks:
      - microservices-network

networks:
  microservices-network:
    driver: bridge
```

### Key Docker Concepts in This Project

#### 1. Port Mapping

```yaml
ports:
  - "5000:5000" # Host:Container
```

- **Left side (5000)**: Port on your host machine
- **Right side (5000)**: Port inside the container
- Access via `http://localhost:5000` from host

#### 2. Environment Variables

```yaml
environment:
  - ASPNETCORE_URLS=http://+:5000
  - ServiceUrls__TodoService=http://todoservice:5001
```

- `ASPNETCORE_URLS`: Tells .NET to listen on all interfaces (`+`)
- `ServiceUrls__TodoService`: Configuration for service URLs (double underscore `__` = nested config)

#### 3. Networks

```yaml
networks:
  - microservices-network
```

- All services on same network can communicate
- Use service names as hostnames (`todoservice`, `userservice`)
- Gateway uses `http://todoservice:5001` not `http://localhost:5001`

#### 4. Health Checks

```yaml
healthcheck:
  test: ["CMD", "curl", "-f", "http://localhost:5001/swagger/v1/swagger.json"]
  interval: 10s
  timeout: 5s
  retries: 5
  start_period: 30s
```

- Checks if service is healthy
- Gateway waits for dependencies to be healthy before starting
- Prevents connection errors

#### 5. Depends On

```yaml
depends_on:
  todoservice:
    condition: service_healthy
  userservice:
    condition: service_healthy
```

- Gateway waits for TodoService and UserService to be healthy
- Ensures proper startup order

### Docker Networking

**Bridge Network**: Default network type

- Containers can communicate using service names
- Isolated from host network
- Gateway can reach `todoservice:5001` and `userservice:5002`

**Important**: In Docker, use service names, not `localhost`!

- ❌ `http://localhost:5001` (won't work between containers)
- ✅ `http://todoservice:5001` (works between containers)

### Running with Docker

```bash
# Build and start all services
docker-compose up --build

# Start in background
docker-compose up -d --build

# Stop all services
docker-compose down

# View logs
docker-compose logs gateway
docker-compose logs -f  # Follow logs
```

---

## 7. What is an API Gateway?

### Definition

An **API Gateway** is a single entry point for all client requests. It acts as a reverse proxy that routes requests to appropriate backend services.

### Why Use an API Gateway?

#### Without Gateway (Direct Access)

```
Client → TodoService (Port 5001)
Client → UserService (Port 5002)
Client → OrderService (Port 5003)
...
```

**Problems:**

- Client needs to know all service URLs
- CORS issues
- No centralized authentication
- Difficult to manage

#### With Gateway

```
Client → Gateway (Port 5000) → TodoService
                              → UserService
                              → OrderService
```

**Benefits:**

- ✅ Single entry point
- ✅ Centralized authentication
- ✅ Request routing
- ✅ Response aggregation
- ✅ Rate limiting
- ✅ Logging and monitoring

### Gateway Responsibilities

1. **Request Routing**: Routes requests to correct service
2. **Authentication/Authorization**: Validates tokens, checks permissions
3. **Load Balancing**: Distributes requests across instances
4. **Rate Limiting**: Prevents abuse
5. **Request/Response Transformation**: Modifies data format
6. **Caching**: Stores frequently accessed data
7. **Logging**: Centralized logging
8. **Circuit Breaker**: Prevents cascading failures

### Gateway in This Project

**Location**: `src/Gateway/`

**Architecture:**

```
┌─────────────┐
│   Client    │
└──────┬──────┘
       │ HTTP Request
       ▼
┌─────────────────────────────┐
│      Gateway Service        │
│  ┌───────────────────────┐ │
│  │  Controllers          │ │
│  │  - TodoController     │ │
│  │  - UserController     │ │
│  └───────────┬───────────┘ │
│              │              │
│  ┌───────────▼───────────┐ │
│  │  Gateway Services     │ │
│  │  - TodoGatewayService │ │
│  │  - UserGatewayService │ │
│  └───────────┬───────────┘ │
└──────────────┼──────────────┘
               │ HTTP Calls
       ┌───────┴───────┐
       ▼               ▼
┌──────────┐    ┌──────────┐
│TodoService│    │UserService│
└──────────┘    └──────────┘
```

### Gateway Implementation

#### TodoController.cs

```csharp
[ApiController]
[Route("api/[controller]")]
public class TodoController : ControllerBase
{
    private readonly ITodoGatewayService _todoGatewayService;

    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<Todo>>>> GetAllTodos([FromQuery] Guid? userId)
    {
        var response = await _todoGatewayService.GetAllTodosAsync(userId);
        return StatusCode(response.Success ? 200 : 500, response);
    }
    // ... other endpoints
}
```

#### TodoGatewayService.cs

```csharp
public class TodoGatewayService : ITodoGatewayService
{
    private readonly IHttpClientService _httpClient;
    private readonly string _todoServiceUrl;

    public async Task<ApiResponse<List<Todo>>> GetAllTodosAsync(Guid? userId = null)
    {
        var url = userId.HasValue
            ? $"{_todoServiceUrl}/api/todo?userId={userId.Value}"
            : $"{_todoServiceUrl}/api/todo";

        var response = await _httpClient.GetAsync<ApiResponse<List<Todo>>>(url);
        return response ?? ApiResponse<List<Todo>>.ErrorResponse("Failed to retrieve todos");
    }
}
```

**Flow:**

1. Client calls `GET /api/todo` on Gateway
2. `TodoController.GetAllTodos()` receives request
3. Calls `TodoGatewayService.GetAllTodosAsync()`
4. Makes HTTP GET to `http://todoservice:5001/api/todo`
5. Returns response to client

---

## 8. Project Entry Points

### What is an Entry Point?

The **entry point** is where the application starts executing. In .NET, this is the `Program.cs` file.

### Entry Points in This Project

#### 1. Gateway Entry Point

**File**: `src/Gateway/Program.cs`

```csharp
using Gateway.Services;
using Shared.Services;

var builder = WebApplication.CreateBuilder(args);

// 1. Register Services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();

// 2. Register Custom Services
builder.Services.AddScoped<IHttpClientService>(sp => {
    var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
    var httpClient = httpClientFactory.CreateClient();
    return new HttpClientService(httpClient);
});

builder.Services.AddScoped<ITodoGatewayService, TodoGatewayService>();
builder.Services.AddScoped<IUserGatewayService, UserGatewayService>();

// 3. Configure CORS
builder.Services.AddCors(options => {
    options.AddPolicy("AllowAll", policy => {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// 4. Build Application
var app = builder.Build();

// 5. Configure Middleware Pipeline
if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI(c => {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Gateway API v1");
        c.RoutePrefix = "swagger";
    });
}

app.UseRouting();
app.UseCors("AllowAll");
app.UseAuthorization();

// 6. Map Endpoints
app.MapGet("/", () => Results.Redirect("/swagger"));
app.MapControllers();

// 7. Start Server
app.Run();
```

**Step-by-Step Explanation:**

1. **Create Builder**: `WebApplication.CreateBuilder(args)` creates the application builder
2. **Register Services**: Add services to dependency injection container
3. **Build App**: `builder.Build()` creates the application
4. **Configure Middleware**: Set up request/response pipeline
5. **Map Routes**: Define API endpoints
6. **Run**: Start the web server

#### 2. TodoService Entry Point

**File**: `src/TodoService/Program.cs`

```csharp
using TodoService.Services;

var builder = WebApplication.CreateBuilder(args);

// Register services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<ITodoService, TodoService.Services.TodoService>();

// CORS
builder.Services.AddCors(options => {
    options.AddPolicy("AllowAll", policy => {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure pipeline
if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();

app.Run();
```

#### 3. UserService Entry Point

**File**: `src/UserService/Program.cs`

Similar structure to TodoService, but registers `IUserService` instead.

### Middleware Pipeline

The middleware pipeline processes requests in order:

```
Request → Swagger → Routing → CORS → Authorization → Controllers → Response
```

**Order Matters!**

- `UseRouting()` must come before `MapControllers()`
- `UseCors()` must come after `UseRouting()`
- `UseAuthorization()` must come after `UseCors()`

### Dependency Injection

.NET has built-in dependency injection:

```csharp
// Register service
builder.Services.AddScoped<ITodoService, TodoService>();

// Use in controller
public class TodoController : ControllerBase
{
    private readonly ITodoService _todoService;

    public TodoController(ITodoService todoService)  // Injected automatically
    {
        _todoService = todoService;
    }
}
```

**Service Lifetimes:**

- **Singleton**: One instance for entire application
- **Scoped**: One instance per HTTP request
- **Transient**: New instance every time

---

## 9. Project Architecture Deep Dive

### Project Structure

```
DotNetMicroServices/
├── src/
│   ├── Gateway/              # API Gateway Service
│   │   ├── Controllers/      # API Controllers
│   │   ├── Services/         # Gateway Services
│   │   ├── Program.cs        # Entry point
│   │   └── appsettings.json  # Configuration
│   ├── TodoService/          # Todo Microservice
│   │   ├── Controllers/
│   │   ├── Services/
│   │   ├── Models/
│   │   ├── DTOs/
│   │   └── Program.cs
│   └── UserService/          # User Microservice
│       ├── Controllers/
│       ├── Services/
│       ├── Models/
│       ├── DTOs/
│       └── Program.cs
├── libs/
│   └── Shared/               # Shared Library
│       ├── Models/
│       ├── DTOs/
│       ├── Common/
│       ├── Services/
│       └── Constants/
├── Dockerfile.gateway
├── Dockerfile.todoservice
├── Dockerfile.userservice
└── docker-compose.yml
```

### Key Components

#### 1. Controllers

**Purpose**: Handle HTTP requests and responses

**Location**: `src/{Service}/Controllers/`

**Example**: `src/Gateway/Controllers/TodoController.cs`

```csharp
[ApiController]
[Route("api/[controller]")]  // Routes to /api/todo
public class TodoController : ControllerBase
{
    private readonly ITodoGatewayService _todoGatewayService;

    [HttpGet]  // GET /api/todo
    public async Task<ActionResult<ApiResponse<List<Todo>>>> GetAllTodos([FromQuery] Guid? userId)
    {
        var response = await _todoGatewayService.GetAllTodosAsync(userId);
        return StatusCode(response.Success ? 200 : 500, response);
    }
}
```

**Attributes:**

- `[ApiController]`: Enables API-specific features
- `[Route]`: Defines URL pattern
- `[HttpGet]`, `[HttpPost]`, etc.: HTTP method
- `[FromQuery]`, `[FromBody]`: Parameter binding

#### 2. Services

**Purpose**: Business logic and data access

**Location**: `src/{Service}/Services/`

**Example**: `src/TodoService/Services/TodoService.cs`

```csharp
public class TodoService : ITodoService
{
    private static readonly List<Todo> _todos = new();
    private static readonly object _lock = new();

    public Task<List<Todo>> GetAllTodosAsync()
    {
        lock (_lock)
        {
            return Task.FromResult(new List<Todo>(_todos));
        }
    }
    // ... other methods
}
```

**Patterns:**

- **Interface-based**: `ITodoService` defines contract
- **Async/Await**: All methods are async
- **Thread-safe**: Uses locks for in-memory storage

#### 3. Models

**Purpose**: Data structures

**Location**: `src/{Service}/Models/` and `libs/Shared/Models/`

**Example**: `libs/Shared/Models/Todo.cs`

```csharp
public class Todo
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsCompleted { get; set; }
    public Guid UserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
```

#### 4. DTOs (Data Transfer Objects)

**Purpose**: Data structures for API requests/responses

**Location**: `src/{Service}/DTOs/` and `libs/Shared/DTOs/`

**Example**: `libs/Shared/DTOs/CreateTodoDto.cs`

```csharp
public class CreateTodoDto
{
    [Required]
    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    [Required]
    public Guid UserId { get; set; }
}
```

**Why DTOs?**

- Separate API contract from internal models
- Validation attributes
- Versioning support
- Security (hide internal fields)

#### 5. ApiResponse<T>

**Purpose**: Standardized API responses

**Location**: `libs/Shared/Common/ApiResponse.cs`

```csharp
public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }
    public List<string> Errors { get; set; } = new();

    public static ApiResponse<T> SuccessResponse(T data, string message = "Operation successful")
    {
        return new ApiResponse<T>
        {
            Success = true,
            Message = message,
            Data = data
        };
    }
}
```

**Benefits:**

- Consistent response format
- Easy error handling
- Client-friendly structure

#### 6. HttpClientService

**Purpose**: Wrapper for HTTP client communication

**Location**: `libs/Shared/Services/HttpClientService.cs`

```csharp
public class HttpClientService : IHttpClientService
{
    private readonly HttpClient _httpClient;

    public async Task<T?> GetAsync<T>(string url)
    {
        var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<T>(_jsonOptions);
    }

    public async Task<T?> PostAsync<T>(string url, object data)
    {
        var response = await _httpClient.PostAsJsonAsync(url, data, _jsonOptions);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<T>(_jsonOptions);
    }
    // ... Put, Delete methods
}
```

**Features:**

- Generic methods for type safety
- JSON serialization/deserialization
- Error handling

---

## 10. Service Communication Flow

### Complete Request Flow

#### Example: Create a Todo

```
1. Client Request
   POST http://localhost:5000/api/todo
   Body: { "title": "Learn .NET", "userId": "..." }

2. Gateway Receives Request
   └─> TodoController.CreateTodo()
       └─> TodoGatewayService.CreateTodoAsync()
           └─> HTTP POST http://todoservice:5001/api/todo
               └─> HttpClientService.PostAsync()

3. TodoService Processes
   └─> TodoController.CreateTodo()
       └─> TodoService.CreateTodoAsync()
           └─> Adds to in-memory list
           └─> Returns Todo object

4. Response Flows Back
   TodoService → Gateway → Client
```

### Code Flow

#### Gateway Side

```csharp
// 1. Controller receives request
[HttpPost]
public async Task<ActionResult<ApiResponse<Todo>>> CreateTodo([FromBody] CreateTodoDto dto)
{
    // 2. Call gateway service
    var response = await _todoGatewayService.CreateTodoAsync(dto);
    return StatusCode(response.Success ? 201 : 500, response);
}

// 3. Gateway service makes HTTP call
public async Task<ApiResponse<Todo>> CreateTodoAsync(CreateTodoDto dto)
{
    var url = $"{_todoServiceUrl}/api/todo";
    var response = await _httpClient.PostAsync<ApiResponse<Todo>>(url, dto);
    return response ?? ApiResponse<Todo>.ErrorResponse("Failed to create todo");
}
```

#### TodoService Side

```csharp
// 1. Controller receives request
[HttpPost]
public async Task<ActionResult<ApiResponse<Todo>>> CreateTodo([FromBody] CreateTodoDto dto)
{
    var todo = new Todo
    {
        Title = dto.Title,
        Description = dto.Description,
        UserId = dto.UserId
    };

    // 2. Call business service
    var createdTodo = await _todoService.CreateTodoAsync(todo);
    var response = ApiResponse<Todo>.SuccessResponse(createdTodo, "Todo created successfully");
    return StatusCode(201, response);
}

// 3. Business logic
public Task<Todo> CreateTodoAsync(Todo todo)
{
    lock (_lock)
    {
        todo.Id = Guid.NewGuid();
        todo.CreatedAt = DateTimeHelper.GetUtcNow();
        todo.UpdatedAt = DateTimeHelper.GetUtcNow();
        _todos.Add(todo);
        return Task.FromResult(todo);
    }
}
```

### Error Handling

```csharp
try
{
    var response = await _httpClient.GetAsync<ApiResponse<List<Todo>>>(url);
    return response ?? ApiResponse<List<Todo>>.ErrorResponse("Failed to retrieve todos");
}
catch (Exception ex)
{
    _logger.LogError(ex, "Error calling TodoService");
    return ApiResponse<List<Todo>>.ErrorResponse("An error occurred while retrieving todos");
}
```

---

## 11. Running the Project

### Prerequisites

1. **.NET SDK 10.0** or later

   - Download from: https://dotnet.microsoft.com/download
   - Verify: `dotnet --version`

2. **Docker Desktop** (for Docker)
   - Download from: https://www.docker.com/products/docker-desktop
   - Verify: `docker --version`

### Running Locally

#### Step 1: Restore Dependencies

```bash
dotnet restore
```

#### Step 2: Build Solution

```bash
dotnet build
```

#### Step 3: Run Services

**Terminal 1 - Gateway:**

```bash
cd src/Gateway
dotnet run
```

**Terminal 2 - TodoService:**

```bash
cd src/TodoService
dotnet run
```

**Terminal 3 - UserService:**

```bash
cd src/UserService
dotnet run
```

#### Step 4: Access Services

- Gateway: http://localhost:5000/swagger
- TodoService: http://localhost:5001/swagger
- UserService: http://localhost:5002/swagger

### Running with Docker

#### Step 1: Build and Start

```bash
docker-compose up --build
```

#### Step 2: Access Services

- Gateway: http://localhost:5000/swagger
- TodoService: http://localhost:5001/swagger
- UserService: http://localhost:5002/swagger

#### Step 3: View Logs

```bash
docker-compose logs -f
```

#### Step 4: Stop Services

```bash
docker-compose down
```

### Testing the API

#### Create a User

```bash
curl -X POST http://localhost:5000/api/user \
  -H "Content-Type: application/json" \
  -d '{
    "username": "johndoe",
    "email": "john@example.com",
    "firstName": "John",
    "lastName": "Doe"
  }'
```

#### Create a Todo

```bash
curl -X POST http://localhost:5000/api/todo \
  -H "Content-Type: application/json" \
  -d '{
    "title": "Learn Microservices",
    "description": "Study .NET microservices architecture",
    "userId": "<user-id-from-previous-response>"
  }'
```

#### Get All Todos

```bash
curl http://localhost:5000/api/todo
```

---

## 12. Key Concepts Explained

### Dependency Injection

**What**: Design pattern where objects receive dependencies from external source

**In .NET:**

```csharp
// Register
builder.Services.AddScoped<ITodoService, TodoService>();

// Use
public class TodoController : ControllerBase
{
    private readonly ITodoService _todoService;

    public TodoController(ITodoService todoService)  // Injected
    {
        _todoService = todoService;
    }
}
```

**Benefits:**

- Loose coupling
- Testability
- Maintainability

### Async/Await

**What**: Pattern for asynchronous programming

**Example:**

```csharp
public async Task<List<Todo>> GetAllTodosAsync()
{
    // Non-blocking operation
    var response = await _httpClient.GetAsync(url);
    return await response.Content.ReadFromJsonAsync<List<Todo>>();
}
```

**Why:**

- Non-blocking I/O
- Better resource utilization
- Scalability

### CORS (Cross-Origin Resource Sharing)

**What**: Mechanism to allow web pages to make requests to different domains

**In This Project:**

```csharp
builder.Services.AddCors(options => {
    options.AddPolicy("AllowAll", policy => {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});
```

**Why Needed:**

- Frontend (different port) needs to call API
- Browser security restriction

### Swagger/OpenAPI

**What**: API documentation and testing tool

**In This Project:**

- Automatically generates API documentation
- Interactive testing interface
- Available at `/swagger` endpoint

### Configuration

**appsettings.json**: Application configuration

```json
{
  "ServiceUrls": {
    "TodoService": "http://localhost:5001",
    "UserService": "http://localhost:5002"
  }
}
```

**Environment Variables**: Override configuration

```bash
ServiceUrls__TodoService=http://todoservice:5001
```

**Double Underscore (`__`)**: Represents nested configuration in .NET

### Health Checks

**What**: Mechanism to verify service is running correctly

**In Docker Compose:**

```yaml
healthcheck:
  test: ["CMD", "curl", "-f", "http://localhost:5001/swagger/v1/swagger.json"]
  interval: 10s
  timeout: 5s
  retries: 5
```

**Purpose:**

- Ensure service is ready before dependent services start
- Monitor service health

### Service Discovery

**Current**: Hardcoded service URLs

- Local: `http://localhost:5001`
- Docker: `http://todoservice:5001`

**Production Options:**

- Consul
- Eureka
- Kubernetes Service Discovery
- Azure Service Fabric

---

## Summary

This project demonstrates:

1. **Microservices Architecture**: Three independent services
2. **API Gateway Pattern**: Single entry point
3. **Service Communication**: HTTP/REST between services
4. **Docker Containerization**: Containerized deployment
5. **Shared Library**: Code reuse across services
6. **.NET Best Practices**: Dependency injection, async/await, clean architecture

### Key Takeaways

- **Microservices** break large applications into small, independent services
- **API Gateway** provides single entry point and routing
- **Docker** ensures consistent deployment across environments
- **.NET** provides robust framework for building microservices
- **Shared Library** reduces code duplication and ensures consistency

### Next Steps

1. Add database (Entity Framework Core)
2. Implement authentication (JWT)
3. Add message queue (RabbitMQ)
4. Implement service discovery
5. Add logging and monitoring
6. Implement circuit breaker pattern
7. Add rate limiting
8. Implement caching

---

## Additional Resources

- [.NET Documentation](https://docs.microsoft.com/dotnet/)
- [ASP.NET Core Documentation](https://docs.microsoft.com/aspnet/core)
- [Docker Documentation](https://docs.docker.com/)
- [Microservices Patterns](https://microservices.io/patterns/)
- [API Gateway Pattern](https://microservices.io/patterns/apigateway.html)

---

**Happy Learning! 🚀**
