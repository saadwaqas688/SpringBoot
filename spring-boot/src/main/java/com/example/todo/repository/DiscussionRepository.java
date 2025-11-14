package com.example.todo.repository;

import com.example.todo.model.Discussion;
import org.springframework.data.mongodb.repository.MongoRepository;
import org.springframework.stereotype.Repository;

import java.util.List;

@Repository
public interface DiscussionRepository extends MongoRepository<Discussion, String> {
    
    List<Discussion> findByCourseId(String courseId);
    
    List<Discussion> findByCourseIdIn(List<String> courseIds);
}




