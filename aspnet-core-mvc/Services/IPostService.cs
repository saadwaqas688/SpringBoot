using CourseManagementAPI.Models;

namespace CourseManagementAPI.Services;

public interface IPostService
{
    Task<List<Post>> GetPostsByDiscussionIdAsync(string discussionId);
    Task<Post?> GetPostByIdAsync(string id);
    Task<Post> CreatePostAsync(Post post, string userId);
    Task<Post> UpdatePostAsync(string id, Post postDetails, string userId);
    Task DeletePostAsync(string id, string userId);
}

