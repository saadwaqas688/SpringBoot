using MongoDB.Driver;
using DotNetMessaging.API.Data;
using DotNetMessaging.API.Models;

namespace DotNetMessaging.API.Repositories;

public class GroupRepository : Repository<Group>, IGroupRepository
{
    private readonly MongoDbContext _context;

    public GroupRepository(MongoDbContext context) : base(context, "groups")
    {
        _context = context;
    }

    public async Task<IEnumerable<Group>> GetUserGroupsAsync(string userId)
    {
        // Get group IDs where user is a member
        var memberFilter = Builders<GroupMember>.Filter.Eq(gm => gm.UserId, userId);
        var memberIds = await _context.GroupMembers
            .Find(memberFilter)
            .Project(gm => gm.GroupId)
            .ToListAsync();

        if (!memberIds.Any())
            return new List<Group>();

        var groupFilter = Builders<Group>.Filter.In(g => g.Id, memberIds);
        var sort = Builders<Group>.Sort.Descending(g => g.LastMessageAt).Descending(g => g.CreatedAt);
        
        return await _collection.Find(groupFilter).Sort(sort).ToListAsync();
    }
}

