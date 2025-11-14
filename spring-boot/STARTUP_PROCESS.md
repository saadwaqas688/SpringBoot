# Spring Boot Application Startup Process

This document explains how the Todo Microservice application starts and initializes.

## Entry Point: Main Method

The application starts from the `main` method in `TodoMicroserviceApplication.java`:

```java
@SpringBootApplication
public class TodoMicroserviceApplication {
    public static void main(String[] args) {
        SpringApplication.run(TodoMicroserviceApplication.class, args);
    }
}
```

## Step-by-Step Startup Process

### 1. **Application Launch**

- When you run `.\mvnw.cmd spring-boot:run` or execute the main class, Java calls the `main` method
- `SpringApplication.run()` is invoked with the application class and command-line arguments

### 2. **@SpringBootApplication Annotation**

The `@SpringBootApplication` annotation is a composite annotation that includes:

- `@SpringBootConfiguration` - Marks this as a Spring Boot configuration class
- `@EnableAutoConfiguration` - Enables Spring Boot's auto-configuration
- `@ComponentScan` - Scans for Spring components (controllers, services, repositories, etc.)

### 3. **Component Scanning**

Spring Boot scans the package and sub-packages of `TodoMicroserviceApplication`:

- Finds all classes annotated with:
  - `@Controller`, `@RestController` → `TodoController`, `HomeController`
  - `@Service` → `TodoService`
  - `@Repository` → `TodoRepository`
  - `@Configuration` → `OpenApiConfig`
  - `@Component` → `GlobalExceptionHandler`

### 4. **Dependency Injection & Bean Creation**

Spring creates and wires beans in this order:

**a. Configuration Beans:**

- `OpenApiConfig` → Creates `OpenAPI` bean for Swagger documentation

**b. Repository Layer:**

- `TodoRepository` → Spring Data MongoDB creates proxy implementation

**c. Service Layer:**

- `TodoService` → Injected with `TodoRepository`

**d. Controller Layer:**

- `TodoController` → Injected with `TodoService`
- `HomeController` → Standalone controller
- `GlobalExceptionHandler` → Global exception handling

### 5. **Auto-Configuration**

Spring Boot automatically configures:

**a. Web Server (Embedded Tomcat):**

- Reads `server.port=8080` from `application.properties`
- Starts embedded Tomcat server on port 8080

**b. MongoDB Connection:**

- Reads MongoDB settings from `application.properties`:
  ```properties
  spring.data.mongodb.host=localhost
  spring.data.mongodb.port=27017
  spring.data.mongodb.database=tododb
  ```
- Creates `MongoClient` and connects to MongoDB
- Sets up `MongoTemplate` for database operations

**c. Spring MVC:**

- Configures DispatcherServlet
- Sets up request mapping handlers
- Configures JSON serialization/deserialization

**d. Validation:**

- Enables Jakarta Bean Validation
- Configures validation interceptors

### 6. **Swagger/OpenAPI Initialization**

- SpringDoc scans for `@Operation`, `@ApiResponse` annotations
- Generates OpenAPI specification
- Serves Swagger UI at `/swagger-ui.html`
- Serves API docs at `/api-docs`

### 7. **Request Mapping Registration**

Spring registers all REST endpoints:

- `GET /` → `HomeController.home()`
- `GET /api/todos` → `TodoController.getAllTodos()`
- `GET /api/todos/{id}` → `TodoController.getTodoById()`
- `POST /api/todos` → `TodoController.createTodo()`
- `PUT /api/todos/{id}` → `TodoController.updateTodo()`
- `PATCH /api/todos/{id}/toggle` → `TodoController.toggleTodoStatus()`
- `DELETE /api/todos/{id}` → `TodoController.deleteTodo()`
- `DELETE /api/todos` → `TodoController.deleteAllTodos()`
- `GET /swagger-ui.html` → Swagger UI interface
- `GET /api-docs` → OpenAPI JSON

### 8. **Application Ready**

Once all components are initialized:

- Embedded Tomcat starts listening on port 8080
- Application context is fully loaded
- You see the Spring Boot banner and "Started TodoMicroserviceApplication" message

## Startup Logs Explained

When the application starts, you'll see logs like:

```
  .   ____          _            __ _ _
 /\\ / ___'_ __ _ _(_)_ __  __ _ \ \ \ \
( ( )\___ | '_ | '_| | '_ \/ _` | \ \ \ \
 \\/  ___)| |_)| | | | | || (_| |  ) ) ) )
  '  |____| .__|_| |_|_| |_\__, | / / / /
 =========|_|==============|___/=/_/_/_/
 :: Spring Boot ::                (v3.2.0)

2025-11-06 12:00:00.123  INFO --- [main] c.e.t.TodoMicroserviceApplication : Starting TodoMicroserviceApplication
2025-11-06 12:00:00.456  INFO --- [main] o.s.b.w.embedded.tomcat.TomcatWebServer : Tomcat initialized with port(s): 8080 (http)
2025-11-06 12:00:00.789  INFO --- [main] o.s.d.m.core.MongoTemplate : Connecting to MongoDB at localhost:27017
2025-11-06 12:00:01.234  INFO --- [main] o.s.b.w.embedded.tomcat.TomcatWebServer : Tomcat started on port(s): 8080 (http)
2025-11-06 12:00:01.567  INFO --- [main] c.e.t.TodoMicroserviceApplication : Started TodoMicroserviceApplication in 1.444 seconds
```

## Component Initialization Order

1. **Configuration Classes** → `OpenApiConfig`
2. **Repositories** → `TodoRepository` (MongoDB repository proxy)
3. **Services** → `TodoService` (depends on repository)
4. **Controllers** → `TodoController`, `HomeController` (depend on services)
5. **Exception Handlers** → `GlobalExceptionHandler`
6. **Web Server** → Embedded Tomcat
7. **MongoDB Connection** → MongoClient connection pool

## Key Configuration Files

### `application.properties`

- Server port configuration
- MongoDB connection settings
- Logging levels
- Swagger UI settings

### `pom.xml`

- Defines all dependencies
- Spring Boot parent POM provides default configurations
- Maven plugins for building and running

## Lifecycle Hooks

Spring Boot provides several lifecycle hooks (not currently used, but available):

- `@PostConstruct` - Called after bean initialization
- `@PreDestroy` - Called before bean destruction
- `ApplicationListener` - Listen to application events
- `CommandLineRunner` - Execute code after application starts
- `ApplicationRunner` - Similar to CommandLineRunner

## Application Shutdown

When you press `Ctrl+C` or stop the application:

1. Spring Boot gracefully shuts down the application context
2. Closes MongoDB connections
3. Stops the embedded Tomcat server
4. Destroys all beans in reverse order

## Troubleshooting Startup Issues

**If MongoDB is not running:**

- Error: `Cannot connect to MongoDB at localhost:27017`
- Solution: Start MongoDB before running the application

**If port 8080 is in use:**

- Error: `Port 8080 is already in use`
- Solution: Change `server.port` in `application.properties`

**If dependencies are missing:**

- Error: `ClassNotFoundException` or `NoClassDefFoundError`
- Solution: Run `mvn clean install` to download dependencies

## Summary

The application startup is a sophisticated process where Spring Boot:

1. Scans for components
2. Creates and wires beans
3. Auto-configures infrastructure (web server, database, etc.)
4. Registers REST endpoints
5. Starts the embedded server
6. Makes the application ready to handle HTTP requests

All of this happens automatically thanks to Spring Boot's convention-over-configuration approach!


