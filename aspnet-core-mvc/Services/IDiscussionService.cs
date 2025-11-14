using CourseManagementAPI.Models;

namespace CourseManagementAPI.Services;

public interface IDiscussionService
{
    Task<List<Discussion>> GetAllDiscussionsAsync();
    Task<Discussion?> GetDiscussionByIdAsync(string id);
    Task<List<Discussion>> GetDiscussionsByCourseIdAsync(string courseId);
    Task<Discussion> CreateDiscussionAsync(Discussion discussion, string userId);
    Task<Discussion> UpdateDiscussionAsync(string id, Discussion discussionDetails, string userId);
    Task DeleteDiscussionAsync(string id, string userId);
}



