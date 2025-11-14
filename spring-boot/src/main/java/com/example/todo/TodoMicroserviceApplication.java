// ============================================================================
// PACKAGE DECLARATION
// ============================================================================
// This tells Java that this class belongs to the "com.example.todo" package.
// NODE.JS EQUIVALENT: This is like the folder structure
//   In Node.js: src/app.js or server.js (root level)
//   In NestJS:  src/main.ts
package com.example.todo;

// ============================================================================
// IMPORTS
// ============================================================================
// SpringApplication: The main class that starts the Spring Boot application
// NODE.JS EQUIVALENT: Like the Express app or NestJS application bootstrap
//   In Node.js: const app = express();
//   In NestJS:   const app = await NestFactory.create(AppModule);
import org.springframework.boot.SpringApplication;

// SpringBootApplication: Annotation that enables Spring Boot auto-configuration
// NODE.JS EQUIVALENT: 
//   - In Express: No direct equivalent (you manually configure everything)
//   - In NestJS: @Module() decorator that auto-configures the app
//   This annotation does A LOT automatically:
//     - Component scanning (finds all @Component, @Service, @Controller classes)
//     - Auto-configuration (sets up database, web server, etc. based on dependencies)
//     - Enables Spring Boot features
import org.springframework.boot.autoconfigure.SpringBootApplication;

// ============================================================================
// MAIN APPLICATION CLASS
// ============================================================================
// @SpringBootApplication is a "convenience annotation" that combines:
//   1. @Configuration - Marks this as a configuration class
//   2. @EnableAutoConfiguration - Enables Spring Boot auto-configuration
//   3. @ComponentScan - Scans for components in this package and sub-packages
//
// NODE.JS EQUIVALENT (Plain Node.js with Express):
//   // app.js
//   const express = require('express');
//   const app = express();
//   
//   // Configure middleware, routes, etc.
//   app.use(express.json());
//   app.use('/api', routes);
//   
//   // Start server
//   const PORT = process.env.PORT || 3000;
//   app.listen(PORT, () => {
//     console.log(`Server running on port ${PORT}`);
//   });
//
// NODE.JS EQUIVALENT (NestJS):
//   // main.ts
//   import { NestFactory } from '@nestjs/core';
//   import { AppModule } from './app.module';
//   
//   async function bootstrap() {
//     const app = await NestFactory.create(AppModule);
//     await app.listen(3000);
//   }
//   bootstrap();
//
// WHAT @SpringBootApplication DOES:
//   1. Scans for @Component, @Service, @Controller, @Repository classes
//   2. Auto-configures based on classpath (if MongoDB is on classpath, configures it)
//   3. Sets up embedded web server (Tomcat by default)
//   4. Enables Spring Boot features (dev tools, actuator, etc.)
@SpringBootApplication
public class TodoMicroserviceApplication {
    // This is the main entry point of the application
    // NODE.JS EQUIVALENT: This is like the entry point in Node.js
    //   In Node.js: node app.js (runs the file)
    //   In Java: java -jar app.jar (runs the main method)

    // ============================================================================
    // MAIN METHOD - Application Entry Point
    // ============================================================================
    // public static void main(String[] args)
    //   - public: Can be called from anywhere
    //   - static: Belongs to the class, not an instance (called without creating object)
    //   - void: Doesn't return anything
    //   - main: Special method name - JVM looks for this to start the program
    //   - String[] args: Command-line arguments (like process.argv in Node.js)
    //
    // NODE.JS EQUIVALENT:
    //   // In Node.js, the entry point is just the file itself:
    //   // app.js
    //   const express = require('express');
    //   const app = express();
    //   app.listen(3000);
    //
    //   // Or with command-line args:
    //   const args = process.argv.slice(2);  // Like String[] args
    //   const port = args[0] || 3000;
    //
    // NODE.JS EQUIVALENT (NestJS):
    //   async function bootstrap() {
    //     const app = await NestFactory.create(AppModule);
    //     await app.listen(3000);
    //   }
    //   bootstrap();  // This is like main() - the entry point
    public static void main(String[] args) {
        // SpringApplication.run() does the following:
        //   1. Creates Spring Application Context (container for all beans/components)
        //   2. Registers all @Component, @Service, @Controller classes
        //   3. Auto-configures based on dependencies (MongoDB, Web, etc.)
        //   4. Starts embedded web server (Tomcat on port 8080 by default)
        //   5. Makes the application ready to handle HTTP requests
        //
        // NODE.JS EQUIVALENT (Plain Node.js):
        //   const express = require('express');
        //   const app = express();
        //   
        //   // Register routes, middleware
        //   app.get('/api/health', (req, res) => { ... });
        //   
        //   // Start server
        //   app.listen(8080, () => {
        //     console.log('Server started on port 8080');
        //   });
        //
        // NODE.JS EQUIVALENT (NestJS):
        //   const app = await NestFactory.create(AppModule);
        //   // AppModule contains all controllers, services, etc.
        //   await app.listen(8080);
        //
        // PARAMETERS:
        //   - TodoMicroserviceApplication.class: The main class (tells Spring where to start scanning)
        //   - args: Command-line arguments (like: java -jar app.jar --server.port=9090)
        //
        // WHAT HAPPENS WHEN YOU RUN THIS:
        //   1. Spring scans com.example.todo package and sub-packages
        //   2. Finds all @Component, @Service, @Controller, @Repository classes
        //   3. Creates instances of them (dependency injection)
        //   4. Configures MongoDB connection (from application.properties)
        //   5. Registers all REST controllers (@RestController classes)
        //   6. Starts Tomcat server on port 8080
        //   7. Application is ready to accept HTTP requests!
        SpringApplication.run(TodoMicroserviceApplication.class, args);
        
        // After this line, the application is running and listening for requests
        // NODE.JS: After app.listen(), the server is running
    }
}





