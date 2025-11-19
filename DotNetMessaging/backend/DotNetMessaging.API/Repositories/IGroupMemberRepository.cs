using DotNetMessaging.API.Models;

namespace DotNetMessaging.API.Repositories;

public interface IGroupMemberRepository : IRepository<GroupMember>
{
    Task<GroupMember?> GetMemberAsync(string groupId, string userId);
    Task<IEnumerable<GroupMember>> GetGroupMembersAsync(string groupId);
    Task<bool> IsMemberAsync(string groupId, string userId);
    Task<bool> IsAdminAsync(string groupId, string userId);
}

