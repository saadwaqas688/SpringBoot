# Manual Dependency Injection Example (Without Spring)

This document shows how you'd do dependency injection **manually** without Spring, and compares it to Spring's automatic approach.

---

## Complete Example: Authentication System

### Scenario:

- `JwtUtil` - Utility class for JWT operations
- `UserRepository` - Database access
- `AuthService` - Business logic (needs JwtUtil and UserRepository)
- `AuthController` - HTTP handler (needs AuthService)

---

## Manual Dependency Injection (Without Spring)

### Step 1: Create the Classes (No Annotations)

```java
// JwtUtil.java - No @Component annotation
public class JwtUtil {
    private String secret;

    public JwtUtil(String secret) {
        this.secret = secret;
    }

    public String generateToken(String userId) {
        // JWT generation logic
        return "token-" + userId;
    }
}

// UserRepository.java - No @Repository annotation
public class UserRepository {
    // Database connection would be here
    private String connectionString;

    public UserRepository(String connectionString) {
        this.connectionString = connectionString;
    }

    public User findByEmail(String email) {
        // Database query logic
        return new User(email);
    }
}

// AuthService.java - No @Service annotation
public class AuthService {
    // Dependencies as fields
    private UserRepository userRepository;
    private JwtUtil jwtUtil;

    // Manual dependency injection via constructor
    public AuthService(UserRepository userRepository, JwtUtil jwtUtil) {
        // YOU manually pass dependencies here!
        this.userRepository = userRepository;
        this.jwtUtil = jwtUtil;
    }

    public AuthResponse signup(String email, String password) {
        User user = userRepository.findByEmail(email);
        String token = jwtUtil.generateToken(user.getId());
        return new AuthResponse(token);
    }
}

// AuthController.java - No @RestController annotation
public class AuthController {
    // Dependency as field
    private AuthService authService;

    // Manual dependency injection via constructor
    public AuthController(AuthService authService) {
        // YOU manually pass dependencies here!
        this.authService = authService;
    }

    public AuthResponse signup(String email, String password) {
        return authService.signup(email, password);
    }
}
```

### Step 2: Manual Object Creation and Wiring (The Hard Part!)

```java
// Application.java - Main class that does ALL the manual work
public class Application {

    public static void main(String[] args) {
        // ============================================================
        // MANUAL DEPENDENCY INJECTION - YOU DO EVERYTHING!
        // ============================================================

        // Step 1: Create dependencies (bottom of the chain)
        String jwtSecret = "my-secret-key";
        JwtUtil jwtUtil = new JwtUtil(jwtSecret);  // ← Manual creation

        String dbConnection = "mongodb://localhost:27017/tododb";
        UserRepository userRepository = new UserRepository(dbConnection);  // ← Manual creation

        // Step 2: Create service with dependencies
        AuthService authService = new AuthService(userRepository, jwtUtil);  // ← Manual injection

        // Step 3: Create controller with service
        AuthController authController = new AuthController(authService);  // ← Manual injection

        // Step 4: Now you can use it
        AuthResponse response = authController.signup("user@example.com", "password123");

        System.out.println("Token: " + response.getToken());
    }
}
```

---

## Complete Manual Example (More Realistic)

### Full Manual Implementation:

```java
// ============================================================
// MANUAL DEPENDENCY INJECTION - COMPLETE EXAMPLE
// ============================================================

public class ManualDependencyInjectionExample {

    public static void main(String[] args) {
        // ============================================================
        // STEP 1: Create all dependencies manually
        // ============================================================

        // Create JwtUtil (no dependencies)
        String jwtSecret = System.getenv("JWT_SECRET");
        if (jwtSecret == null) {
            jwtSecret = "default-secret-key";
        }
        JwtUtil jwtUtil = new JwtUtil(jwtSecret);

        // Create UserRepository (no dependencies)
        String dbUrl = System.getenv("DB_URL");
        if (dbUrl == null) {
            dbUrl = "mongodb://localhost:27017/tododb";
        }
        UserRepository userRepository = new UserRepository(dbUrl);

        // Create PasswordEncoder (no dependencies)
        PasswordEncoder passwordEncoder = new BCryptPasswordEncoder();

        // ============================================================
        // STEP 2: Wire dependencies together manually
        // ============================================================

        // Create AuthService with all its dependencies
        AuthService authService = new AuthService(
            userRepository,      // ← Manual injection
            passwordEncoder,      // ← Manual injection
            jwtUtil              // ← Manual injection
        );

        // ============================================================
        // STEP 3: Create controller with service
        // ============================================================

        AuthController authController = new AuthController(authService);  // ← Manual injection

        // ============================================================
        // STEP 4: Create HTTP server and register routes manually
        // ============================================================

        // You'd need to manually set up HTTP server (like Jetty, Tomcat, etc.)
        HttpServer server = HttpServer.create(new InetSocketAddress(8080), 0);

        // Manually register routes
        server.createContext("/api/auth/signup", (exchange) -> {
            // Parse request body manually
            String requestBody = readRequestBody(exchange);
            SignupRequest request = parseJson(requestBody);

            // Call controller manually
            AuthResponse response = authController.signup(request);

            // Send response manually
            sendResponse(exchange, response);
        });

        server.start();
        System.out.println("Server started on port 8080");
    }
}
```

