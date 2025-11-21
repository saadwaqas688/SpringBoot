using CourseManagementAPI.Models;
using CourseManagementAPI.Repositories;

namespace CourseManagementAPI.Services;

public class PostService : IPostService
{
    private readonly IPostRepository _postRepository;

    public PostService(IPostRepository postRepository)
    {
        _postRepository = postRepository;
    }

    public async Task<List<Post>> GetPostsByDiscussionIdAsync(string discussionId)
    {
        return await _postRepository.GetByDiscussionIdAsync(discussionId);
    }

    public async Task<Post?> GetPostByIdAsync(string id)
    {
        return await _postRepository.GetByIdAsync(id);
    }

    public async Task<Post> CreatePostAsync(Post post, string userId)
    {
        post.UserId = userId;
        post.CreatedAt = DateTime.UtcNow;
        post.UpdatedAt = DateTime.UtcNow;
        return await _postRepository.CreateAsync(post);
    }

    public async Task<Post> UpdatePostAsync(string id, Post postDetails, string userId)
    {
        var post = await _postRepository.GetByIdAsync(id);
        if (post == null)
        {
            throw new Exception("Post not found with id: " + id);
        }

        if (post.UserId != userId)
        {
            throw new Exception("You can only update your own posts");
        }

        post.Content = postDetails.Content;
        post.UpdatedAt = DateTime.UtcNow;
        return await _postRepository.UpdateAsync(post);
    }

    public async Task DeletePostAsync(string id, string userId)
    {
        var post = await _postRepository.GetByIdAsync(id);
        if (post == null)
        {
            throw new Exception("Post not found with id: " + id);
        }

        if (post.UserId != userId)
        {
            throw new Exception("You can only delete your own posts");
        }

        await _postRepository.DeleteAsync(id);
    }
}

  "_id": "course55",
  "title": "JavaScript Mastery",
  "description": "Complete JS course",
  "thumbnailUrl": "https://cdn.example.com/course.png",
  "category": "Programming",
  "level": "Beginner",
  "createdAt": "2025-01-10T10:00:00Z",
  "updatedAt": "2025-01-10T10:00:00Z"