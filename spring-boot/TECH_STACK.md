# Technology Stack & Tools

Complete list of all technologies, frameworks, libraries, and tools used in this Todo Microservice project.

## 🚀 Core Framework

### Spring Boot

- **Version:** 3.2.0
- **Purpose:** Main application framework
- **Features Used:**
  - Spring Boot Web (REST API)
  - Spring Boot Data MongoDB
  - Spring Boot Validation
  - Spring Boot DevTools
  - Spring Boot Test

## ☕ Programming Language

### Java

- **Version:** 17 (LTS)
- **Features:**
  - Records (if used)
  - Pattern Matching
  - Text Blocks
  - Sealed Classes support

## 🗄️ Database

### MongoDB

- **Type:** NoSQL Document Database
- **Version:** Latest (local installation)
- **Connection:**
  - Host: `localhost`
  - Port: `27017`
  - Database: `tododb`
- **Driver:** Spring Data MongoDB

## 🏗️ Build Tool

### Apache Maven

- **Version:** 3.9.5 (via Maven Wrapper)
- **Purpose:**
  - Dependency management
  - Project building
  - Packaging
- **Wrapper:** Included (`mvnw.cmd` for Windows)

## 📦 Core Dependencies

### Spring Framework Modules

#### 1. Spring Boot Starter Web

- **Artifact:** `spring-boot-starter-web`
- **Includes:**
  - Spring MVC (REST controllers)
  - Embedded Tomcat server
  - Jackson (JSON serialization)
  - Spring Web
  - Spring Boot Auto-Configuration

#### 2. Spring Data MongoDB

- **Artifact:** `spring-boot-starter-data-mongodb`
- **Includes:**
  - Spring Data MongoDB
  - MongoDB Java Driver
  - Spring Data Commons
- **Features:**
  - Repository pattern
  - Query methods
  - Automatic schema mapping

#### 3. Spring Boot Validation

- **Artifact:** `spring-boot-starter-validation`
- **Includes:**
  - Jakarta Bean Validation (Hibernate Validator)
  - Validation annotations (`@NotNull`, `@NotBlank`, `@Size`, etc.)

#### 4. Spring Boot DevTools

- **Artifact:** `spring-boot-devtools`
- **Features:**
  - Automatic application restart
  - LiveReload server
  - Property defaults
  - Developer tools

#### 5. Spring Boot Test

- **Artifact:** `spring-boot-starter-test`
- **Includes:**
  - JUnit 5
  - Mockito
  - AssertJ
  - Spring Test
  - Hamcrest

## 🔧 Development Tools

### Lombok

- **Artifact:** `lombok`
- **Purpose:** Reduces boilerplate code
- **Features:**
  - `@Getter`, `@Setter`
  - `@NoArgsConstructor`, `@AllArgsConstructor`
  - `@Builder`
  - `@Data`
- **Note:** Currently optional (not heavily used)

### SpringDoc OpenAPI (Swagger UI)

- **Artifact:** `springdoc-openapi-starter-webmvc-ui`
- **Version:** 2.3.0
- **Purpose:** API documentation and testing
- **Features:**
  - Interactive API documentation
  - OpenAPI 3.0 specification
  - Try-it-out functionality
  - Schema documentation
- **Endpoints:**
  - Swagger UI: `/swagger-ui.html`
  - OpenAPI JSON: `/api-docs`

## 🌐 Web Server

### Embedded Tomcat

- **Included in:** `spring-boot-starter-web`
- **Port:** 8080 (configurable)
- **Type:** Embedded (no separate installation needed)

## 📝 API Documentation Standards

### OpenAPI 3.0

- **Specification:** OpenAPI 3.0
- **Implementation:** SpringDoc OpenAPI
- **Annotations Used:**
  - `@Operation`
  - `@ApiResponse`
  - `@Parameter`
  - `@Tag`
  - `@Schema`

## 🔍 Validation Framework

### Jakarta Bean Validation

- **Version:** 3.0 (via Hibernate Validator)
- **Annotations Used:**
  - `@NotBlank`
  - `@Size`
  - `@Valid`
- **Implementation:** Hibernate Validator

## 🗂️ Project Structure

### Maven Standard Directory Layout

```
src/
├── main/
│   ├── java/          # Java source files
│   └── resources/     # Configuration files
└── test/
    ├── java/          # Test source files
    └── resources/     # Test resources
```

## 📋 Configuration Files

### Application Configuration

- **Format:** Properties & YAML
- **Files:**
  - `application.properties` (primary)
  - `application.yml` (alternative)
- **Configuration Areas:**
  - Server settings
  - MongoDB connection
  - Logging
  - DevTools
  - Swagger/OpenAPI

### Maven Configuration

- **File:** `pom.xml`
- **Purpose:**
  - Dependency management
  - Build configuration
  - Plugin configuration

## 🛠️ Development Features

### Hot Reload / Auto-Restart

- **Tool:** Spring Boot DevTools
- **Features:**
  - Automatic restart on code changes
  - Fast restart (Spring context only)
  - LiveReload server (port 35729)
  - File watching