---

## Node.js Equivalent (Manual Dependency Injection)

### Complete Manual Example in Node.js:

```javascript
// ============================================================
// MANUAL DEPENDENCY INJECTION IN NODE.JS
// ============================================================

// jwtUtil.js
class JwtUtil {
  constructor(secret) {
    this.secret = secret;
  }

  generateToken(userId) {
    return `token-${userId}`;
  }
}

// userRepository.js
class UserRepository {
  constructor(connectionString) {
    this.connectionString = connectionString;
  }

  async findByEmail(email) {
    // Database query
    return { id: "123", email: email };
  }
}

// authService.js
class AuthService {
  constructor(userRepository, jwtUtil) {
    // Manual dependency injection - you pass them in!
    this.userRepository = userRepository;
    this.jwtUtil = jwtUtil;
  }

  async signup(email, password) {
    const user = await this.userRepository.findByEmail(email);
    const token = this.jwtUtil.generateToken(user.id);
    return { token };
  }
}

// authController.js
class AuthController {
  constructor(authService) {
    // Manual dependency injection - you pass it in!
    this.authService = authService;
  }

  async signup(req, res) {
    const response = await this.authService.signup(
      req.body.email,
      req.body.password
    );
    res.json(response);
  }
}

// ============================================================
// MANUAL WIRING - app.js
// ============================================================

const express = require("express");
const app = express();

// Step 1: Create all dependencies manually
const jwtSecret = process.env.JWT_SECRET || "default-secret";
const jwtUtil = new JwtUtil(jwtSecret); // ← Manual creation

const dbUrl = process.env.DB_URL || "mongodb://localhost:27017/tododb";
const userRepository = new UserRepository(dbUrl); // ← Manual creation

// Step 2: Create service with dependencies
const authService = new AuthService(userRepository, jwtUtil); // ← Manual injection

// Step 3: Create controller with service
const authController = new AuthController(authService); // ← Manual injection

// Step 4: Register routes manually
app.post("/api/auth/signup", (req, res) => {
  authController.signup(req, res);
});

// Step 5: Start server
app.listen(8080, () => {
  console.log("Server started on port 8080");
});
```

---

## Comparison: Manual vs Spring Boot

### Manual Dependency Injection (What You'd Do):

```java
// Manual - YOU do everything:
public class Application {
    public static void main(String[] args) {
        // 1. Create JwtUtil
        JwtUtil jwtUtil = new JwtUtil("secret");

        // 2. Create UserRepository
        UserRepository repo = new UserRepository("mongodb://...");

        // 3. Create PasswordEncoder
        PasswordEncoder encoder = new BCryptPasswordEncoder();

        // 4. Create AuthService with dependencies
        AuthService service = new AuthService(repo, encoder, jwtUtil);

        // 5. Create AuthController with service
        AuthController controller = new AuthController(service);

        // 6. Manually set up HTTP server
        // 7. Manually register routes
        // 8. Manually handle requests
    }
}
```

### Spring Boot (Automatic):

