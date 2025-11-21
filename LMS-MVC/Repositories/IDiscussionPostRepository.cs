using LMS_MVC.Models;

namespace LMS_MVC.Repositories;

public interface IDiscussionPostRepository
{
    Task<List<DiscussionPost>> GetByContentIdAsync(int contentId);
    Task<List<DiscussionPost>> GetByLessonIdAsync(int lessonId);
    Task<DiscussionPost?> GetByIdAsync(int id);
    Task<DiscussionPost> CreateAsync(DiscussionPost post);
    Task<DiscussionPost> UpdateAsync(DiscussionPost post);
    Task DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
}

