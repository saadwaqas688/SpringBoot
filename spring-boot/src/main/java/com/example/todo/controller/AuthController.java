package com.example.todo.controller;

import com.example.todo.dto.AuthResponse;
import com.example.todo.dto.SigninRequest;
import com.example.todo.dto.SignupRequest;
import com.example.todo.service.AuthService;
import io.swagger.v3.oas.annotations.Operation;
import io.swagger.v3.oas.annotations.media.Content;
import io.swagger.v3.oas.annotations.media.Schema;
import io.swagger.v3.oas.annotations.responses.ApiResponse;
import io.swagger.v3.oas.annotations.responses.ApiResponses;
import io.swagger.v3.oas.annotations.tags.Tag;
import jakarta.validation.Valid;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;

@RestController
@RequestMapping("/api/auth")
@CrossOrigin(origins = "*")
@Tag(name = "Authentication Controller", description = "REST API for user authentication")
public class AuthController {

    // ============================================================================
    // DEPENDENCY INJECTION - STEP 1: Declare the dependency
    // ============================================================================
    // This field will hold the injected AuthService instance
    // NODE.JS EQUIVALENT: You'd manually require it:
    //   const authService = require('../services/authService');
    private final AuthService authService;

    // ============================================================================
    // DEPENDENCY INJECTION - STEP 2: Constructor Injection (WHERE IT HAPPENS!)
    // ============================================================================
    // @Autowired: This annotation tells Spring "Inject dependencies here!"
    // 
    // WHAT HAPPENS:
    //   1. Spring sees @Autowired on the constructor
    //   2. Spring looks at the constructor parameter: AuthService authService
    //   3. Spring looks in its container for an AuthService bean
    //   4. Spring finds AuthService (marked with @Service)
    //   5. Spring automatically creates/injects AuthService instance here!
    //   6. You don't need to write: authService = new AuthService(...)
    //
    // NODE.JS EQUIVALENT (Manual - what you'd have to do):
    //   class AuthController {
    //     constructor() {
    //       // Manual dependency injection:
    //       const authService = require('../services/authService');
    //       this.authService = authService;
    //     }
    //   }
    //
    // NODE.JS EQUIVALENT (NestJS - similar to Spring):
    //   @Controller('auth')
    //   export class AuthController {
    //     constructor(
    //       private authService: AuthService  // Auto-injected (like Spring!)
    //     ) {}
    //   }
    //
    // NOTE: In modern Spring (4.3+), @Autowired is optional if you only have ONE constructor
    //       Spring automatically uses the constructor for dependency injection
    @Autowired
    public AuthController(AuthService authService) {
        // THIS IS WHERE DEPENDENCY INJECTION HAPPENS!
        // Spring automatically provides the AuthService instance
        // You don't create it - Spring does!
        this.authService = authService;
    }

    @Operation(summary = "User signup", description = "Register a new user account")
    @ApiResponses(value = {
            @ApiResponse(responseCode = "201", description = "User created successfully", content = @Content(schema = @Schema(implementation = AuthResponse.class))),
            @ApiResponse(responseCode = "400", description = "Invalid input data or user already exists")
    })
    @PostMapping("/signup")
    public ResponseEntity<?> signup(@Valid @RequestBody SignupRequest request) {
        try {
            AuthResponse response = authService.signup(request);
            return ResponseEntity.status(HttpStatus.CREATED).body(response);
        } catch (RuntimeException e) {
            // Return error message in response body
            return ResponseEntity.status(HttpStatus.BAD_REQUEST)
                    .body(java.util.Map.of("message", e.getMessage(), "error", "Bad Request"));
        }
    }

    @Operation(summary = "User signin", description = "Authenticate user and get JWT token")
    @ApiResponses(value = {
            @ApiResponse(responseCode = "200", description = "Authentication successful", content = @Content(schema = @Schema(implementation = AuthResponse.class))),
            @ApiResponse(responseCode = "401", description = "Invalid credentials")
    })
    @PostMapping("/signin")
    public ResponseEntity<AuthResponse> signin(@Valid @RequestBody SigninRequest request) {
        try {
            AuthResponse response = authService.signin(request);
            return ResponseEntity.ok(response);
        } catch (RuntimeException e) {
            return ResponseEntity.status(HttpStatus.UNAUTHORIZED).build();
        }
    }

    @Operation(summary = "Admin signup", description = "Register a new admin user account")
    @ApiResponses(value = {
            @ApiResponse(responseCode = "201", description = "Admin user created successfully", content = @Content(schema = @Schema(implementation = AuthResponse.class))),
            @ApiResponse(responseCode = "400", description = "Invalid input data or user already exists")
    })
    @PostMapping("/signup-admin")
    public ResponseEntity<?> signupAdmin(@Valid @RequestBody SignupRequest request) {
        try {
            AuthResponse response = authService.signupAdmin(request);
            return ResponseEntity.status(HttpStatus.CREATED).body(response);
        } catch (RuntimeException e) {
            // Return error message in response body
            return ResponseEntity.status(HttpStatus.BAD_REQUEST)
                    .body(java.util.Map.of("message", e.getMessage(), "error", "Bad Request"));
        }
    }
}


