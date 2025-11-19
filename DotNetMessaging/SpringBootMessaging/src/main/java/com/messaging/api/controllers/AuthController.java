package com.messaging.api.controllers;

import com.messaging.api.dtos.AuthDTOs;
import com.messaging.api.services.AuthService;
import lombok.RequiredArgsConstructor;
import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;

@RestController
@RequestMapping("/api/auth")
@RequiredArgsConstructor
public class AuthController {
    
    private final AuthService authService;
    
    @PostMapping("/register")
    public ResponseEntity<?> register(@RequestBody AuthDTOs.RegisterRequest request) {
        return authService.register(request)
                .map(response -> ResponseEntity.ok(response))
                .orElse(ResponseEntity.badRequest()
                        .body(new ErrorResponse("Email or username already exists")));
    }
    
    @PostMapping("/login")
    public ResponseEntity<?> login(@RequestBody AuthDTOs.LoginRequest request) {
        return authService.login(request)
                .map(response -> ResponseEntity.ok(response))
                .orElse(ResponseEntity.status(HttpStatus.UNAUTHORIZED)
                        .body(new ErrorResponse("Invalid email or password")));
    }
    
    private record ErrorResponse(String message) {}
}

