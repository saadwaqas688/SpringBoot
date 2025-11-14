package com.example.todo.controller;

import com.example.todo.model.Discussion;
import com.example.todo.service.DiscussionService;
import com.example.todo.service.EnrollmentService;
import com.example.todo.util.JwtUtil;
import com.example.todo.security.CurrentUser;
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
import java.util.stream.Collectors;

@RestController
@RequestMapping("/api/discussions")
@CrossOrigin(origins = "*")
@Tag(name = "Discussion Controller", description = "REST API for managing course discussions")
public class DiscussionController {

    private final DiscussionService discussionService;
    private final EnrollmentService enrollmentService;
    private final JwtUtil jwtUtil;

    @Autowired
    public DiscussionController(DiscussionService discussionService, EnrollmentService enrollmentService, JwtUtil jwtUtil) {
        this.discussionService = discussionService;
        this.enrollmentService = enrollmentService;
        this.jwtUtil = jwtUtil;
    }

    @Operation(
            summary = "Get all discussions",
            description = "Retrieve a list of all discussions. For normal users, returns only discussions from enrolled courses. Admins see all discussions.",
            security = @SecurityRequirement(name = "bearerAuth")
    )
    @ApiResponse(responseCode = "200", description = "Successfully retrieved list of discussions")
    @GetMapping
    public ResponseEntity<List<Discussion>> getAllDiscussions(
            @RequestHeader(value = "Authorization", required = false) String authHeader) {
        try {
            String userId = CurrentUser.getUserId();
            String role = null;
            
            if (authHeader != null && authHeader.startsWith("Bearer ")) {
                String token = authHeader.substring(7);
                role = jwtUtil.getRoleFromToken(token);
                if (userId == null) {
                    userId = jwtUtil.getUserIdFromToken(token);
                }
            }

            List<Discussion> discussions;
            
            // If user is admin, return all discussions
            if ("ADMIN".equals(role)) {
                discussions = discussionService.getAllDiscussions();
            } else if (userId != null) {
                // For normal users, return only discussions from enrolled courses
                List<String> enrolledCourseIds = enrollmentService.getEnrolledCourses(userId)
                        .stream()
                        .map(course -> course.getId())
                        .collect(Collectors.toList());
                
                if (enrolledCourseIds.isEmpty()) {
                    discussions = List.of();
                } else {
                    discussions = discussionService.getDiscussionsByCourseIds(enrolledCourseIds);
                }
            } else {
                // No auth, return empty list
                discussions = List.of();
            }
            
            return ResponseEntity.ok(discussions);
        } catch (Exception e) {
            // If error, return all (for backward compatibility)
            return ResponseEntity.ok(discussionService.getAllDiscussions());
        }
    }

    @Operation(summary = "Get discussion by ID", description = "Retrieve a specific discussion by its ID")
    @ApiResponses(value = {
            @ApiResponse(responseCode = "200", description = "Discussion found", content = @Content(schema = @Schema(implementation = Discussion.class))),
            @ApiResponse(responseCode = "404", description = "Discussion not found")
    })
    @GetMapping("/{id}")
    public ResponseEntity<Discussion> getDiscussionById(
            @Parameter(description = "ID of the discussion to retrieve", required = true) @PathVariable String id) {
        return discussionService.getDiscussionById(id)
                .map(discussion -> ResponseEntity.ok(discussion))
                .orElse(ResponseEntity.notFound().build());
    }

    @Operation(summary = "Get discussions by course ID", description = "Retrieve all discussions for a specific course")
    @ApiResponse(responseCode = "200", description = "Successfully retrieved discussions")
    @GetMapping("/course/{courseId}")
    public ResponseEntity<List<Discussion>> getDiscussionsByCourseId(
            @Parameter(description = "ID of the course", required = true) @PathVariable String courseId) {
        List<Discussion> discussions = discussionService.getDiscussionsByCourseId(courseId);
        return ResponseEntity.ok(discussions);
    }

