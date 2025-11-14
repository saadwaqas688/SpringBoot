package com.example.todo.model;

import io.swagger.v3.oas.annotations.media.Schema;
import jakarta.validation.constraints.NotBlank;
import jakarta.validation.constraints.Size;
import org.springframework.data.annotation.Id;
import org.springframework.data.mongodb.core.mapping.Document;

import java.time.LocalDateTime;

@Document(collection = "posts")
@Schema(description = "Post entity for discussion posts")
public class Post {

    @Id
    @Schema(description = "Unique identifier of the post", example = "507f1f77bcf86cd799439011", accessMode = Schema.AccessMode.READ_ONLY)
    private String id;

    @NotBlank(message = "Discussion ID is required")
    @Schema(description = "ID of the discussion this post belongs to", example = "507f1f77bcf86cd799439011", required = true)
    private String discussionId;

    @NotBlank(message = "Course ID is required")
    @Schema(description = "ID of the course this post belongs to", example = "507f1f77bcf86cd799439011", required = true)
    private String courseId;

    @NotBlank(message = "User ID is required")
    @Schema(description = "ID of the user who created this post", example = "507f1f77bcf86cd799439011", required = true)
    private String userId;

    @NotBlank(message = "Post content is required")
    @Size(min = 1, max = 5000, message = "Post content must be between 1 and 5000 characters")
    @Schema(description = "Content of the post", example = "This is a great question!", required = true)
    private String content;

    @Schema(description = "Timestamp when the post was created", example = "2025-11-06T12:00:00", accessMode = Schema.AccessMode.READ_ONLY)
    private LocalDateTime createdAt;

    @Schema(description = "Timestamp when the post was last updated", example = "2025-11-06T12:30:00", accessMode = Schema.AccessMode.READ_ONLY)
    private LocalDateTime updatedAt;

    // Constructors
    public Post() {
    }

    public Post(String discussionId, String courseId, String userId, String content) {
        this.discussionId = discussionId;
        this.courseId = courseId;
        this.userId = userId;
        this.content = content;
    }

    // Getters and Setters
    public String getId() {
        return id;
    }

    public void setId(String id) {
        this.id = id;
    }

    public String getDiscussionId() {
        return discussionId;
    }

    public void setDiscussionId(String discussionId) {
        this.discussionId = discussionId;
    }

    public String getCourseId() {
        return courseId;
    }

    public void setCourseId(String courseId) {
        this.courseId = courseId;
    }

    public String getUserId() {
        return userId;
    }

    public void setUserId(String userId) {
        this.userId = userId;
    }

    public String getContent() {
        return content;
    }

    public void setContent(String content) {
        this.content = content;
    }

    public LocalDateTime getCreatedAt() {
        return createdAt;
    }

    public void setCreatedAt(LocalDateTime createdAt) {
        this.createdAt = createdAt;
    }

    public LocalDateTime getUpdatedAt() {
        return updatedAt;
    }

    public void setUpdatedAt(LocalDateTime updatedAt) {
        this.updatedAt = updatedAt;
    }
}