### Logging

- **Framework:** Logback (via Spring Boot)
- **Levels Configured:**
  - DEBUG: Application code
  - INFO: Spring framework

## 🧪 Testing Framework

### JUnit 5

- **Included in:** `spring-boot-starter-test`
- **Purpose:** Unit and integration testing

### Mockito

- **Included in:** `spring-boot-starter-test`
- **Purpose:** Mocking dependencies in tests

## 📚 Documentation Tools

### Markdown

- **Files:**
  - `README.md` - Main documentation
  - `QUICKSTART.md` - Quick start guide
  - `HOT_RELOAD_GUIDE.md` - Hot reload documentation
  - `STARTUP_PROCESS.md` - Application startup explanation
  - `SWAGGER_REFRESH.md` - Swagger troubleshooting
  - `TECH_STACK.md` - This file

## 🔐 Security (Future/Not Currently Implemented)

### Potential Additions:

- Spring Security (for authentication/authorization)
- JWT (JSON Web Tokens)
- OAuth2
- CORS configuration (currently open with `@CrossOrigin`)

## 📊 Data Access Layer

### Spring Data MongoDB Repository

- **Interface:** `MongoRepository<T, ID>`
- **Features:**
  - CRUD operations
  - Custom query methods
  - Automatic implementation
  - Query derivation from method names

## 🎯 API Design

### RESTful Architecture

- **Style:** REST
- **HTTP Methods Used:**
  - GET (retrieve)
  - POST (create)
  - PUT (update)
  - PATCH (partial update)
  - DELETE (remove)

### Response Format

- **Format:** JSON
- **Serialization:** Jackson (automatic)

## 🌍 Cross-Origin Resource Sharing (CORS)

### Current Configuration

- **Annotation:** `@CrossOrigin(origins = "*")`
- **Status:** Open to all origins (development)

## 📦 Packaging

### Spring Boot Maven Plugin

- **Purpose:**
  - Create executable JAR
  - Package application
  - Exclude DevTools from production

## 🔄 Version Control

### Git

- **Files Ignored:** `.gitignore`
- **Excluded:**
  - `target/` directory
  - IDE files
  - Build artifacts

## 🖥️ Runtime Environment

### Java Virtual Machine (JVM)

- **Version:** 17
- **Type:** OpenJDK or Oracle JDK

### Operating System Support

- ✅ Windows
- ✅ Linux
- ✅ macOS

## 📱 API Client Tools (Recommended)

### For Testing:

- **Postman** - API testing
- **cURL** - Command-line tool
- **Swagger UI** - Built-in interactive testing
- **HTTPie** - Modern HTTP client

## 🔍 Monitoring & Debugging

### Built-in Features:

- Spring Boot Actuator (not currently included, but can be added)
- Logging framework (Logback)
- DevTools restart logging

## 📈 Performance

### Optimizations:

- Fast restart (DevTools)
- Embedded server (no deployment overhead)
- Connection pooling (MongoDB driver)

## 🎨 Code Quality Tools (Potential)

### Can be added:

- **Checkstyle** - Code style checking
- **SpotBugs** - Bug detection
- **PMD** - Code analysis
- **SonarQube** - Code quality platform

## 📦 Dependency Management

### Maven Central Repository

- **Source:** Maven Central
- **Proxy:** Local Maven repository (`~/.m2/repository`)

## 🔗 External Services

### MongoDB

- **Type:** Local installation
- **Connection:** Direct connection
- **Authentication:** Optional (currently none)

## 📝 Summary Table

| Category       | Technology              | Version           |
| -------------- | ----------------------- | ----------------- |
| **Framework**  | Spring Boot             | 3.2.0             |
| **Language**   | Java                    | 17                |
| **Database**   | MongoDB                 | Latest            |
| **Build Tool** | Maven                   | 3.9.5             |
| **Web Server** | Embedded Tomcat         | (via Spring Boot) |
| **API Docs**   | SpringDoc OpenAPI       | 2.3.0             |
| **Validation** | Jakarta Bean Validation | 3.0               |
| **Testing**    | JUnit 5                 | (via Spring Boot) |
| **Dev Tools**  | Spring Boot DevTools    | (via Spring Boot) |
| **Logging**    | Logback                 | (via Spring Boot) |

## 🚀 Getting Started

To see all dependencies in detail:

```cmd
.\mvnw.cmd dependency:tree
```

To see dependency versions:

```cmd
.\mvnw.cmd dependency:list
```

## 📚 Additional Resources

- [Spring Boot Documentation](https://spring.io/projects/spring-boot)
- [Spring Data MongoDB](https://spring.io/projects/spring-data-mongodb)
- [MongoDB Documentation](https://docs.mongodb.com/)
- [SpringDoc OpenAPI](https://springdoc.org/)
- [Maven Documentation](https://maven.apache.org/)

---

**Last Updated:** Based on current project configuration
**Project Version:** 1.0.0








