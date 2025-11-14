package com.example.todo.controller;

import com.example.todo.dto.CreatePostRequest;
import com.example.todo.dto.UpdatePostRequest;
import com.example.todo.model.Post;
import com.example.todo.service.PostService;
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
@RequestMapping("/api/posts")
@CrossOrigin(origins = "*")
@Tag(name = "Post Controller", description = "REST API for managing discussion posts")
public class PostController {

    private final PostService postService;
    private final JwtUtil jwtUtil;

    @Autowired
    public PostController(PostService postService, JwtUtil jwtUtil) {
        this.postService = postService;
        this.jwtUtil = jwtUtil;
    }

    @Operation(
            summary = "Get all posts by discussion ID",
            description = "Retrieve all posts for a specific discussion",
            security = @SecurityRequirement(name = "bearerAuth")
    )
    @ApiResponse(responseCode = "200", description = "Successfully retrieved list of posts")
    @GetMapping("/discussion/{discussionId}")
    public ResponseEntity<List<Post>> getPostsByDiscussionId(
            @Parameter(description = "ID of the discussion", required = true) @PathVariable String discussionId) {
        List<Post> posts = postService.getPostsByDiscussionId(discussionId);
        return ResponseEntity.ok(posts);
    }

    @Operation(
            summary = "Get post by ID",
            description = "Retrieve a specific post by its ID",
            security = @SecurityRequirement(name = "bearerAuth")
    )
    @ApiResponses(value = {
            @ApiResponse(responseCode = "200", description = "Post found", content = @Content(schema = @Schema(implementation = Post.class))),
            @ApiResponse(responseCode = "404", description = "Post not found")
    })
    @GetMapping("/{id}")
    public ResponseEntity<Post> getPostById(
            @Parameter(description = "ID of the post to retrieve", required = true) @PathVariable String id) {
        return postService.getPostById(id)
                .map(post -> ResponseEntity.ok(post))
                .orElse(ResponseEntity.notFound().build());
    }

    @Operation(
            summary = "Create a new post",
            description = "Create a new post for a discussion",
            security = @SecurityRequirement(name = "bearerAuth")
    )
    @ApiResponses(value = {
            @ApiResponse(responseCode = "201", description = "Post created successfully", content = @Content(schema = @Schema(implementation = Post.class))),
            @ApiResponse(responseCode = "400", description = "Invalid input data")
    })
    @PostMapping
    public ResponseEntity<?> createPost(
            @Valid @RequestBody CreatePostRequest request,
            @RequestHeader("Authorization") String authHeader) {
        try {
            String token = extractToken(authHeader);
            String userId = jwtUtil.getUserIdFromToken(token);
            
            Post post = new Post();
            post.setDiscussionId(request.getDiscussionId());
            post.setCourseId(request.getCourseId());
            post.setContent(request.getContent());
            
            Post createdPost = postService.createPost(post, userId);
            return ResponseEntity.status(HttpStatus.CREATED).body(createdPost);
        } catch (RuntimeException e) {
            return ResponseEntity.status(HttpStatus.BAD_REQUEST)
                    .body(java.util.Map.of("message", e.getMessage()));
        }
    }

    @Operation(
            summary = "Update a post",
            description = "Update an existing post by ID. Only the creator can update.",
            security = @SecurityRequirement(name = "bearerAuth")
    )
    @ApiResponses(value = {
            @ApiResponse(responseCode = "200", description = "Post updated successfully", content = @Content(schema = @Schema(implementation = Post.class))),
            @ApiResponse(responseCode = "404", description = "Post not found"),
            @ApiResponse(responseCode = "400", description = "Invalid input data"),
            @ApiResponse(responseCode = "403", description = "You can only update your own posts")
    })
    @PutMapping("/{id}")
    public ResponseEntity<?> updatePost(
            @Parameter(description = "ID of the post to update", required = true) @PathVariable String id,
            @Valid @RequestBody UpdatePostRequest request,
            @RequestHeader("Authorization") String authHeader) {
        try {
            String token = extractToken(authHeader);
            String userId = jwtUtil.getUserIdFromToken(token);
            
            Post post = new Post();
            post.setContent(request.getContent());
            
            Post updatedPost = postService.updatePost(id, post, userId);
            return ResponseEntity.ok(updatedPost);
        } catch (RuntimeException e) {
            if (e.getMessage().contains("not found")) {
                return ResponseEntity.notFound().build();
            }
            return ResponseEntity.status(HttpStatus.FORBIDDEN)
                    .body(java.util.Map.of("message", e.getMessage()));
        }
    }

    @Operation(
            summary = "Delete a post",
            description = "Delete a post by ID. Only the creator can delete.",
            security = @SecurityRequirement(name = "bearerAuth")
    )
    @ApiResponses(value = {
            @ApiResponse(responseCode = "204", description = "Post deleted successfully"),
            @ApiResponse(responseCode = "404", description = "Post not found"),
            @ApiResponse(responseCode = "403", description = "You can only delete your own posts")
    })
    @DeleteMapping("/{id}")
    public ResponseEntity<?> deletePost(
            @Parameter(description = "ID of the post to delete", required = true) @PathVariable String id,
            @RequestHeader("Authorization") String authHeader) {
        try {
            String token = extractToken(authHeader);
            String userId = jwtUtil.getUserIdFromToken(token);
            
            postService.deletePost(id, userId);
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




