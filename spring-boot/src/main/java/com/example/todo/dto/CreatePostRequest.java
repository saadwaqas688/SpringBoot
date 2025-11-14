package com.example.todo.dto;

import io.swagger.v3.oas.annotations.media.Schema;
import jakarta.validation.constraints.NotBlank;
import jakarta.validation.constraints.Size;

@Schema(description = "Request to create a new post")
public class CreatePostRequest {

    @NotBlank(message = "Discussion ID is required")
    @Schema(description = "ID of the discussion this post belongs to", example = "507f1f77bcf86cd799439011", required = true)
    private String discussionId;

    @NotBlank(message = "Course ID is required")
    @Schema(description = "ID of the course this post belongs to", example = "507f1f77bcf86cd799439011", required = true)
    private String courseId;

    @NotBlank(message = "Post content is required")
    @Size(min = 1, max = 5000, message = "Post content must be between 1 and 5000 characters")
    @Schema(description = "Content of the post", example = "This is a great question!", required = true)
    private String content;

    // Getters and Setters
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

    public String getContent() {
        return content;
    }

    public void setContent(String content) {
        this.content = content;
    }
}

