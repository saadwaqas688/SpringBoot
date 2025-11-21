using LMS.API.DTOs;

namespace LMS.API.Services;

public interface IDiscussionService
{
    Task<List<DiscussionPostDto>> GetPostsByContentIdAsync(string contentId);
    Task<DiscussionPostDto> CreatePostAsync(CreateDiscussionPostDto dto, string userId);
    Task<DiscussionPostDto> UpdatePostAsync(string id, UpdateDiscussionPostDto dto, string userId);
    Task DeletePostAsync(string id, string userId);
}

