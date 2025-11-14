# Todo Microservice

A Spring Boot microservice for managing todo lists with RESTful API endpoints.

## Features

- Create, read, update, and delete todos
- Mark todos as completed/incomplete
- Search todos by title
- Filter todos by completion status
- Input validation
- MongoDB database for data persistence
- **Hot Reload / Auto-Restart** - Automatic application restart on code changes (like nodemon)

## Technology Stack

- **Spring Boot 3.2.0**
- **Spring Data MongoDB**
- **MongoDB** - NoSQL database
- **SpringDoc OpenAPI (Swagger UI)** - Interactive API documentation
- **Maven**
- **Java 17**

## Prerequisites

- **Java 17 or higher** (required)
- **MongoDB** - Local MongoDB instance running on default port (27017)
- Maven is included via Maven Wrapper (no need to install separately)

### MongoDB Setup

1. **Install MongoDB** (if not already installed):

   - Download from: https://www.mongodb.com/try/download/community
   - Or use Docker: `docker run -d -p 27017:27017 --name mongodb mongo:latest`

2. **Start MongoDB**:

   - Windows: Run `mongod` from MongoDB installation directory
   - Linux/Mac: `sudo systemctl start mongod` or `brew services start mongodb-community`
   - Docker: `docker start mongodb`

3. **Verify MongoDB is running**:
   - Default connection: `localhost:27017`
   - The application will automatically connect to the `tododb` database

## Running the Application

### Option 1: Using Maven Wrapper (Recommended - No Maven Installation Required)

**On Windows:**

```cmd
.\mvnw.cmd spring-boot:run
```

**On Linux/Mac:**

```bash
./mvnw spring-boot:run
```

### Option 2: Using IDE

1. Open the project in your IDE (IntelliJ IDEA, Eclipse, VS Code, etc.)
2. Navigate to `src/main/java/com/example/todo/TodoMicroserviceApplication.java`
3. Right-click and select "Run" or press the run button

### Option 3: If Maven is Installed Globally

```bash
mvn spring-boot:run
```

### Build the Project First (Optional)

If you want to build the project before running:

**Windows:**

```cmd
.\mvnw.cmd clean install
```

**Linux/Mac:**

```bash
./mvnw clean install
```

The application will start on `http://localhost:8080`

## Hot Reload / Auto-Restart (Like nodemon)

The application includes **Spring Boot DevTools** for automatic restart on code changes, similar to `nodemon` in Express.js.

### How It Works

- **Automatic Restart**: When you save any Java file or configuration, the app automatically restarts
- **Fast Restart**: Only the Spring context restarts (1-2 seconds), not the full JVM
- **LiveReload**: Optional browser auto-refresh when code changes

### Usage

Just run the application normally:

```cmd
.\mvnw.cmd spring-boot:run
```

Then:

- Edit any `.java` file → Save → App restarts automatically! 🚀
- Edit `application.properties` → Save → App restarts automatically! 🚀

**No manual restart needed!**

For detailed information, see [HOT_RELOAD_GUIDE.md](HOT_RELOAD_GUIDE.md)

## Swagger UI - API Documentation

The application includes **Swagger UI** for interactive API documentation and testing.

### Access Swagger UI

Once the application is running, open your browser and navigate to:

**Swagger UI:** http://localhost:8080/swagger-ui.html

**OpenAPI JSON:** http://localhost:8080/api-docs

### Features

- **Interactive API Testing**: Test all endpoints directly from the browser
- **Request/Response Examples**: See example requests and responses
- **Schema Documentation**: View data models and validation rules
- **Try It Out**: Execute API calls and see real responses

### What You'll See

- All available endpoints organized by tags
- Request parameters and body schemas
- Response codes and examples
- Model definitions for Todo entity

## API Endpoints

### Get All Todos

```
GET /api/todos
```

### Get Todo by ID

```
GET /api/todos/{id}
```

### Get Todos by Completion Status

```
GET /api/todos/completed/{true|false}
```

### Search Todos by Title

```
GET /api/todos/search?title={searchTerm}
```

### Create Todo

```
POST /api/todos
Content-Type: application/json

{
  "title": "Task title",
  "description": "Task description",
  "completed": false
}
```

### Update Todo

```
PUT /api/todos/{id}
Content-Type: application/json

{
  "title": "Updated title",
  "description": "Updated description",
  "completed": true
}
```

### Toggle Todo Status

```
PATCH /api/todos/{id}/toggle
```

### Delete Todo

```
DELETE /api/todos/{id}
```

### Delete All Todos

```
DELETE /api/todos
```

## MongoDB Connection

The application connects to MongoDB using the following default settings:

- **Host**: `localhost`
- **Port**: `27017`
- **Database**: `tododb`

### MongoDB Configuration

You can modify the connection settings in `src/main/resources/application.properties`:

```properties
spring.data.mongodb.host=localhost
spring.data.mongodb.port=27017
spring.data.mongodb.database=tododb
```

### Accessing MongoDB

You can use MongoDB Compass (GUI) or MongoDB Shell to view and manage your data:

- **MongoDB Compass**: https://www.mongodb.com/products/compass
- **MongoDB Shell**: `mongosh` (command line tool)

## Example Requests

### Create a Todo

```bash
curl -X POST http://localhost:8080/api/todos \
  -H "Content-Type: application/json" \
  -d '{
    "title": "Complete Spring Boot project",
    "description": "Finish the todo microservice implementation",
    "completed": false
  }'
```

### Get All Todos

```bash
curl http://localhost:8080/api/todos
```

### Update Todo

```bash
curl -X PUT http://localhost:8080/api/todos/1 \
  -H "Content-Type: application/json" \
  -d '{
    "title": "Updated task",
    "description": "Updated description",
    "completed": true
  }'
```

### Toggle Todo Status

```bash
curl -X PATCH http://localhost:8080/api/todos/1/toggle
```

### Delete Todo

```bash
curl -X DELETE http://localhost:8080/api/todos/1
```

## Project Structure

```
src/
├── main/
│   ├── java/
│   │   └── com/example/todo/
│   │       ├── TodoMicroserviceApplication.java
│   │       ├── controller/
│   │       │   ├── TodoController.java
│   │       │   └── GlobalExceptionHandler.java
│   │       ├── model/
│   │       │   └── Todo.java
│   │       ├── repository/
│   │       │   └── TodoRepository.java
│   │       └── service/
│   │           └── TodoService.java
│   └── resources/
│       ├── application.properties
│       └── application.yml
```

## License

This project is open source and available for use.
