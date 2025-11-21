using LMS_MVC.DTOs;

namespace LMS_MVC.Services;

public interface IDiscussionService
{
    Task<List<DiscussionPostDto>> GetPostsByContentIdAsync(int contentId);
    Task<List<DiscussionPostDto>> GetPostsByLessonIdAsync(int lessonId);
    Task<DiscussionPostDto?> GetPostByIdAsync(int id);
    Task<DiscussionPostDto> CreatePostAsync(CreateDiscussionPostDto dto, string userId);
    Task<DiscussionPostDto> UpdatePostAsync(int id, UpdateDiscussionPostDto dto, string userId);
    Task DeletePostAsync(int id, string userId);
}

