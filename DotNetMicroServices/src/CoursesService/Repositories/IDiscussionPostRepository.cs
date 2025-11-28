using CoursesService.Models;
using Shared.Repositories;

namespace CoursesService.Repositories;

public interface IDiscussionPostRepository : IBaseRepository<DiscussionPost>
{
    Task<IEnumerable<DiscussionPost>> GetByLessonIdAsync(string lessonId);
    Task<IEnumerable<DiscussionPost>> GetCommentsByPostIdAsync(string postId);
    Task<IEnumerable<DiscussionPost>> GetPostsByLessonIdAsync(string lessonId); // Only posts, not comments
}

