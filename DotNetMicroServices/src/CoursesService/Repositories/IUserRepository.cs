using CoursesService.Models;
using Shared.Common;
using Shared.Repositories;

namespace CoursesService.Repositories;

public interface IUserRepository : IBaseRepository<User>
{
    Task<PagedResponse<User>> GetAllNonAdminUsersAsync(int page, int pageSize, string? searchTerm = null);
}

