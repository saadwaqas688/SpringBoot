# Quick Start Guide

## How to Run the Todo Microservice

### Step 1: Verify Java is Installed

```cmd
java -version
```

You should see Java 17 or higher. ✅ You already have Java 17 installed!

### Step 2: Run the Application

**On Windows (PowerShell or Command Prompt):**

```cmd
.\mvnw.cmd spring-boot:run
```

**On Linux/Mac:**

```bash
./mvnw spring-boot:run
```

### Step 3: Wait for Application to Start

You'll see output like:

```
  .   ____          _            __ _ _
 /\\ / ___'_ __ _ _(_)_ __  __ _ \ \ \ \
( ( )\___ | '_ | '_| | '_ \/ _` | \ \ \ \
 \\/  ___)| |_)| | | | | || (_| |  ) ) ) )
  '  |____| .__|_| |_|_| |_\__, | / / / /
 =========|_|==============|___/=/_/_/_/
 :: Spring Boot ::                (v3.2.0)

...
Started TodoMicroserviceApplication in X.XXX seconds
```

### Step 4: Test the API

The application will be running at: **http://localhost:8080**

**Test with curl or Postman:**

1. **Get all todos:**

   ```cmd
   curl http://localhost:8080/api/todos
   ```

2. **Create a todo:**

   ```cmd
   curl -X POST http://localhost:8080/api/todos -H "Content-Type: application/json" -d "{\"title\":\"My first task\",\"description\":\"Learn Spring Boot\",\"completed\":false}"
   ```

3. **Access H2 Database Console:**
   - Open browser: http://localhost:8080/h2-console
   - JDBC URL: `jdbc:h2:mem:tododb`
   - Username: `sa`
   - Password: (leave empty)

### Alternative: Run from IDE

1. Open the project in IntelliJ IDEA, Eclipse, or VS Code
2. Find `TodoMicroserviceApplication.java`
3. Right-click → Run

### Troubleshooting

**If you get "JAVA_HOME not found":**

- Set JAVA_HOME environment variable to your JDK installation path
- Example: `set JAVA_HOME=C:\Program Files\Microsoft\jdk-17.0.16+8`

**If port 8080 is already in use:**

- Change the port in `src/main/resources/application.properties`:
  ```
  server.port=8081
  ```

**To stop the application:**

- Press `Ctrl+C` in the terminal




