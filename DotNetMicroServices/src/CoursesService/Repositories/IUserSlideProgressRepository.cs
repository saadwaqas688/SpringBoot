using CoursesService.Models;
using Shared.Repositories;

namespace CoursesService.Repositories;

public interface IUserSlideProgressRepository : IBaseRepository<UserSlideProgress>
{
    Task<UserSlideProgress?> GetByUserAndSlideAsync(string userId, string slideId);
    Task<IEnumerable<UserSlideProgress>> GetByUserIdAsync(string userId);
}

