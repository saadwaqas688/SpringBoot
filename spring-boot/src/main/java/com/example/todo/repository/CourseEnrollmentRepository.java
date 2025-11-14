package com.example.todo.repository;

import com.example.todo.model.CourseEnrollment;
import org.springframework.data.mongodb.repository.MongoRepository;
import org.springframework.stereotype.Repository;

import java.util.List;
import java.util.Optional;

@Repository
public interface CourseEnrollmentRepository extends MongoRepository<CourseEnrollment, String> {
    
    List<CourseEnrollment> findByCourseId(String courseId);
    
    List<CourseEnrollment> findByUserId(String userId);
    
    Optional<CourseEnrollment> findByCourseIdAndUserId(String courseId, String userId);
    
    boolean existsByCourseIdAndUserId(String courseId, String userId);
}



