package com.example.todo.service;

import com.example.todo.dto.EnrolledUserDto;
import com.example.todo.model.Course;
import com.example.todo.model.CourseEnrollment;
import com.example.todo.model.User;
import com.example.todo.repository.CourseEnrollmentRepository;
import com.example.todo.repository.CourseRepository;
import com.example.todo.repository.UserRepository;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Service;

import java.time.LocalDateTime;
import java.util.ArrayList;
import java.util.List;
import java.util.stream.Collectors;

@Service
public class EnrollmentService {

    private final CourseEnrollmentRepository enrollmentRepository;
    private final CourseRepository courseRepository;
    private final UserRepository userRepository;

    @Autowired
    public EnrollmentService(
            CourseEnrollmentRepository enrollmentRepository,
            CourseRepository courseRepository,
            UserRepository userRepository) {
        this.enrollmentRepository = enrollmentRepository;
        this.courseRepository = courseRepository;
        this.userRepository = userRepository;
    }

    public List<CourseEnrollment> enrollUsers(String courseId, List<String> userIds, String grantedBy) {
        // Verify course exists
        Course course = courseRepository.findById(courseId)
                .orElseThrow(() -> new RuntimeException("Course not found with id: " + courseId));

        List<CourseEnrollment> enrollments = new ArrayList<>();

        for (String userId : userIds) {
            // Verify user exists
            User user = userRepository.findById(userId)
                    .orElseThrow(() -> new RuntimeException("User not found with id: " + userId));

            // Check if already enrolled
            if (enrollmentRepository.existsByCourseIdAndUserId(courseId, userId)) {
                continue; // Skip if already enrolled
            }

            // Create enrollment
            CourseEnrollment enrollment = new CourseEnrollment();
            enrollment.setCourseId(courseId);
            enrollment.setUserId(userId);
            enrollment.setGrantedBy(grantedBy);
            enrollment.setEnrolledAt(LocalDateTime.now());

            enrollments.add(enrollmentRepository.save(enrollment));
        }

        return enrollments;
    }

    public List<EnrolledUserDto> getEnrolledUsers(String courseId) {
        // Verify course exists
        courseRepository.findById(courseId)
                .orElseThrow(() -> new RuntimeException("Course not found with id: " + courseId));

        List<CourseEnrollment> enrollments = enrollmentRepository.findByCourseId(courseId);

        return enrollments.stream()
                .map(enrollment -> {
                    User user = userRepository.findById(enrollment.getUserId())
                            .orElse(null);

                    if (user == null) {
                        return null;
                    }

                    EnrolledUserDto dto = new EnrolledUserDto();
                    dto.setEnrollmentId(enrollment.getId());
                    dto.setUserId(user.getId());
                    dto.setEmail(user.getEmail());
                    dto.setFirstName(user.getFirstName());
                    dto.setLastName(user.getLastName());
                    dto.setGrantedBy(enrollment.getGrantedBy());
                    dto.setEnrolledAt(enrollment.getEnrolledAt());
                    return dto;
                })
                .filter(dto -> dto != null)
                .collect(Collectors.toList());
    }

    public boolean isUserEnrolled(String courseId, String userId) {
        return enrollmentRepository.existsByCourseIdAndUserId(courseId, userId);
    }

    public List<Course> getEnrolledCourses(String userId) {
        List<CourseEnrollment> enrollments = enrollmentRepository.findByUserId(userId);
        List<String> courseIds = enrollments.stream()
                .map(CourseEnrollment::getCourseId)
                .collect(Collectors.toList());
        
        return courseRepository.findAllById(courseIds);
    }
}



