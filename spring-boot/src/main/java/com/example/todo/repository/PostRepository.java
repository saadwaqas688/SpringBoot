package com.example.todo.repository;

import com.example.todo.model.Post;
import org.springframework.data.mongodb.repository.MongoRepository;
import org.springframework.stereotype.Repository;

import java.util.List;

@Repository
public interface PostRepository extends MongoRepository<Post, String> {
    
    List<Post> findByDiscussionId(String discussionId);
    
    List<Post> findByDiscussionIdOrderByCreatedAtAsc(String discussionId);
}




