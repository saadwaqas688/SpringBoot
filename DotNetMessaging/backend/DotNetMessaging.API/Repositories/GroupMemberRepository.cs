using MongoDB.Driver;
using DotNetMessaging.API.Data;
using DotNetMessaging.API.Models;

namespace DotNetMessaging.API.Repositories;

public class GroupMemberRepository : Repository<GroupMember>, IGroupMemberRepository
{
    public GroupMemberRepository(MongoDbContext context) : base(context, "groupMembers")
    {
        // Create compound unique index
        var indexKeys = Builders<GroupMember>.IndexKeys
            .Ascending(gm => gm.GroupId)
            .Ascending(gm => gm.UserId);
        var indexOptions = new CreateIndexOptions { Unique = true };
        _collection.Indexes.CreateOne(new CreateIndexModel<GroupMember>(indexKeys, indexOptions));
    }

    public async Task<GroupMember?> GetMemberAsync(string groupId, string userId)
    {
        var filter = Builders<GroupMember>.Filter.And(
            Builders<GroupMember>.Filter.Eq(gm => gm.GroupId, groupId),
            Builders<GroupMember>.Filter.Eq(gm => gm.UserId, userId)
        );
        
        return await _collection.Find(filter).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<GroupMember>> GetGroupMembersAsync(string groupId)
    {
        var filter = Builders<GroupMember>.Filter.Eq(gm => gm.GroupId, groupId);
        return await _collection.Find(filter).ToListAsync();
    }

    public async Task<bool> IsMemberAsync(string groupId, string userId)
    {
        var member = await GetMemberAsync(groupId, userId);
        return member != null;
    }

    public async Task<bool> IsAdminAsync(string groupId, string userId)
    {
        var member = await GetMemberAsync(groupId, userId);
        return member != null && member.Role == GroupRole.Admin;
    }
}

