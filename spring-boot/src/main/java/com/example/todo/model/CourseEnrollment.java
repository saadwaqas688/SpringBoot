package com.example.todo.model;

import io.swagger.v3.oas.annotations.media.Schema;
import org.springframework.data.annotation.Id;
import org.springframework.data.mongodb.core.mapping.Document;

import java.time.LocalDateTime;

@Document(collection = "course_enrollments")
@Schema(description = "Course enrollment entity")
public class CourseEnrollment {

    @Id
    @Schema(description = "Unique identifier of the enrollment", example = "507f1f77bcf86cd799439011", accessMode = Schema.AccessMode.READ_ONLY)
    private String id;

    @Schema(description = "ID of the course", example = "507f1f77bcf86cd799439011", required = true)
    private String courseId;

    @Schema(description = "ID of the user", example = "507f1f77bcf86cd799439011", required = true)
    private String userId;

    @Schema(description = "ID of the user who granted access (admin)", example = "507f1f77bcf86cd799439011")
    private String grantedBy;

    @Schema(description = "Timestamp when the enrollment was created", example = "2025-11-06T12:00:00", accessMode = Schema.AccessMode.READ_ONLY)
    private LocalDateTime enrolledAt;

    // Constructors
    public CourseEnrollment() {
    }

    public CourseEnrollment(String courseId, String userId, String grantedBy) {
        this.courseId = courseId;
        this.userId = userId;
        this.grantedBy = grantedBy;
    }

    // Getters and Setters
    public String getId() {
        return id;
    }

    public void setId(String id) {
        this.id = id;
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

    public String getGrantedBy() {
        return grantedBy;
    }

    public void setGrantedBy(String grantedBy) {
        this.grantedBy = grantedBy;
    }

    public LocalDateTime getEnrolledAt() {
        return enrolledAt;
    }

    public void setEnrolledAt(LocalDateTime enrolledAt) {
        this.enrolledAt = enrolledAt;
    }
}








