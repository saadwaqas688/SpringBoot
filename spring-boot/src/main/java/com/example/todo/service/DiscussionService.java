package com.example.todo.service;

import com.example.todo.model.Discussion;
import com.example.todo.repository.DiscussionRepository;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Service;

import java.time.LocalDateTime;
import java.util.List;
import java.util.Optional;

/**
 * Service for handling course discussion operations with complex aggregations
 */
@Service
public class DiscussionService {
    
    @Autowired
    private DiscussionRepository discussionRepository;
    
    public List<Discussion> getAllDiscussions() {
        return discussionRepository.findAll();
    }
    
    public Optional<Discussion> getDiscussionById(String id) {
        return discussionRepository.findById(id);
    }
    
    public List<Discussion> getDiscussionsByCourseId(String courseId) {
        return discussionRepository.findByCourseId(courseId);
    }

    public List<Discussion> getDiscussionsByCourseIds(List<String> courseIds) {
        return discussionRepository.findByCourseIdIn(courseIds);
    }
    
    public Discussion createDiscussion(Discussion discussion, String userId) {
        discussion.setCreatedBy(userId);
        discussion.setCreatedAt(LocalDateTime.now());
        discussion.setUpdatedAt(LocalDateTime.now());
        return discussionRepository.save(discussion);
    }
    
    public Discussion updateDiscussion(String id, Discussion discussionDetails, String userId) {
        Discussion discussion = discussionRepository.findById(id)
                .orElseThrow(() -> new RuntimeException("Discussion not found with id: " + id));

        // Check ownership (only creator can update)
        if (discussion.getCreatedBy() != null && !discussion.getCreatedBy().equals(userId)) {
            throw new RuntimeException("You can only update your own discussions");
        }

        discussion.setCourseId(discussionDetails.getCourseId());
        discussion.setTitle(discussionDetails.getTitle());
        discussion.setDescription(discussionDetails.getDescription());
        discussion.setUpdatedAt(LocalDateTime.now());

        return discussionRepository.save(discussion);
    }
    
    public void deleteDiscussion(String id, String userId) {
        Discussion discussion = discussionRepository.findById(id)
                .orElseThrow(() -> new RuntimeException("Discussion not found with id: " + id));

        // Check ownership (only creator can delete)
        if (discussion.getCreatedBy() != null && !discussion.getCreatedBy().equals(userId)) {
            throw new RuntimeException("You can only delete your own discussions");
        }

        discussionRepository.deleteById(id);
    }
}
