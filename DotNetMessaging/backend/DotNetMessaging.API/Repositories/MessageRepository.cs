using MongoDB.Driver;
using DotNetMessaging.API.Data;
using DotNetMessaging.API.Models;

namespace DotNetMessaging.API.Repositories;

public class MessageRepository : Repository<Message>, IMessageRepository
{
    public MessageRepository(MongoDbContext context) : base(context, "messages")
    {
        // Create indexes
        var chatIndex = Builders<Message>.IndexKeys.Ascending(m => m.ChatId);
        var groupIndex = Builders<Message>.IndexKeys.Ascending(m => m.GroupId);
        var createdAtIndex = Builders<Message>.IndexKeys.Descending(m => m.CreatedAt);
        
        _collection.Indexes.CreateOne(new CreateIndexModel<Message>(chatIndex));
        _collection.Indexes.CreateOne(new CreateIndexModel<Message>(groupIndex));
        _collection.Indexes.CreateOne(new CreateIndexModel<Message>(createdAtIndex));
    }

    public async Task<IEnumerable<Message>> GetChatMessagesAsync(string chatId, int skip = 0, int take = 50)
    {
        var filter = Builders<Message>.Filter.And(
            Builders<Message>.Filter.Eq(m => m.ChatId, chatId),
            Builders<Message>.Filter.Eq(m => m.IsDeleted, false)
        );
        
        var sort = Builders<Message>.Sort.Descending(m => m.CreatedAt);
        
        return await _collection.Find(filter)
            .Sort(sort)
            .Skip(skip)
            .Limit(take)
            .ToListAsync();
    }

    public async Task<IEnumerable<Message>> GetGroupMessagesAsync(string groupId, int skip = 0, int take = 50)
    {
        var filter = Builders<Message>.Filter.And(
            Builders<Message>.Filter.Eq(m => m.GroupId, groupId),
            Builders<Message>.Filter.Eq(m => m.IsDeleted, false)
        );
        
        var sort = Builders<Message>.Sort.Descending(m => m.CreatedAt);
        
        return await _collection.Find(filter)
            .Sort(sort)
            .Skip(skip)
            .Limit(take)
            .ToListAsync();
    }

    public async Task<long> GetUnreadCountAsync(string chatId, string userId, HashSet<string>? readMessageIds = null)
    {
        var filter = Builders<Message>.Filter.And(
            Builders<Message>.Filter.Eq(m => m.ChatId, chatId),
            Builders<Message>.Filter.Ne(m => m.SenderId, userId),
            Builders<Message>.Filter.Eq(m => m.IsDeleted, false)
        );

        if (readMessageIds != null && readMessageIds.Any())
        {
            filter = Builders<Message>.Filter.And(
                filter,
                Builders<Message>.Filter.Nin(m => m.Id, readMessageIds)
            );
        }
        
        return await _collection.CountDocumentsAsync(filter);
    }

    public async Task<long> GetGroupUnreadCountAsync(string groupId, string userId, HashSet<string>? readMessageIds = null)
    {
        var filter = Builders<Message>.Filter.And(
            Builders<Message>.Filter.Eq(m => m.GroupId, groupId),
            Builders<Message>.Filter.Ne(m => m.SenderId, userId),
            Builders<Message>.Filter.Eq(m => m.IsDeleted, false)
        );

        if (readMessageIds != null && readMessageIds.Any())
        {
            filter = Builders<Message>.Filter.And(
                filter,
                Builders<Message>.Filter.Nin(m => m.Id, readMessageIds)
            );
        }
        
        return await _collection.CountDocumentsAsync(filter);
    }
}

