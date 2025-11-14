using CourseManagementAPI.Models;

namespace CourseManagementAPI.Repositories;

public interface IPostRepository
{
    Task<List<Post>> GetByDiscussionIdAsync(string discussionId);
    Task<Post?> GetByIdAsync(string id);
    Task<Post> CreateAsync(Post post);
    Task<Post> UpdateAsync(Post post);
    Task DeleteAsync(string id);
}

