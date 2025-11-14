package com.example.todo.dto;

import io.swagger.v3.oas.annotations.media.Schema;
import jakarta.validation.constraints.NotBlank;
import jakarta.validation.constraints.NotEmpty;

import java.util.List;

@Schema(description = "Request to enroll users to a course")
public class EnrollUsersRequest {

    @NotBlank(message = "Course ID is required")
    @Schema(description = "ID of the course", example = "507f1f77bcf86cd799439011", required = true)
    private String courseId;

    @NotEmpty(message = "At least one user ID is required")
    @Schema(description = "List of user IDs to enroll", example = "[\"507f1f77bcf86cd799439011\", \"507f1f77bcf86cd799439012\"]", required = true)
    private List<String> userIds;

    // Getters and Setters
    public String getCourseId() {
        return courseId;
    }

    public void setCourseId(String courseId) {
        this.courseId = courseId;
    }

    public List<String> getUserIds() {
        return userIds;
    }

    public void setUserIds(List<String> userIds) {
        this.userIds = userIds;
    }
}








