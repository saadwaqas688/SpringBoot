# Todo Microservices - .NET Implementation

A learning project demonstrating a microservices architecture using .NET, similar to the NestJS microservices structure. This project implements a Todo List application with three microservices.

## Architecture

This project follows a microservices architecture pattern with:

- **Gateway Service** (Port 5000): API Gateway that routes requests to appropriate microservices
- **Todo Service** (Port 5001): Manages todo items (CRUD operations)
- **User Service** (Port 5002): Manages user accounts (CRUD operations)
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
│   ├── TodoService/          # Todo Microservice
│   │   ├── Controllers/      # Todo API Controllers
│   │   ├── Services/         # Todo Business Logic
│   │   ├── Models/           # Todo Models
│   │   └── DTOs/             # Data Transfer Objects
│   └── UserService/          # User Microservice
│       ├── Controllers/      # User API Controllers
│       ├── Services/         # User Business Logic
│       ├── Models/           # User Models
│       └── DTOs/             # Data Transfer Objects
├── libs/
│   └── Shared/               # Shared Library
│       ├── Models/           # Shared Models
│       ├── DTOs/             # Shared DTOs
│       ├── Common/           # Common classes (ApiResponse, etc.)
│       ├── Constants/       # Constants
│       ├── Utils/            # Utility functions
│       └── Services/        # Shared Services (HttpClient)
├── Dockerfile.gateway        # Gateway Dockerfile
├── Dockerfile.todoservice    # TodoService Dockerfile
├── Dockerfile.userservice    # UserService Dockerfile
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

   Terminal 2 - TodoService:

   ```bash
   cd src/TodoService
   dotnet run
   ```

   Terminal 3 - UserService:

   ```bash
   cd src/UserService
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

#### User Endpoints

- `GET /api/user` - Get all users
- `GET /api/user/{id}` - Get user by ID
- `GET /api/user/email/{email}` - Get user by email
- `POST /api/user` - Create a new user
- `PUT /api/user/{id}` - Update a user
- `DELETE /api/user/{id}` - Delete a user

### Direct Service Access

#### TodoService (http://localhost:5001)

- Same endpoints as Gateway `/api/todo/*`

#### UserService (http://localhost:5002)

- Same endpoints as Gateway `/api/user/*`

## Example API Calls

### Create a User

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

### Create a Todo

```bash
curl -X POST http://localhost:5000/api/todo \
  -H "Content-Type: application/json" \
  -d '{
    "title": "Learn Microservices",
    "description": "Study .NET microservices architecture",
    "userId": "<user-id-from-previous-response>"
  }'
```

### Get All Todos for a User

```bash
curl http://localhost:5000/api/todo?userId=<user-id>
```

## Swagger Documentation

When running in Development mode, Swagger UI is available at:

- Gateway: http://localhost:5000/swagger
- TodoService: http://localhost:5001/swagger
- UserService: http://localhost:5002/swagger

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
