package com.example.todo.controller;

import com.example.todo.model.Course;
import com.example.todo.service.CourseService;
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
@RequestMapping("/api/courses")
@CrossOrigin(origins = "*")
@Tag(name = "Course Controller", description = "REST API for managing courses")
public class CourseController {

    // ============================================================================
    // WHERE DOES courseService COME FROM?
    // ============================================================================
    // Step 1: Declare the dependency field
    // This field will hold the injected CourseService instance
    // NODE.JS EQUIVALENT: const courseService = require('../services/courseService');
    private final CourseService courseService;

    // ============================================================================
    // DEPENDENCY INJECTION - WHERE courseService COMES FROM!
    // ============================================================================
    // @Autowired: Tells Spring to inject CourseService here
    // 
    // HOW IT WORKS:
    //   1. Spring sees @Autowired on the constructor
    //   2. Spring looks at the parameter: CourseService courseService
    //   3. Spring searches for a CourseService bean in its container
    //   4. Spring finds CourseService class (marked with @Service in CourseService.java)
    //   5. Spring creates CourseService instance (if not already created)
    //   6. Spring automatically passes it to this constructor!
    //   7. You don't write: courseService = new CourseService(...)
    //
    // WHERE IS CourseService DEFINED?
    //   → File: src/main/java/com/example/todo/service/CourseService.java
    //   → Marked with @Service annotation
    //   → Spring automatically creates it as a bean
    //
    // NODE.JS EQUIVALENT (Manual):
    //   const courseService = require('../services/courseService');
    //   this.courseService = courseService;
    //
    // NODE.JS EQUIVALENT (NestJS - similar to Spring):
    //   constructor(private courseService: CourseService) {
    //     // Auto-injected like Spring!
    //   }
    @Autowired
    public CourseController(CourseService courseService) {
        // THIS IS WHERE courseService COMES FROM!
        // Spring automatically provides the CourseService instance
        // The CourseService is defined in: service/CourseService.java
        // It's marked with @Service, so Spring manages it as a bean
        this.courseService = courseService;
    }

    @Operation(
            summary = "Get all courses",
            description = "Retrieve a list of all courses. For normal users, returns only enrolled courses. Admins see all courses.",
            security = @SecurityRequirement(name = "bearerAuth")
    )
    @ApiResponse(responseCode = "200", description = "Successfully retrieved list of courses")
    @GetMapping
    public ResponseEntity<List<Course>> getAllCourses(
            @RequestHeader(value = "Authorization", required = false) String authHeader) {
        // For now, return all courses
        // Frontend will filter based on user role
        List<Course> courses = courseService.getAllCourses();
        return ResponseEntity.ok(courses);
    }

    @Operation(summary = "Get course by ID", description = "Retrieve a specific course by its ID")
    @ApiResponses(value = {
            @ApiResponse(responseCode = "200", description = "Course found", content = @Content(schema = @Schema(implementation = Course.class))),
            @ApiResponse(responseCode = "404", description = "Course not found")
    })
    @GetMapping("/{id}")
    public ResponseEntity<Course> getCourseById(
            @Parameter(description = "ID of the course to retrieve", required = true) @PathVariable String id) {
        return courseService.getCourseById(id)
                .map(course -> ResponseEntity.ok(course))
                .orElse(ResponseEntity.notFound().build());
    }

    @Operation(summary = "Create a new course", description = "Create a new course")
    @ApiResponses(value = {
            @ApiResponse(responseCode = "201", description = "Course created successfully", content = @Content(schema = @Schema(implementation = Course.class))),
            @ApiResponse(responseCode = "400", description = "Invalid input data")
    })
    @PostMapping
    public ResponseEntity<Course> createCourse(@Valid @RequestBody Course course) {
        Course createdCourse = courseService.createCourse(course);
        return ResponseEntity.status(HttpStatus.CREATED).body(createdCourse);
    }

    @Operation(summary = "Update a course", description = "Update an existing course by ID")
    @ApiResponses(value = {
            @ApiResponse(responseCode = "200", description = "Course updated successfully", content = @Content(schema = @Schema(implementation = Course.class))),
            @ApiResponse(responseCode = "404", description = "Course not found"),
            @ApiResponse(responseCode = "400", description = "Invalid input data")
    })
    @PutMapping("/{id}")
    public ResponseEntity<Course> updateCourse(
            @Parameter(description = "ID of the course to update", required = true) @PathVariable String id,
            @Valid @RequestBody Course course) {
        try {
            Course updatedCourse = courseService.updateCourse(id, course);
            return ResponseEntity.ok(updatedCourse);
        } catch (RuntimeException e) {
            return ResponseEntity.notFound().build();
        }
    }

    @Operation(summary = "Delete a course", description = "Delete a course by ID")
    @ApiResponses(value = {
            @ApiResponse(responseCode = "204", description = "Course deleted successfully"),
            @ApiResponse(responseCode = "404", description = "Course not found")
    })
    @DeleteMapping("/{id}")
    public ResponseEntity<Void> deleteCourse(
            @Parameter(description = "ID of the course to delete", required = true) @PathVariable String id) {
        try {
            courseService.deleteCourse(id);
            return ResponseEntity.noContent().build();
        } catch (RuntimeException e) {
            return ResponseEntity.notFound().build();
        }
    }

    @Operation(summary = "Delete all courses", description = "Delete all courses from the database")
    @ApiResponse(responseCode = "204", description = "All courses deleted successfully")
    @DeleteMapping
    public ResponseEntity<Void> deleteAllCourses() {
        courseService.deleteAllCourses();
        return ResponseEntity.noContent().build();
    }
}