    @Operation(
            summary = "Create a new discussion",
            description = "Create a new discussion for a course. User must be enrolled in the course.",
            security = @SecurityRequirement(name = "bearerAuth")
    )
    @ApiResponses(value = {
            @ApiResponse(responseCode = "201", description = "Discussion created successfully", content = @Content(schema = @Schema(implementation = Discussion.class))),
            @ApiResponse(responseCode = "400", description = "Invalid input data"),
            @ApiResponse(responseCode = "403", description = "User not enrolled in course")
    })
    @PostMapping
    public ResponseEntity<?> createDiscussion(
            @Valid @RequestBody Discussion discussion,
            @RequestHeader("Authorization") String authHeader) {
        try {
            String token = extractToken(authHeader);
            String userId = jwtUtil.getUserIdFromToken(token);
            String role = jwtUtil.getRoleFromToken(token);

            // Check if user is enrolled (unless admin)
            if (!"ADMIN".equals(role)) {
                if (!enrollmentService.isUserEnrolled(discussion.getCourseId(), userId)) {
                    return ResponseEntity.status(HttpStatus.FORBIDDEN)
                            .body(java.util.Map.of("message", "You must be enrolled in the course to create discussions"));
                }
            }

            Discussion createdDiscussion = discussionService.createDiscussion(discussion, userId);
            return ResponseEntity.status(HttpStatus.CREATED).body(createdDiscussion);
        } catch (RuntimeException e) {
            return ResponseEntity.status(HttpStatus.BAD_REQUEST)
                    .body(java.util.Map.of("message", e.getMessage()));
        }
    }

    @Operation(
            summary = "Update a discussion",
            description = "Update an existing discussion by ID. Only the creator can update.",
            security = @SecurityRequirement(name = "bearerAuth")
    )
    @ApiResponses(value = {
            @ApiResponse(responseCode = "200", description = "Discussion updated successfully", content = @Content(schema = @Schema(implementation = Discussion.class))),
            @ApiResponse(responseCode = "404", description = "Discussion not found"),
            @ApiResponse(responseCode = "400", description = "Invalid input data"),
            @ApiResponse(responseCode = "403", description = "You can only update your own discussions")
    })
    @PutMapping("/{id}")
    public ResponseEntity<?> updateDiscussion(
            @Parameter(description = "ID of the discussion to update", required = true) @PathVariable String id,
            @Valid @RequestBody Discussion discussion,
            @RequestHeader("Authorization") String authHeader) {
        try {
            String token = extractToken(authHeader);
            String userId = jwtUtil.getUserIdFromToken(token);
            
            Discussion updatedDiscussion = discussionService.updateDiscussion(id, discussion, userId);
            return ResponseEntity.ok(updatedDiscussion);
        } catch (RuntimeException e) {
            if (e.getMessage().contains("not found")) {
                return ResponseEntity.notFound().build();
            }
            return ResponseEntity.status(HttpStatus.FORBIDDEN)
                    .body(java.util.Map.of("message", e.getMessage()));
        }
    }

    @Operation(
            summary = "Delete a discussion",
            description = "Delete a discussion by ID. Only the creator can delete.",
            security = @SecurityRequirement(name = "bearerAuth")
    )
    @ApiResponses(value = {
            @ApiResponse(responseCode = "204", description = "Discussion deleted successfully"),
            @ApiResponse(responseCode = "404", description = "Discussion not found"),
            @ApiResponse(responseCode = "403", description = "You can only delete your own discussions")
    })
    @DeleteMapping("/{id}")
    public ResponseEntity<?> deleteDiscussion(
            @Parameter(description = "ID of the discussion to delete", required = true) @PathVariable String id,
            @RequestHeader("Authorization") String authHeader) {
        try {
            String token = extractToken(authHeader);
            String userId = jwtUtil.getUserIdFromToken(token);
            
            discussionService.deleteDiscussion(id, userId);
            return ResponseEntity.noContent().build();
        } catch (RuntimeException e) {
            if (e.getMessage().contains("not found")) {
                return ResponseEntity.notFound().build();
            }
            return ResponseEntity.status(HttpStatus.FORBIDDEN)
                    .body(java.util.Map.of("message", e.getMessage()));
        }
    }

    private String extractToken(String authHeader) {
        if (authHeader != null && authHeader.startsWith("Bearer ")) {
            return authHeader.substring(7);
        }
        throw new RuntimeException("Invalid authorization header");
    }
}




