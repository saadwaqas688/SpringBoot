using DotNetMessaging.API.Models;

namespace DotNetMessaging.API.Repositories;

public interface IGroupRepository : IRepository<Group>
{
    Task<IEnumerable<Group>> GetUserGroupsAsync(string userId);
}

