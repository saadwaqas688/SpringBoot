package com.example.todo.service;

import com.example.todo.model.Post;
import com.example.todo.repository.PostRepository;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Service;

import java.time.LocalDateTime;
import java.util.List;
import java.util.Optional;

@Service
public class PostService {

    private final PostRepository postRepository;

    @Autowired
    public PostService(PostRepository postRepository) {
        this.postRepository = postRepository;
    }

    public List<Post> getPostsByDiscussionId(String discussionId) {
        return postRepository.findByDiscussionIdOrderByCreatedAtAsc(discussionId);
    }

    public Optional<Post> getPostById(String id) {
        return postRepository.findById(id);
    }

    public Post createPost(Post post, String userId) {
        post.setUserId(userId);
        post.setCreatedAt(LocalDateTime.now());
        post.setUpdatedAt(LocalDateTime.now());
        return postRepository.save(post);
    }

    public Post updatePost(String id, Post postDetails, String userId) {
        Post post = postRepository.findById(id)
                .orElseThrow(() -> new RuntimeException("Post not found with id: " + id));

        // Check ownership (only creator can update)
        if (!post.getUserId().equals(userId)) {
            throw new RuntimeException("You can only update your own posts");
        }

        post.setContent(postDetails.getContent());
        post.setUpdatedAt(LocalDateTime.now());

        return postRepository.save(post);
    }

    public void deletePost(String id, String userId) {
        Post post = postRepository.findById(id)
                .orElseThrow(() -> new RuntimeException("Post not found with id: " + id));

        // Check ownership (only creator can delete)
        if (!post.getUserId().equals(userId)) {
            throw new RuntimeException("You can only delete your own posts");
        }

        postRepository.deleteById(id);
    }
}




