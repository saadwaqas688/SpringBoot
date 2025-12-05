using CoursesService.Models;
using Shared.Core.Common;
using Shared.Infrastructure.Repositories;

namespace CoursesService.Repositories;

public interface IUserRepository : IBaseRepository<User>
{
    Task<PagedResponse<User>> GetAllNonAdminUsersAsync(int page, int pageSize, string? searchTerm = null);
}


