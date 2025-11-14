package com.example.todo.dto;

import io.swagger.v3.oas.annotations.media.Schema;
import jakarta.validation.constraints.NotBlank;
import jakarta.validation.constraints.Size;

@Schema(description = "Request to update a post")
public class UpdatePostRequest {

    @NotBlank(message = "Post content is required")
    @Size(min = 1, max = 5000, message = "Post content must be between 1 and 5000 characters")
    @Schema(description = "Content of the post", example = "This is an updated post!", required = true)
    private String content;

    // Getters and Setters
    public String getContent() {
        return content;
    }

    public void setContent(String content) {
        this.content = content;
    }
}

