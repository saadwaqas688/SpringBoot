# ASP.NET Core MVC Training Guide for NestJS Developers

## Table of Contents

1. [Introduction](#introduction)
2. [Entry Point - Program.cs](#entry-point---programcs)
3. [Project Structure](#project-structure)
4. [Key Terminologies](#key-terminologies)
5. [Controllers Deep Dive](#controllers-deep-dive)
6. [Services Deep Dive](#services-deep-dive)
7. [Repositories Deep Dive](#repositories-deep-dive)
8. [Models and DTOs](#models-and-dtos)
9. [Middleware](#middleware)
10. [Configuration](#configuration)
11. [Dependency Injection](#dependency-injection)
12. [Authentication & Authorization](#authentication--authorization)

---

## Introduction

### What is ASP.NET Core MVC?

ASP.NET Core MVC is Microsoft's web framework for building RESTful APIs and web applications using C#. If you're coming from NestJS, you'll find many similarities:

| NestJS Concept      | ASP.NET Core Equivalent                |
| ------------------- | -------------------------------------- |
| `main.ts`           | `Program.cs`                           |
| `@Controller()`     | `[ApiController]` + `[Route()]`        |
| `@Injectable()`     | Service class (DI handles it)          |
| `@Module()`         | Service registration in `Program.cs`   |
| `@Get()`, `@Post()` | `[HttpGet]`, `[HttpPost]`              |
| `@UseGuards()`      | `[Authorize]`                          |
| `@Body()`           | `[FromBody]`                           |
| `@Param()`          | Route parameters in method signature   |
| `class-validator`   | Data Annotations (`[Required]`, etc.)  |
| `@nestjs/mongoose`  | `MongoDB.Driver`                       |
| `ExceptionFilter`   | Middleware or try-catch in controllers |

---

## Entry Point - Program.cs

### Overview

`Program.cs` is the entry point of your ASP.NET Core application, similar to `main.ts` in NestJS. It's where you:

- Configure services (like `app.module.ts` in NestJS)
- Set up middleware pipeline
- Configure authentication
- Register dependencies

### Line-by-Line Breakdown

```csharp
var builder = WebApplication.CreateBuilder(args);
```

**NestJS Equivalent:** `const app = await NestFactory.create(AppModule);`

This creates a builder pattern object that configures your application.

```csharp
builder.Services.AddControllers();
```

**NestJS Equivalent:** `app.useGlobalPipes()` - Enables controller routing

```csharp
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => { ... });
```

**NestJS Equivalent:** `SwaggerModule.setup()` - Sets up Swagger/OpenAPI documentation

```csharp
builder.Services.AddSingleton<IMongoClient>(sp => new MongoClient(connectionString));
builder.Services.AddScoped<IMongoDatabase>(sp => { ... });
```

**NestJS Equivalent:**

```typescript
MongooseModule.forRoot(connectionString);
```

**Key Difference:**

- `AddSingleton` = One instance for the entire application lifetime (like a service in NestJS with `@Injectable()` scope)
- `AddScoped` = One instance per HTTP request (like NestJS default scope)
- `AddTransient` = New instance every time (rarely used)

```csharp
builder.Services.AddAuthentication(...)
    .AddJwtBearer(...);
```

**NestJS Equivalent:** `JwtModule.register()` - Configures JWT authentication

```csharp
builder.Services.AddScoped<IUserRepository, UserRepository>();
```

**NestJS Equivalent:**

```typescript
@Module({
  providers: [UserRepository]
})
```

This registers the repository with Dependency Injection. The interface (`IUserRepository`) is the contract, and `UserRepository` is the implementation.

```csharp
var app = builder.Build();
```

**NestJS Equivalent:** `await app.init()` - Builds the application

```csharp
app.UseSwagger();
app.UseSwaggerUI();
```

**NestJS Equivalent:** `SwaggerModule.setup('api', app, document)` - Enables Swagger UI

```csharp
app.UseCors("AllowAll");
```

**NestJS Equivalent:** `app.enableCors()` - Enables CORS

```csharp
app.UseAuthentication();
app.UseAuthorization();
```

**NestJS Equivalent:** `app.useGlobalGuards(new AuthGuard())` - Enables authentication/authorization

**Important:** Order matters! Authentication must come before Authorization.

```csharp
app.UseMiddleware<ErrorHandlingMiddleware>();
```

**NestJS Equivalent:** `app.useGlobalFilters(new HttpExceptionFilter())` - Global exception handler

```csharp
app.MapControllers();
```

**NestJS Equivalent:** `app.listen(3000)` - Maps all controllers and starts listening

---

## Project Structure

```
CourseManagementAPI/
├── Controllers/          # API endpoints (like NestJS controllers)
│   ├── AuthController.cs
│   ├── CourseController.cs
│   ├── DiscussionController.cs
│   ├── PostController.cs
│   ├── EnrollmentController.cs
│   └── UserController.cs
│
├── Services/             # Business logic (like NestJS services)
│   ├── AuthService.cs
│   ├── CourseService.cs
│   ├── DiscussionService.cs
│   ├── PostService.cs
│   ├── EnrollmentService.cs
│   └── JwtService.cs
│
├── Repositories/         # Data access layer (like NestJS repositories)
│   ├── UserRepository.cs
│   ├── CourseRepository.cs
│   ├── DiscussionRepository.cs
│   ├── PostRepository.cs
│   └── CourseEnrollmentRepository.cs
│
├── Models/               # Entity models (like Mongoose schemas)
│   ├── User.cs
│   ├── Course.cs
│   ├── Discussion.cs
│   ├── Post.cs
│   └── CourseEnrollment.cs
│
├── DTOs/                 # Data Transfer Objects (like DTOs in NestJS)
│   ├── SignupRequest.cs
│   ├── SigninRequest.cs
│   ├── AuthResponse.cs
│   ├── CreatePostRequest.cs
│   └── UpdatePostRequest.cs
│
├── Middleware/           # Custom middleware (like NestJS interceptors/guards)
│   └── ErrorHandlingMiddleware.cs
│
├── Program.cs            # Entry point (like main.ts)
├── appsettings.json      # Configuration (like .env or config files)
└── CourseManagementAPI.csproj  # Project file (like package.json)
```

### Comparison with NestJS Structure

| ASP.NET Core       | NestJS                                 |
| ------------------ | -------------------------------------- |
| `Controllers/`     | `src/controllers/`                     |
| `Services/`        | `src/services/`                        |
| `Repositories/`    | `src/repositories/` or Mongoose models |
| `Models/`          | Mongoose schemas                       |
| `DTOs/`            | `src/dto/`                             |
| `Middleware/`      | `src/interceptors/` or `src/guards/`   |
| `Program.cs`       | `src/main.ts`                          |
| `appsettings.json` | `.env` or `config/`                    |

---

## Key Terminologies

This section explains all the key terminologies used in ASP.NET Core and their equivalents in NestJS, Node.js, and JavaScript.

### 1. Attributes (Decorators in NestJS/JavaScript)

**ASP.NET Core:**

```csharp
[ApiController]        // Marks class as API controller
[Route("api/[controller]")]  // Defines base route
[Authorize]            // Requires authentication
[HttpPost("signup")]   // HTTP method decorator
[FromBody]             // Binds request body to parameter
```

**NestJS Equivalent:**

```typescript
@Controller('api/auth')      // Marks class as controller
@UseGuards(AuthGuard)        // Requires authentication
@Post('signup')              // HTTP method decorator
@Body()                      // Binds request body to parameter
```

**Node.js/Express Equivalent:**

```javascript
// No decorators - uses function-based approach
router.post("/api/auth/signup", authenticate, (req, res) => {
  // req.body contains the request body
});
```

**Explanation:**

- **Attributes** in C# are metadata annotations (like decorators in TypeScript)
- They provide declarative information about code elements
- In JavaScript/Node.js, you use middleware functions instead of decorators

### 2. Dependency Injection (DI)

**ASP.NET Core:**

```csharp
// Service Registration
builder.Services.AddScoped<IUserRepository, UserRepository>();

// Constructor Injection
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)  // DI via constructor
    {
        _authService = authService;
    }
}
```

**NestJS Equivalent:**

```typescript
// Service Registration (in Module)
@Module({
  providers: [AuthService, UserRepository],
})
// Constructor Injection
@Controller()
export class AuthController {
  constructor(
    private readonly authService: AuthService // DI via constructor
  ) {}
}
```

**Node.js/Express Equivalent:**

```javascript
// Manual dependency injection
class AuthController {
  constructor(authService, userRepository) {
    this.authService = authService;
    this.userRepository = userRepository;
  }
}

// Usage
const authService = new AuthService();
const userRepository = new UserRepository();
const authController = new AuthController(authService, userRepository);
```

**Explanation:**

- **Dependency Injection** = Providing dependencies to a class from outside
- ASP.NET Core and NestJS have built-in DI containers
- Node.js typically uses manual DI or libraries like `inversify`

### 3. Interface vs Implementation

**ASP.NET Core:**

```csharp
// Interface (contract)
public interface IUserRepository
{
    Task<User?> GetByIdAsync(string id);
}

// Implementation
public class UserRepository : IUserRepository
{
    public async Task<User?> GetByIdAsync(string id) { ... }
}

// Registration
builder.Services.AddScoped<IUserRepository, UserRepository>();
```

**NestJS Equivalent:**

```typescript
// Interface (TypeScript)
export interface IUserRepository {
  findById(id: string): Promise<User | null>;
}

// Implementation
@Injectable()
export class UserRepository implements IUserRepository {
  async findById(id: string): Promise<User | null> { ... }
}

// Registration (uses class directly)
@Module({
  providers: [UserRepository]
})
```

**JavaScript Equivalent:**

```javascript
// No interfaces in JavaScript - use JSDoc or TypeScript
class UserRepository {
  async findById(id) {
    // Implementation
  }
}
```

**Explanation:**

- **Interface** = Contract defining what methods a class must implement
- C# interfaces are enforced at compile-time
- TypeScript interfaces are compile-time only (removed in JavaScript)
- JavaScript doesn't have interfaces (use TypeScript or JSDoc)

### 4. Async/Await Pattern

**ASP.NET Core:**

```csharp
public async Task<ActionResult<User>> GetUser(string id)
{
    var user = await _userService.GetByIdAsync(id);
    return Ok(user);
}
```

**NestJS Equivalent:**

```typescript
async getUser(@Param('id') id: string): Promise<User> {
  const user = await this.userService.findById(id);
  return user;
}
```

**Node.js/JavaScript Equivalent:**

```javascript
// Using async/await
async function getUser(id) {
  const user = await userService.findById(id);
  return user;
}

// Using Promises (older style)
function getUser(id) {
  return userService
    .findById(id)
    .then((user) => user)
    .catch((error) => {
      throw error;
    });
}
```

**Explanation:**

- **async/await** = Syntactic sugar for Promises
- `Task<T>` in C# = `Promise<T>` in TypeScript/JavaScript
- All three languages support async/await syntax

### 5. Nullable Types

**ASP.NET Core:**

```csharp
public string? Id { get; set; }  // ? = nullable (can be null)
public string Email { get; set; } = string.Empty;  // Non-nullable with default
```

**NestJS/TypeScript Equivalent:**

```typescript
id?: string;        // Optional (can be undefined)
email: string;      // Required (cannot be undefined)
```

**JavaScript Equivalent:**

```javascript
// No type system - all variables can be null/undefined
let id = null; // Can be null
let email = ""; // Can be empty string
```

**Explanation:**

- **Nullable types** (`?` in C#) = Optional properties (`?` in TypeScript)
- C# distinguishes between `null` and `undefined`
- TypeScript distinguishes between `null` and `undefined`
- JavaScript treats both as falsy values

### 6. LINQ (Language Integrated Query)

**ASP.NET Core:**

```csharp
// LINQ-style query
var user = await _users.Find(u => u.Email == email).FirstOrDefaultAsync();

// LINQ filtering
var activeUsers = users.Where(u => u.IsActive).ToList();
```

**NestJS/TypeScript Equivalent:**

```typescript
// Array methods
const user = await this.userModel.findOne({ email }).exec();
const activeUsers = users.filter((u) => u.isActive);
```

**JavaScript Equivalent:**

```javascript
// Array methods (same as TypeScript)
const user = await userModel.findOne({ email }).exec();
const activeUsers = users.filter((u) => u.isActive);
```

**Explanation:**

- **LINQ** = Query language integrated into C#
- JavaScript/TypeScript use array methods: `filter()`, `map()`, `find()`, etc.
- MongoDB Driver in C# uses LINQ-style syntax
- Mongoose in Node.js uses query builder syntax

### 7. Generics

**ASP.NET Core:**

```csharp
public interface IRepository<T>  // T is a type parameter
{
    Task<T?> GetByIdAsync(string id);
}

public class UserRepository : IRepository<User>  // User replaces T
{
    // Implementation
}
```

**NestJS/TypeScript Equivalent:**

```typescript
interface IRepository<T> {
  // T is a type parameter
  findById(id: string): Promise<T | null>;
}

class UserRepository implements IRepository<User> {
  // User replaces T
  // Implementation
}
```

**JavaScript Equivalent:**

```javascript
// No generics - use runtime type checking
class Repository {
  async findById(id) {
    // Returns any type
  }
}
```

**Explanation:**

- **Generics** = Type parameters that work with any type
- C# and TypeScript have compile-time generics
- JavaScript doesn't have generics (use TypeScript)

### 8. Service Lifetime (Scope)

**ASP.NET Core:**

```csharp
builder.Services.AddSingleton<IService, Service>();  // One instance for app lifetime
builder.Services.AddScoped<IService, Service>();     // One instance per HTTP request
builder.Services.AddTransient<IService, Service>();  // New instance every time
```

**NestJS Equivalent:**

```typescript
@Injectable({ scope: Scope.DEFAULT })    // Singleton (one instance)
@Injectable({ scope: Scope.REQUEST })    // One instance per request
@Injectable({ scope: Scope.TRANSIENT })  // New instance every time
```

**Node.js Equivalent:**

```javascript
// Manual management
const service = new Service(); // Singleton (if created once)
// Or create new instance each time
function createService() {
  return new Service();
}
```

**Explanation:**

- **Service Lifetime** = How long a service instance lives
- **Singleton** = One instance shared across entire application
- **Scoped** = One instance per HTTP request (most common)
- **Transient** = New instance every time it's requested

### 9. Middleware

**ASP.NET Core:**

```csharp
public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;  // Next middleware in pipeline

    public async Task InvokeAsync(HttpContext context)
    {
        await _next(context);  // Call next middleware
    }
}
```

**NestJS Equivalent:**

```typescript
@Injectable()
export class LoggingInterceptor implements NestInterceptor {
  intercept(context: ExecutionContext, next: CallHandler) {
    return next.handle(); // Call next handler
  }
}
```

**Node.js/Express Equivalent:**

```javascript
function errorHandlingMiddleware(req, res, next) {
  try {
    next(); // Call next middleware
  } catch (error) {
    // Handle error
  }
}

app.use(errorHandlingMiddleware);
```

**Explanation:**

- **Middleware** = Functions that execute in sequence during request processing
- **RequestDelegate** = Function pointer to next middleware
- Express uses `next()` callback
- NestJS uses interceptors/guards

### 10. ActionResult vs Direct Return

**ASP.NET Core:**

```csharp
public async Task<ActionResult<User>> GetUser(string id)
{
    if (user == null)
        return NotFound();      // Returns 404
    return Ok(user);            // Returns 200 with data
    return BadRequest();        // Returns 400
    return StatusCode(201, user); // Returns 201
}
```

**NestJS Equivalent:**

```typescript
async getUser(id: string): Promise<User> {
  if (!user) {
    throw new NotFoundException();  // Throws 404
  }
  return user;  // Returns 200 automatically
}

@HttpCode(201)
async createUser(): Promise<User> {
  return user;  // Returns 201
}
```

**Node.js/Express Equivalent:**

```javascript
async function getUser(req, res) {
  const user = await userService.findById(req.params.id);
  if (!user) {
    return res.status(404).json({ error: "Not found" });
  }
  res.status(200).json(user);
}
```

**Explanation:**

- **ActionResult** = Wrapper that can return different HTTP status codes
- NestJS uses exceptions for error status codes
- Express requires explicit `res.status().json()`

### 11. Claims vs User Object

**ASP.NET Core:**

```csharp
// JWT Claims stored in User property
var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
var role = User.FindFirst(ClaimTypes.Role)?.Value;
var email = User.FindFirst(ClaimTypes.Email)?.Value;
```

**NestJS Equivalent:**

```typescript
// User object attached to request
@Req() request: Request
const userId = request.user.id;
const role = request.user.role;
const email = request.user.email;
```

**Node.js/Express Equivalent:**

```javascript
// User attached to request by middleware
function getUser(req, res) {
  const userId = req.user.id;
  const role = req.user.role;
  const email = req.user.email;
}
```

**Explanation:**

- **Claims** = Key-value pairs in JWT token (like properties in an object)
- ASP.NET Core uses `ClaimTypes` for standard claim names
- NestJS/Node.js attach decoded JWT as `user` object to request

### 12. BSON Attributes (MongoDB Mapping)

**ASP.NET Core:**

```csharp
[BsonId]  // Marks as primary key
[BsonRepresentation(BsonType.ObjectId)]  // Converts string to ObjectId
[BsonElement("email")]  // Maps property to MongoDB field name
public string Email { get; set; }
```

**NestJS/Mongoose Equivalent:**

```typescript
@Prop({ required: true })
email: string;  // Automatically maps to 'email' field

@Prop({ type: mongoose.Schema.Types.ObjectId })
_id: ObjectId;  // Primary key
```

**JavaScript/Mongoose Equivalent:**

```javascript
const userSchema = new mongoose.Schema({
  email: { type: String, required: true },
});
```

**Explanation:**

- **BSON** = Binary JSON (MongoDB's data format)
- **BsonId** = Marks the primary key field
- **BsonElement** = Maps C# property name to MongoDB field name
- Mongoose automatically handles field mapping

### 13. Data Annotations (Validation)

**ASP.NET Core:**

```csharp
[Required]                    // Field is required
[EmailAddress]                // Must be valid email
[MinLength(6)]                // Minimum 6 characters
[MaxLength(100)]              // Maximum 100 characters
[Range(1, 100)]               // Value between 1 and 100
public string Email { get; set; }
```

**NestJS Equivalent:**

```typescript
@IsEmail()                    // Must be valid email
@IsNotEmpty()                 // Field is required
@MinLength(6)                 // Minimum 6 characters
@MaxLength(100)               // Maximum 100 characters
@IsInt()
@Min(1)
@Max(100)                     // Value between 1 and 100
email: string;
```

**JavaScript Equivalent:**

```javascript
// Manual validation
function validateEmail(email) {
  if (!email) throw new Error("Email is required");
  if (!email.includes("@")) throw new Error("Invalid email");
  if (email.length < 6) throw new Error("Email too short");
}
```

**Explanation:**

- **Data Annotations** = Attributes that add validation rules
- Both ASP.NET Core and NestJS provide automatic validation
- JavaScript requires manual validation or libraries like `joi` or `yup`

### 14. Task vs Promise

**ASP.NET Core:**

```csharp
public async Task<User> GetUserAsync(string id)  // Task = async operation
{
    return await _repository.GetByIdAsync(id);
}

public Task<List<User>> GetAllUsersAsync()  // Can return Task without async
{
    return _repository.GetAllAsync();
}
```

**NestJS/TypeScript Equivalent:**

```typescript
async getUser(id: string): Promise<User> {  // Promise = async operation
  return await this.repository.findById(id);
}

getAllUsers(): Promise<User[]> {  // Can return Promise without async
  return this.repository.findAll();
}
```

**JavaScript Equivalent:**

```javascript
async function getUser(id) {
  // Returns Promise automatically
  return await repository.findById(id);
}

function getAllUsers() {
  // Returns Promise
  return repository.findAll();
}
```

**Explanation:**

- **Task<T>** in C# = **Promise<T>** in TypeScript/JavaScript
- Both represent asynchronous operations
- `async/await` works the same in all three languages

### 15. Extension Methods

**ASP.NET Core:**

```csharp
// Extension method (adds method to existing type)
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCustomServices(this IServiceCollection services)
    {
        services.AddScoped<IService, Service>();
        return services;
    }
}

// Usage
builder.Services.AddCustomServices();
```

**NestJS Equivalent:**

```typescript
// No direct equivalent - use helper functions
function addCustomServices(module: Module) {
  module.providers.push(Service);
  return module;
}
```

**JavaScript Equivalent:**

```javascript
// No extension methods - use helper functions
function addCustomServices(app) {
  app.use(service);
  return app;
}
```

**Explanation:**

- **Extension Methods** = Add methods to existing types without modifying them
- C# feature - not available in JavaScript/TypeScript
- Use helper functions or mixins instead

### 16. Lambda Expressions

**ASP.NET Core:**

```csharp
// Lambda expression (arrow function)
var user = users.Where(u => u.IsActive).ToList();
var userIds = users.Select(u => u.Id).ToList();

// LINQ with lambda
_users.Find(u => u.Email == email).FirstOrDefaultAsync();
```

**NestJS/TypeScript Equivalent:**

```typescript
// Arrow function (same syntax)
const user = users.filter((u) => u.isActive);
const userIds = users.map((u) => u.id);
```

**JavaScript Equivalent:**

```javascript
// Arrow function (same syntax)
const user = users.filter((u) => u.isActive);
const userIds = users.map((u) => u.id);
```

**Explanation:**

- **Lambda** = Anonymous function (arrow function)
- Same syntax in C#, TypeScript, and JavaScript: `x => x.property`
- Used for filtering, mapping, and querying

### 17. Null Coalescing Operator

**ASP.NET Core:**

```csharp
var name = user?.Name ?? "Unknown";  // ?? = if null, use default
var email = user?.Email ?? string.Empty;
```

**NestJS/TypeScript Equivalent:**

```typescript
const name = user?.name ?? "Unknown"; // ?? = if null/undefined, use default
const email = user?.email ?? "";
```

**JavaScript Equivalent:**

```javascript
// Nullish coalescing (ES2020)
const name = user?.name ?? "Unknown";
const email = user?.email ?? "";

// Older way
const name = user && user.name ? user.name : "Unknown";
```

**Explanation:**

- **??** = Nullish coalescing operator (returns right side if left is null/undefined)
- **?.** = Optional chaining (safe property access)
- Available in modern JavaScript (ES2020) and TypeScript

### 18. String Interpolation

**ASP.NET Core:**

```csharp
var message = $"User {userId} not found";  // $ = string interpolation
var error = $"Error: {exception.Message}";
```

**NestJS/TypeScript Equivalent:**

```typescript
const message = `User ${userId} not found`; // Template literals
const error = `Error: ${exception.message}`;
```

**JavaScript Equivalent:**

```javascript
// Template literals (ES6)
const message = `User ${userId} not found`;
const error = `Error: ${exception.message}`;

// Older way
const message = "User " + userId + " not found";
```

**Explanation:**

- **String Interpolation** = Embedding variables in strings
- C# uses `$"text {variable}"`
- JavaScript/TypeScript use template literals: `` `text ${variable}` ``

### 19. Collection Types

**ASP.NET Core:**

```csharp
List<string> userIds = new List<string>();  // Mutable list
IEnumerable<string> names;                   // Read-only enumeration
Dictionary<string, User> users;             // Key-value pairs
```

**NestJS/TypeScript Equivalent:**

```typescript
const userIds: string[] = []; // Array (mutable)
const names: ReadonlyArray<string>; // Read-only array
const users: Record<string, User>; // Key-value pairs (object)
```

**JavaScript Equivalent:**

```javascript
const userIds = []; // Array
const users = {}; // Object (key-value pairs)
```

**Explanation:**

- **List<T>** in C# = **Array** in JavaScript/TypeScript
- **Dictionary<K,V>** in C# = **Object/Record** in JavaScript/TypeScript
- All are mutable by default

### 20. Exception Handling

**ASP.NET Core:**

```csharp
try
{
    var result = await _service.DoSomethingAsync();
    return Ok(result);
}
catch (Exception ex)  // Catches all exceptions
{
    return BadRequest(new { message = ex.Message });
}
```

**NestJS Equivalent:**

```typescript
try {
  const result = await this.service.doSomething();
  return result;
} catch (error) {
  // Catches all errors
  throw new BadRequestException(error.message);
}
```

**JavaScript Equivalent:**

```javascript
try {
  const result = await service.doSomething();
  return result;
} catch (error) {
  // Catches all errors
  throw new Error(error.message);
}
```

**Explanation:**

- **Exception** in C# = **Error** in JavaScript
- All three use `try/catch` blocks
- NestJS has specific exception classes (NotFoundException, BadRequestException, etc.)

---

## Summary of All Terminologies

### Quick Reference Table

| ASP.NET Core Term     | NestJS Equivalent   | Node.js/JavaScript Equivalent | Description               |
| --------------------- | ------------------- | ----------------------------- | ------------------------- |
| `[Attribute]`         | `@Decorator()`      | Function/Middleware           | Metadata annotations      |
| `Task<T>`             | `Promise<T>`        | `Promise`                     | Asynchronous operation    |
| `async/await`         | `async/await`       | `async/await`                 | Async syntax (same)       |
| `List<T>`             | `Array<T>`          | `Array`                       | Collection type           |
| `Dictionary<K,V>`     | `Record<K,V>`       | `Object`                      | Key-value pairs           |
| `string?`             | `string \| null`    | `string \| null`              | Nullable type             |
| `??`                  | `??`                | `??`                          | Nullish coalescing        |
| `?.`                  | `?.`                | `?.`                          | Optional chaining         |
| `$"text {var}"`       | `` `text ${var}` `` | `` `text ${var}` ``           | String interpolation      |
| `interface`           | `interface`         | N/A (use TypeScript)          | Contract definition       |
| `class`               | `class`             | `class`                       | Class definition (same)   |
| `try/catch`           | `try/catch`         | `try/catch`                   | Exception handling (same) |
| `Exception`           | `Error`             | `Error`                       | Error object              |
| `AddScoped`           | `Scope.REQUEST`     | Manual management             | One per request           |
| `AddSingleton`        | `Scope.DEFAULT`     | Single instance               | One for app lifetime      |
| `[Required]`          | `@IsNotEmpty()`     | Manual validation             | Validation attribute      |
| `[FromBody]`          | `@Body()`           | `req.body`                    | Request body binding      |
| `ActionResult<T>`     | `Promise<T>`        | `res.json()`                  | HTTP response             |
| `User.Claims`         | `request.user`      | `req.user`                    | Authenticated user data   |
| `IMongoCollection<T>` | `Model<T>`          | `Model`                       | MongoDB collection/model  |
| `[BsonId]`            | `@Prop()`           | Schema definition             | MongoDB field mapping     |

### Key Concepts Explained

1. **Attributes/Decorators**: Both C# and TypeScript use metadata to add behavior to code
2. **Dependency Injection**: Automatic dependency management (built-in in ASP.NET Core and NestJS)
3. **Async/Await**: Same syntax across all three languages
4. **Type System**: C# and TypeScript are statically typed; JavaScript is dynamically typed
5. **Collections**: Different names but similar concepts (List = Array, Dictionary = Object)
6. **Null Safety**: C# and TypeScript have nullable types; JavaScript treats all as nullable
7. **Validation**: Both frameworks provide automatic validation via attributes/decorators
8. **Middleware**: Request processing pipeline (same concept, different implementation)

---

## Controllers Deep Dive

### AuthController

**Purpose:** Handles user authentication (signup, signin, admin signup)

```csharp
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
```

**NestJS Equivalent:**

```typescript
@Controller("api/auth")
export class AuthController {}
```

**Key Points:**

- `[ApiController]` enables automatic model validation and API-specific behaviors
- `[Route("api/[controller]")]` creates route `api/Auth` (controller name without "Controller")
- `ControllerBase` provides helper methods like `Ok()`, `BadRequest()`, etc.

#### Signup Endpoint

```csharp
[HttpPost("signup")]
public async Task<ActionResult<AuthResponse>> Signup([FromBody] SignupRequest request)
```

**NestJS Equivalent:**

```typescript
@Post('signup')
async signup(@Body() signupDto: SignupDto): Promise<AuthResponse> {}
```

**Breakdown:**

- `[HttpPost("signup")]` = `@Post('signup')`
- `[FromBody]` = `@Body()` - extracts JSON from request body
- `Task<ActionResult<AuthResponse>>` = `Promise<AuthResponse>`
- `Ok(new AuthResponse { ... })` = `return { ... }` (NestJS auto-wraps in 200)

#### Signin Endpoint

Similar to signup, but returns a JWT token.

#### SignupAdmin Endpoint

```csharp
[HttpPost("signup-admin")]
[Authorize(Roles = "ADMIN")]
public async Task<ActionResult<AuthResponse>> SignupAdmin(...)
```

**NestJS Equivalent:**

```typescript
@Post('signup-admin')
@UseGuards(AuthGuard)
@Roles('ADMIN')
async signupAdmin(...) {}
```

**Key Difference:**

- `[Authorize(Roles = "ADMIN")]` combines guard and role check
- NestJS uses separate decorators: `@UseGuards()` + `@Roles()`

### CourseController

**Purpose:** CRUD operations for courses

```csharp
[Authorize]  // Applied to entire controller
public class CourseController : ControllerBase
```

**NestJS Equivalent:**

```typescript
@UseGuards(AuthGuard)
export class CourseController {}
```

**Endpoints:**

- `GET /api/courses` - Get all courses
- `GET /api/courses/{id}` - Get course by ID
- `POST /api/courses` - Create course
- `PUT /api/courses/{id}` - Update course
- `DELETE /api/courses/{id}` - Delete course

**Key Pattern:**

```csharp
[HttpGet("{id}")]
public async Task<ActionResult<Course>> GetCourseById(string id)
{
    var course = await _courseService.GetCourseByIdAsync(id);
    if (course == null)
    {
        return NotFound();  // Returns 404
    }
    return Ok(course);  // Returns 200 with data
}
```

**NestJS Equivalent:**

```typescript
@Get(':id')
async getCourseById(@Param('id') id: string): Promise<Course> {
  const course = await this.courseService.findById(id);
  if (!course) {
    throw new NotFoundException();
  }
  return course;
}
```

### DiscussionController

**Purpose:** Manages discussions within courses

**Key Feature - User ID Extraction:**

```csharp
var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
    ?? User.FindFirst("nameid")?.Value;
```

**NestJS Equivalent:**

```typescript
@Req() request: Request
const userId = request.user.id;
```

**Explanation:**

- After JWT authentication, user claims are available in `User` property
- `ClaimTypes.NameIdentifier` is the standard claim type for user ID
- `"nameid"` is the JWT short form (MongoDB ObjectId is stored here)

### PostController

**Purpose:** Manages posts within discussions

**Key Pattern - Authorization Check:**

```csharp
if (post.UserId != userId)
{
    throw new Exception("You can only update your own posts");
}
```

**NestJS Equivalent:**

```typescript
if (post.userId !== userId) {
  throw new ForbiddenException("You can only update your own posts");
}
```

### EnrollmentController

**Purpose:** Manages course enrollments

**Key Feature - Complex Business Logic:**

```csharp
foreach (var userId in userIds)
{
    var user = await _userRepository.GetByIdAsync(userId);
    if (user == null)
    {
        throw new Exception("User not found with id: " + userId);
    }
    // ... enrollment logic
}
```

**NestJS Equivalent:**

```typescript
for (const userId of userIds) {
  const user = await this.userRepository.findById(userId);
  if (!user) {
    throw new NotFoundException(`User not found with id: ${userId}`);
  }
  // ... enrollment logic
}
```

---

## Services Deep Dive

### AuthService

**Purpose:** Handles authentication business logic

```csharp
public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtService _jwtService;

    public AuthService(IUserRepository userRepository, IJwtService jwtService)
    {
        _userRepository = userRepository;
        _jwtService = jwtService;
    }
}
```

**NestJS Equivalent:**

```typescript
@Injectable()
export class AuthService {
  constructor(
    private readonly userRepository: UserRepository,
    private readonly jwtService: JwtService
  ) {}
}
```

**Key Methods:**

#### SignupAsync

```csharp
public async Task<User> SignupAsync(string email, string password, ...)
{
    var existingUser = await _userRepository.GetByEmailAsync(email);
    if (existingUser != null)
    {
        throw new Exception("User with this email already exists");
    }

    var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
    // ... create user
}
```

**NestJS Equivalent:**

```typescript
async signup(email: string, password: string, ...): Promise<User> {
  const existingUser = await this.userRepository.findByEmail(email);
  if (existingUser) {
    throw new ConflictException('User with this email already exists');
  }

  const hashedPassword = await bcrypt.hash(password, 10);
  // ... create user
}
```

**Key Differences:**

- ASP.NET Core uses `BCrypt.Net.BCrypt.HashPassword()` (synchronous)
- NestJS uses `bcrypt.hash()` (asynchronous)
- ASP.NET Core throws generic `Exception`, NestJS uses specific exceptions

#### SigninAsync

```csharp
if (!BCrypt.Net.BCrypt.Verify(password, user.Password))
{
    throw new Exception("Invalid email or password");
}
return _jwtService.GenerateToken(user.Id!, user.Email, user.Role);
```

**NestJS Equivalent:**

```typescript
const isPasswordValid = await bcrypt.compare(password, user.password);
if (!isPasswordValid) {
  throw new UnauthorizedException("Invalid email or password");
}
return this.jwtService.sign({
  sub: user.id,
  email: user.email,
  role: user.role,
});
```

### CourseService

**Purpose:** Business logic for course management

**Pattern - Simple CRUD:**

```csharp
public async Task<Course> CreateCourseAsync(Course course)
{
    course.CreatedAt = DateTime.UtcNow;
    course.UpdatedAt = DateTime.UtcNow;
    return await _courseRepository.CreateAsync(course);
}
```

**NestJS Equivalent:**

```typescript
async createCourse(course: Course): Promise<Course> {
  course.createdAt = new Date();
  course.updatedAt = new Date();
  return await this.courseRepository.create(course);
}
```

### PostService

**Purpose:** Business logic for posts with authorization checks

**Key Pattern - Ownership Validation:**

```csharp
public async Task<Post> UpdatePostAsync(string id, Post postDetails, string userId)
{
    var post = await _postRepository.GetByIdAsync(id);
    if (post == null)
    {
        throw new Exception("Post not found with id: " + id);
    }

    if (post.UserId != userId)
    {
        throw new Exception("You can only update your own posts");
    }

    post.Content = postDetails.Content;
    post.UpdatedAt = DateTime.UtcNow;
    return await _postRepository.UpdateAsync(post);
}
```

**NestJS Equivalent:**

```typescript
async updatePost(id: string, postDetails: UpdatePostDto, userId: string): Promise<Post> {
  const post = await this.postRepository.findById(id);
  if (!post) {
    throw new NotFoundException(`Post not found with id: ${id}`);
  }

  if (post.userId !== userId) {
    throw new ForbiddenException('You can only update your own posts');
  }

  post.content = postDetails.content;
  post.updatedAt = new Date();
  return await this.postRepository.update(id, post);
}
```

### DiscussionService

Similar to PostService, includes course validation before creating discussions.

### EnrollmentService

**Purpose:** Manages course enrollments with complex business logic

**Key Pattern - Batch Operations:**

```csharp
public async Task<List<CourseEnrollment>> EnrollUsersAsync(
    List<string> userIds,
    string courseId,
    string grantedBy)
{
    var course = await _courseRepository.GetByIdAsync(courseId);
    if (course == null)
    {
        throw new Exception("Course not found with id: " + courseId);
    }

    var enrollments = new List<CourseEnrollment>();
    foreach (var userId in userIds)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            throw new Exception("User not found with id: " + userId);
        }

        var existingEnrollment = await _enrollmentRepository
            .GetByCourseIdAndUserIdAsync(courseId, userId);
        if (existingEnrollment != null)
        {
            continue; // Skip if already enrolled
        }

        // Create enrollment
    }

    return enrollments;
}
```

**NestJS Equivalent:**

```typescript
async enrollUsers(userIds: string[], courseId: string, grantedBy: string): Promise<CourseEnrollment[]> {
  const course = await this.courseRepository.findById(courseId);
  if (!course) {
    throw new NotFoundException(`Course not found with id: ${courseId}`);
  }

  const enrollments = [];
  for (const userId of userIds) {
    const user = await this.userRepository.findById(userId);
    if (!user) {
      throw new NotFoundException(`User not found with id: ${userId}`);
    }

    const existingEnrollment = await this.enrollmentRepository
      .findByCourseAndUser(courseId, userId);
    if (existingEnrollment) {
      continue;
    }

    // Create enrollment
  }

  return enrollments;
}
```

### JwtService

**Purpose:** Handles JWT token generation and parsing

```csharp
public string GenerateToken(string userId, string email, string role)
{
    var claims = new[]
    {
        new Claim(ClaimTypes.NameIdentifier, userId),
        new Claim(ClaimTypes.Email, email),
        new Claim(ClaimTypes.Role, role),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
    };

    var tokenDescriptor = new SecurityTokenDescriptor
    {
        Subject = new ClaimsIdentity(claims),
        Expires = DateTime.UtcNow.AddMinutes(expirationMinutes),
        Issuer = issuer,
        Audience = audience,
        SigningCredentials = new SigningCredentials(
            new SymmetricSecurityKey(key),
            SecurityAlgorithms.HmacSha256Signature)
    };

    var tokenHandler = new JwtSecurityTokenHandler();
    var token = tokenHandler.CreateToken(tokenDescriptor);
    return tokenHandler.WriteToken(token);
}
```

**NestJS Equivalent:**

```typescript
sign(payload: { sub: string; email: string; role: string }): string {
  return this.jwtService.sign(payload, {
    secret: this.configService.get('JWT_SECRET'),
    expiresIn: '60m',
    issuer: 'CourseManagementAPI',
    audience: 'CourseManagementAPI',
  });
}
```

**Key Differences:**

- ASP.NET Core uses `ClaimTypes` for standard claim types
- NestJS uses simple object with `sub` (subject) for user ID
- ASP.NET Core explicitly creates `SecurityTokenDescriptor`, NestJS uses options object

---

## Repositories Deep Dive

### Repository Pattern

Repositories abstract data access logic, similar to Mongoose models in NestJS.

### UserRepository

```csharp
public class UserRepository : IUserRepository
{
    private readonly IMongoCollection<User> _users;

    public UserRepository(IMongoDatabase database)
    {
        _users = database.GetCollection<User>("users");
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _users.Find(u => u.Email == email).FirstOrDefaultAsync();
    }
}
```

**NestJS Equivalent:**

```typescript
@Injectable()
export class UserRepository {
  constructor(@InjectModel(User.name) private userModel: Model<UserDocument>) {}

  async findByEmail(email: string): Promise<User | null> {
    return await this.userModel.findOne({ email }).exec();
  }
}
```

**Key Differences:**

- ASP.NET Core uses `IMongoCollection<T>` directly
- NestJS uses Mongoose `Model<T>` with `@InjectModel()` decorator
- ASP.NET Core uses LINQ-style queries: `.Find().FirstOrDefaultAsync()`
- NestJS uses Mongoose methods: `.findOne().exec()`

### MongoDB Operations Comparison

| ASP.NET Core                                               | NestJS (Mongoose)                       |
| ---------------------------------------------------------- | --------------------------------------- |
| `_users.Find(u => u.Email == email).FirstOrDefaultAsync()` | `userModel.findOne({ email })`          |
| `_users.Find(_ => true).ToListAsync()`                     | `userModel.find({})`                    |
| `_users.InsertOneAsync(user)`                              | `userModel.create(user)`                |
| `_users.ReplaceOneAsync(u => u.Id == id, user)`            | `userModel.findByIdAndUpdate(id, user)` |
| `_users.DeleteOneAsync(u => u.Id == id)`                   | `userModel.findByIdAndDelete(id)`       |

---

## Models and DTOs

### Models (Entities)

Models represent database documents, similar to Mongoose schemas.

```csharp
public class User
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("email")]
    public string Email { get; set; } = string.Empty;

    [BsonElement("role")]
    public string Role { get; set; } = "USER";
}
```

**NestJS Equivalent:**

```typescript
@Schema()
export class User {
  @Prop({ type: mongoose.Schema.Types.ObjectId })
  _id: string;

  @Prop({ required: true, unique: true })
  email: string;

  @Prop({ default: "USER" })
  role: string;
}
```

**Key Attributes:**

- `[BsonId]` = Marks the primary key field
- `[BsonRepresentation(BsonType.ObjectId)]` = Converts string to MongoDB ObjectId
- `[BsonElement("email")]` = Maps C# property to MongoDB field name
- `= string.Empty` = Default value (empty string)
- `?` = Nullable type (can be null)

### DTOs (Data Transfer Objects)

DTOs define request/response shapes, similar to DTOs in NestJS.

```csharp
public class SignupRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MinLength(6)]
    public string Password { get; set; } = string.Empty;

    [Required]
    public string FirstName { get; set; } = string.Empty;
}
```

**NestJS Equivalent:**

```typescript
export class SignupDto {
  @IsEmail()
  @IsNotEmpty()
  email: string;

  @IsString()
  @MinLength(6)
  @IsNotEmpty()
  password: string;

  @IsString()
  @IsNotEmpty()
  firstName: string;
}
```

**Key Differences:**

- ASP.NET Core uses `[Required]`, `[EmailAddress]`, `[MinLength(6)]`
- NestJS uses `@IsEmail()`, `@IsNotEmpty()`, `@MinLength(6)`
- Both provide automatic validation

---

## Middleware

### ErrorHandlingMiddleware

Middleware in ASP.NET Core is similar to NestJS interceptors or exception filters.

```csharp
public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred");
            await HandleExceptionAsync(context, ex);
        }
    }
}
```

**NestJS Equivalent:**

```typescript
@Catch()
export class HttpExceptionFilter implements ExceptionFilter {
  catch(exception: unknown, host: ArgumentsHost) {
    const ctx = host.switchToHttp();
    const response = ctx.getResponse();
    const request = ctx.getRequest();

    const status =
      exception instanceof HttpException
        ? exception.getStatus()
        : HttpStatus.INTERNAL_SERVER_ERROR;

    response.status(status).json({
      message: exception.message,
      error: "Internal Server Error",
    });
  }
}
```

**Key Differences:**

- ASP.NET Core middleware uses `RequestDelegate` (next function)
- NestJS uses `ExceptionFilter` interface
- Both catch exceptions and format responses

**Registration:**

```csharp
app.UseMiddleware<ErrorHandlingMiddleware>();
```

**NestJS Equivalent:**

```typescript
app.useGlobalFilters(new HttpExceptionFilter());
```

---

## Configuration

### appsettings.json

Similar to `.env` or `config/` files in NestJS.

```json
{
  "ConnectionStrings": {
    "MongoDB": "mongodb://localhost:27017"
  },
  "MongoDB": {
    "DatabaseName": "course_management"
  },
  "Jwt": {
    "Key": "YourSuperSecretKey...",
    "Issuer": "CourseManagementAPI",
    "Audience": "CourseManagementAPI",
    "ExpirationMinutes": 60
  }
}
```

**Accessing Configuration:**

```csharp
var connectionString = builder.Configuration.GetConnectionString("MongoDB");
var jwtKey = builder.Configuration["Jwt:Key"];
```

**NestJS Equivalent:**

```typescript
// Using ConfigService
constructor(private configService: ConfigService) {}

const connectionString = this.configService.get<string>('MONGODB_URI');
const jwtKey = this.configService.get<string>('JWT_SECRET');
```

---

## Dependency Injection

### Service Lifetime

| ASP.NET Core   | NestJS                                    | Description                   |
| -------------- | ----------------------------------------- | ----------------------------- |
| `AddSingleton` | `@Injectable({ scope: Scope.DEFAULT })`   | One instance for app lifetime |
| `AddScoped`    | `@Injectable({ scope: Scope.REQUEST })`   | One instance per HTTP request |
| `AddTransient` | `@Injectable({ scope: Scope.TRANSIENT })` | New instance every time       |

**Most Common:** `AddScoped` (one per request) - This is the default in NestJS.

### Registration

**ASP.NET Core:**

```csharp
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
```

**NestJS Equivalent:**

```typescript
@Module({
  providers: [
    UserRepository,
    AuthService,
  ],
})
```

**Key Difference:**

- ASP.NET Core explicitly maps interface to implementation
- NestJS uses class names directly (TypeScript handles interfaces)

---

## Authentication & Authorization

### JWT Configuration

**ASP.NET Core:**

```csharp
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
    };
});
```

**NestJS Equivalent:**

```typescript
JwtModule.register({
  secret: jwtConstants.secret,
  signOptions: { expiresIn: '60m' },
}),
```

### Using Authorization

**ASP.NET Core:**

```csharp
[Authorize]  // Requires authentication
[Authorize(Roles = "ADMIN")]  // Requires ADMIN role
```

**NestJS Equivalent:**

```typescript
@UseGuards(AuthGuard)  // Requires authentication
@Roles('ADMIN')  // Requires ADMIN role
@UseGuards(AuthGuard, RolesGuard)
```

### Accessing User Claims

**ASP.NET Core:**

```csharp
var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
var role = User.FindFirst(ClaimTypes.Role)?.Value;
```

**NestJS Equivalent:**

```typescript
@Req() request: Request
const userId = request.user.id;
const role = request.user.role;
```

---

## Common Patterns & Best Practices

### 1. Error Handling

**ASP.NET Core:**

```csharp
try
{
    var result = await _service.DoSomethingAsync();
    return Ok(result);
}
catch (Exception ex)
{
    if (ex.Message.Contains("not found"))
    {
        return NotFound();
    }
    return BadRequest(new { message = ex.Message });
}
```

**NestJS Equivalent:**

```typescript
try {
  const result = await this.service.doSomething();
  return result;
} catch (error) {
  if (error.message.includes("not found")) {
    throw new NotFoundException();
  }
  throw new BadRequestException(error.message);
}
```

### 2. Null Checking

**ASP.NET Core:**

```csharp
var user = await _repository.GetByIdAsync(id);
if (user == null)
{
    return NotFound();
}
```

**NestJS Equivalent:**

```typescript
const user = await this.repository.findById(id);
if (!user) {
  throw new NotFoundException();
}
```

### 3. Async/Await Pattern

Both frameworks use the same pattern:

- Always use `async`/`await`
- Return `Task<T>` in C# or `Promise<T>` in TypeScript
- Use `async` methods for I/O operations

### 4. Dependency Injection Best Practices

- **Always use interfaces** for repositories and services
- **Inject dependencies via constructor**
- **Use `AddScoped` for most services** (one per request)
- **Use `AddSingleton` for stateless services** (like JWT service)

---

## Summary

### Key Takeaways for NestJS Developers

1. **Controllers** = Controllers (same concept)
2. **Services** = Services (same concept)
3. **Repositories** = Mongoose models or repositories
4. **Middleware** = Interceptors/Exception Filters
5. **Attributes** = Decorators
6. **Dependency Injection** = Same concept, different syntax
7. **Async/Await** = Same pattern
8. **Configuration** = appsettings.json vs .env

### Migration Checklist

When moving from NestJS to ASP.NET Core:

- [ ] Convert `@Controller()` to `[ApiController]` + `[Route()]`
- [ ] Convert `@Injectable()` to service registration in `Program.cs`
- [ ] Convert `@Get()`, `@Post()` to `[HttpGet]`, `[HttpPost]`
- [ ] Convert `@Body()` to `[FromBody]`
- [ ] Convert `@UseGuards()` to `[Authorize]`
- [ ] Convert Mongoose models to MongoDB.Driver collections
- [ ] Convert `class-validator` to Data Annotations
- [ ] Convert exception filters to middleware or try-catch
- [ ] Convert ConfigService to `IConfiguration`

---

## Additional Resources

- [ASP.NET Core Documentation](https://docs.microsoft.com/en-us/aspnet/core/)
- [MongoDB.Driver Documentation](https://www.mongodb.com/docs/drivers/csharp/)
- [JWT Authentication in ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/security/authentication/jwt-authn)

---

**Happy Coding! 🚀**
