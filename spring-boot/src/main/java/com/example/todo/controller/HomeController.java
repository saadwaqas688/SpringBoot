package com.example.todo.controller;

import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.RestController;

import java.util.HashMap;
import java.util.Map;

@RestController
public class HomeController {  

    @GetMapping("/")
    public ResponseEntity<Map<String, Object>> home() {
        Map<String, Object> response = new HashMap<>();
        response.put("message", "Welcome to Course Management API");
        response.put("version", "1.0.0");
        response.put("status", "running");   
        
        Map<String, String> endpoints = new HashMap<>();
        endpoints.put("POST /api/auth/signup", "User registration");
        endpoints.put("POST /api/auth/signin", "User authentication");
        endpoints.put("GET /api/courses", "Get all courses");
        endpoints.put("GET /api/courses/{id}", "Get course by ID");
        endpoints.put("POST /api/courses", "Create a new course");
        endpoints.put("PUT /api/courses/{id}", "Update a course");
        endpoints.put("DELETE /api/courses/{id}", "Delete a course");
        endpoints.put("GET /api/discussions", "Get all discussions");
        endpoints.put("GET /api/discussions/{id}", "Get discussion by ID");
        endpoints.put("POST /api/discussions", "Create a new discussion");
        endpoints.put("GET /api/posts/discussion/{discussionId}", "Get all posts for a discussion");
        endpoints.put("GET /api/posts/{id}", "Get post by ID");
        endpoints.put("POST /api/posts", "Create a new post");
        endpoints.put("PUT /api/posts/{id}", "Update a post");
        endpoints.put("DELETE /api/posts/{id}", "Delete a post");
        endpoints.put("POST /api/enrollments", "Enroll users to a course (Admin only)");
        endpoints.put("GET /api/enrollments/course/{courseId}", "Get enrolled users for a course (Admin only)");
        endpoints.put("GET /swagger-ui.html", "Swagger UI - Interactive API Documentation");
        endpoints.put("GET /api-docs", "OpenAPI JSON Specification");
        
        response.put("endpoints", endpoints);
        return ResponseEntity.ok(response);
    }
}

