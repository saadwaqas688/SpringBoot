package com.example.todo.controller;

import com.example.todo.dto.EnrolledUserDto;
import com.example.todo.dto.EnrollUsersRequest;
import com.example.todo.model.Course;
import com.example.todo.model.CourseEnrollment;
import com.example.todo.service.EnrollmentService;
import com.example.todo.util.JwtUtil;
import io.swagger.v3.oas.annotations.Operation;
import io.swagger.v3.oas.annotations.Parameter;
import io.swagger.v3.oas.annotations.media.Content;
import io.swagger.v3.oas.annotations.media.Schema;
import io.swagger.v3.oas.annotations.responses.ApiResponse;
import io.swagger.v3.oas.annotations.responses.ApiResponses;
import io.swagger.v3.oas.annotations.security.SecurityRequirement;
import io.swagger.v3.oas.annotations.tags.Tag;
import jakarta.validation.Valid;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;

import java.util.List;

@RestController
@RequestMapping("/api/enrollments")
@CrossOrigin(origins = "*")
@Tag(name = "Enrollment Controller", description = "REST API for course enrollment management")
public class EnrollmentController {

    private final EnrollmentService enrollmentService;
    private final JwtUtil jwtUtil;

    @Autowired
    public EnrollmentController(EnrollmentService enrollmentService, JwtUtil jwtUtil) {
        this.enrollmentService = enrollmentService;
        this.jwtUtil = jwtUtil;
    }

    @Operation(
            summary = "Enroll users to a course",
            description = "Grant access to a course for one or multiple users. Requires ADMIN role.",
            security = @SecurityRequirement(name = "bearerAuth")
    )
    @ApiResponses(value = {
            @ApiResponse(responseCode = "201", description = "Users enrolled successfully"),
            @ApiResponse(responseCode = "400", description = "Invalid input data"),
            @ApiResponse(responseCode = "403", description = "Access denied - Admin role required")
    })
    @PostMapping
    public ResponseEntity<List<CourseEnrollment>> enrollUsers(
            @Valid @RequestBody EnrollUsersRequest request,
            @RequestHeader("Authorization") String authHeader) {
        try {
            String token = extractToken(authHeader);
            String grantedBy = jwtUtil.getUserIdFromToken(token);
            String role = jwtUtil.getRoleFromToken(token);

            // Check if user is admin
            if (!"ADMIN".equals(role)) {
                return ResponseEntity.status(HttpStatus.FORBIDDEN).build();
            }

            List<CourseEnrollment> enrollments = enrollmentService.enrollUsers(
                    request.getCourseId(),
                    request.getUserIds(),
                    grantedBy
            );

            return ResponseEntity.status(HttpStatus.CREATED).body(enrollments);
        } catch (RuntimeException e) {
            return ResponseEntity.badRequest().build();
        }
    }

    @Operation(
            summary = "Get enrolled users for a course",
            description = "Get list of all users enrolled in a course. Requires ADMIN role.",
            security = @SecurityRequirement(name = "bearerAuth")
    )
    @ApiResponses(value = {
            @ApiResponse(responseCode = "200", description = "Enrolled users retrieved successfully"),
            @ApiResponse(responseCode = "403", description = "Access denied - Admin role required"),
            @ApiResponse(responseCode = "404", description = "Course not found")
    })
    @GetMapping("/course/{courseId}")
    public ResponseEntity<List<EnrolledUserDto>> getEnrolledUsers(
            @Parameter(description = "ID of the course", required = true) @PathVariable String courseId,
            @RequestHeader("Authorization") String authHeader) {
        try {
            String token = extractToken(authHeader);
            String role = jwtUtil.getRoleFromToken(token);

            // Check if user is admin
            if (!"ADMIN".equals(role)) {
                return ResponseEntity.status(HttpStatus.FORBIDDEN).build();
            }

            List<EnrolledUserDto> enrolledUsers = enrollmentService.getEnrolledUsers(courseId);
            return ResponseEntity.ok(enrolledUsers);
        } catch (RuntimeException e) {
            return ResponseEntity.notFound().build();
        }
    }

    @Operation(
            summary = "Get enrolled courses for current user",
            description = "Get list of all courses the current user is enrolled in.",
            security = @SecurityRequirement(name = "bearerAuth")
    )
    @ApiResponses(value = {
            @ApiResponse(responseCode = "200", description = "Enrolled courses retrieved successfully"),
            @ApiResponse(responseCode = "401", description = "Unauthorized")
    })
    @GetMapping("/my-courses")
    public ResponseEntity<List<Course>> getMyEnrolledCourses(
            @RequestHeader("Authorization") String authHeader) {
        try {
            String token = extractToken(authHeader);
            String userId = jwtUtil.getUserIdFromToken(token);
            
            List<Course> courses = enrollmentService.getEnrolledCourses(userId);
            return ResponseEntity.ok(courses);
        } catch (RuntimeException e) {
            return ResponseEntity.status(HttpStatus.UNAUTHORIZED).build();
        }
    }

    private String extractToken(String authHeader) {
        if (authHeader != null && authHeader.startsWith("Bearer ")) {
            return authHeader.substring(7);
        }
        throw new RuntimeException("Invalid authorization header");
    }
}



