using CoursesService.Models;
using CoursesService.DTOs;
using Shared.Repositories;

namespace CoursesService.Repositories;

public interface IDiscussionPostRepository : IBaseRepository<DiscussionPost>
{
    Task<IEnumerable<DiscussionPost>> GetByLessonIdAsync(string lessonId);
    Task<IEnumerable<DiscussionPost>> GetCommentsByPostIdAsync(string postId);
    Task<IEnumerable<DiscussionPost>> GetPostsByLessonIdAsync(string lessonId); // Only posts, not comments
    Task<IEnumerable<DiscussionPost>> GetPostsByDiscussionIdAsync(string discussionId); // Only posts (ParentPostId = null) for a discussion
    Task<int> GetUniqueUserCountByDiscussionIdAsync(string discussionId); // Count unique users who contributed to a discussion
    
    // Methods with user population using aggregation
    Task<IEnumerable<DiscussionPostWithUserDto>> GetPostsByLessonIdWithUsersAsync(string lessonId);
    Task<IEnumerable<DiscussionPostWithUserDto>> GetCommentsByPostIdWithUsersAsync(string postId);
    Task<IEnumerable<DiscussionPostWithUserDto>> GetPostsByDiscussionIdWithUsersAsync(string discussionId);
}

