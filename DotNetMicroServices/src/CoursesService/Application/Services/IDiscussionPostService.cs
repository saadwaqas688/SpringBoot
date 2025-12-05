using CoursesService.Models;
using CoursesService.DTOs;
using Shared.Core.Common;

namespace CoursesService.Services;

public interface IDiscussionPostService
{
    Task<ApiResponse<List<DiscussionPostWithUserDto>>> GetPostsByLessonAsync(string lessonId, int page = 1, int pageSize = 10);
    Task<ApiResponse<DiscussionPost>> GetPostByIdAsync(string postId);
    Task<ApiResponse<DiscussionPost>> CreatePostAsync(DiscussionPost post);
    Task<ApiResponse<DiscussionPost>> UpdatePostAsync(string postId, DiscussionPost post);
    Task<ApiResponse<bool>> DeletePostAsync(string postId);
    Task<ApiResponse<List<DiscussionPostWithUserDto>>> GetCommentsAsync(string postId, int page = 1, int pageSize = 10);
}





