using LMS.API.Models;

namespace LMS.API.Repositories;

public interface IDiscussionPostRepository
{
    Task<List<DiscussionPost>> GetAllAsync();
    Task<DiscussionPost?> GetByIdAsync(string id);
    Task<List<DiscussionPost>> GetByLessonIdAsync(string lessonId);
    Task<List<DiscussionPost>> GetByContentIdAsync(string contentId);
    Task<List<DiscussionPost>> GetRepliesAsync(string parentPostId);
    Task<DiscussionPost> CreateAsync(DiscussionPost post);
    Task<DiscussionPost> UpdateAsync(DiscussionPost post);
    Task DeleteAsync(string id);
    Task<bool> ExistsAsync(string id);
}