```java
// Spring Boot - Spring does everything automatically!

// JwtUtil.java
@Component  // ← Spring manages this
public class JwtUtil {
    @Value("${jwt.secret}")
    private String secret;
    // Spring creates instance automatically
}

// AuthService.java
@Service  // ← Spring manages this
public class AuthService {
    private final UserRepository userRepository;
    private final JwtUtil jwtUtil;

    @Autowired  // ← Spring injects dependencies automatically!
    public AuthService(UserRepository repo, JwtUtil jwt) {
        this.userRepository = repo;  // Spring provides this!
        this.jwtUtil = jwt;           // Spring provides this!
    }
}

// AuthController.java
@RestController  // ← Spring manages this
public class AuthController {
    private final AuthService authService;

    @Autowired  // ← Spring injects AuthService automatically!
    public AuthController(AuthService service) {
        this.authService = service;  // Spring provides this!
    }

    @PostMapping("/api/auth/signup")  // ← Spring registers route automatically!
    public ResponseEntity<AuthResponse> signup(@RequestBody SignupRequest request) {
        return authService.signup(request);
    }
}

// TodoMicroserviceApplication.java
@SpringBootApplication
public class TodoMicroserviceApplication {
    public static void main(String[] args) {
        // That's it! Spring does everything:
        // - Creates all beans
        // - Injects dependencies
        // - Sets up HTTP server
        // - Registers routes
        SpringApplication.run(TodoMicroserviceApplication.class, args);
    }
}
```

---

## Side-by-Side Comparison

| Task                      | Manual (Without Spring)                                          | Spring Boot (Automatic)                                       |
| ------------------------- | ---------------------------------------------------------------- | ------------------------------------------------------------- |
| **Create JwtUtil**        | `JwtUtil jwtUtil = new JwtUtil(secret);`                         | `@Component` - Spring creates it                              |
| **Create UserRepository** | `UserRepository repo = new UserRepository(url);`                 | `@Repository` - Spring creates it                             |
| **Create AuthService**    | `AuthService service = new AuthService(repo, encoder, jwtUtil);` | `@Service` + `@Autowired` - Spring creates and injects        |
| **Create AuthController** | `AuthController controller = new AuthController(service);`       | `@RestController` + `@Autowired` - Spring creates and injects |
| **Set up HTTP server**    | Manual setup (Jetty, Tomcat, etc.)                               | Automatic (embedded Tomcat)                                   |
| **Register routes**       | Manual route registration                                        | `@PostMapping` - Automatic                                    |
| **Handle requests**       | Manual request/response handling                                 | Automatic                                                     |
| **Configuration**         | Manual config reading                                            | `application.properties` - Automatic                          |

---

## Problems with Manual Dependency Injection

### 1. **You Have to Remember Everything**

```java
// What if you forget to create a dependency?
AuthService service = new AuthService(repo, encoder);  // Oops! Forgot jwtUtil!
// Runtime error!
```

### 2. **Order Matters**

```java
// Wrong order - jwtUtil doesn't exist yet!
AuthService service = new AuthService(repo, encoder, jwtUtil);  // Error!
JwtUtil jwtUtil = new JwtUtil(secret);
```

### 3. **Duplication**

```java
// If multiple controllers need AuthService, you create it multiple times
AuthService service1 = new AuthService(repo, encoder, jwtUtil);
AuthService service2 = new AuthService(repo, encoder, jwtUtil);  // Duplicate!
// Not singleton - wastes memory!
```

### 4. **Hard to Test**

```java
// Hard to mock dependencies in tests
AuthService service = new AuthService(realRepo, realEncoder, realJwtUtil);
// Can't easily swap with mocks
```

### 5. **Configuration Scattered**

```java
// Configuration is everywhere
String secret = System.getenv("JWT_SECRET");
String dbUrl = System.getenv("DB_URL");
// Hard to manage
```

---

## Benefits of Spring Boot Dependency Injection

### 1. **Automatic Creation**

```java
@Component
public class JwtUtil { }
// Spring creates it automatically - no 'new' keyword needed!
```

### 2. **Automatic Injection**

```java
@Autowired
public AuthService(UserRepository repo, JwtUtil jwt) {
    // Spring automatically provides repo and jwt!
}
```

### 3. **Singleton by Default**

```java
// Spring creates ONE instance and reuses it everywhere
// No duplication!
```

### 4. **Easy Testing**

```java
// Easy to mock in tests
@MockBean
private AuthService authService;
```

### 5. **Centralized Configuration**

```properties
# application.properties
jwt.secret=my-secret-key
db.url=mongodb://localhost:27017/tododb
```

---

## Summary

### Manual Dependency Injection:

- ✅ You have full control
- ❌ You do everything manually
- ❌ Easy to make mistakes
- ❌ Hard to maintain
- ❌ Lots of boilerplate code

### Spring Boot Dependency Injection:

- ✅ Automatic creation
- ✅ Automatic injection
- ✅ Less code
- ✅ Less errors
- ✅ Easy to maintain
- ✅ Singleton pattern
- ✅ Easy testing

**The beauty of Spring Boot:** You just mark classes with annotations, and Spring handles all the object creation and dependency injection automatically!





