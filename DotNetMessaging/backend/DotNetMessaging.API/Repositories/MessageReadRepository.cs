using MongoDB.Driver;
using DotNetMessaging.API.Data;
using DotNetMessaging.API.Models;

namespace DotNetMessaging.API.Repositories;

public class MessageReadRepository : Repository<MessageRead>, IMessageReadRepository
{
    private readonly MongoDbContext _context;

    public MessageReadRepository(MongoDbContext context) : base(context, "messageReads")
    {
        _context = context;
        // Create compound unique index
        var indexKeys = Builders<MessageRead>.IndexKeys
            .Ascending(mr => mr.MessageId)
            .Ascending(mr => mr.UserId);
        var indexOptions = new CreateIndexOptions { Unique = true };
        _collection.Indexes.CreateOne(new CreateIndexModel<MessageRead>(indexKeys, indexOptions));
    }

    public async Task MarkMessagesAsReadAsync(string chatId, string userId)
    {
        // Get all unread messages in the chat
        var messages = await _context.Messages
            .Find(m => m.ChatId == chatId && m.SenderId != userId && !m.IsDeleted)
            .Project(m => m.Id)
            .ToListAsync();

        if (!messages.Any()) return;

        // Get already read message IDs
        var readMessageIds = await GetReadMessageIdsAsync(chatId, userId);
        
        // Filter out already read messages
        var unreadMessageIds = messages.Where(m => !readMessageIds.Contains(m)).ToList();

        if (!unreadMessageIds.Any()) return;

        var readRecords = unreadMessageIds
            .Select(messageId => new MessageRead
            {
                MessageId = messageId,
                UserId = userId,
                ReadAt = DateTime.UtcNow
            })
            .ToList();

        if (readRecords.Any())
        {
            await _collection.InsertManyAsync(readRecords, new InsertManyOptions { IsOrdered = false });
        }
    }

    public async Task MarkGroupMessagesAsReadAsync(string groupId, string userId)
    {
        // Get all unread messages in the group
        var messages = await _context.Messages
            .Find(m => m.GroupId == groupId && m.SenderId != userId && !m.IsDeleted)
            .Project(m => m.Id)
            .ToListAsync();

        if (!messages.Any()) return;

        // Get already read message IDs
        var readMessageIds = await GetReadGroupMessageIdsAsync(groupId, userId);
        
        // Filter out already read messages
        var unreadMessageIds = messages.Where(m => !readMessageIds.Contains(m)).ToList();

        if (!unreadMessageIds.Any()) return;

        var readRecords = unreadMessageIds
            .Select(messageId => new MessageRead
            {
                MessageId = messageId,
                UserId = userId,
                ReadAt = DateTime.UtcNow
            })
            .ToList();

        if (readRecords.Any())
        {
            await _collection.InsertManyAsync(readRecords, new InsertManyOptions { IsOrdered = false });
        }
    }

    public async Task<bool> IsMessageReadAsync(string messageId, string userId)
    {
        var filter = Builders<MessageRead>.Filter.And(
            Builders<MessageRead>.Filter.Eq(mr => mr.MessageId, messageId),
            Builders<MessageRead>.Filter.Eq(mr => mr.UserId, userId)
        );
        var read = await _collection.Find(filter).FirstOrDefaultAsync();
        return read != null;
    }

    public async Task<HashSet<string>> GetReadMessageIdsAsync(string chatId, string userId)
    {
        // Get all message IDs for this chat
        var messageIds = await _context.Messages
            .Find(m => m.ChatId == chatId && !m.IsDeleted)
            .Project(m => m.Id)
            .ToListAsync();

        if (!messageIds.Any()) return new HashSet<string>();

        // Get read message IDs
        var filter = Builders<MessageRead>.Filter.And(
            Builders<MessageRead>.Filter.In(mr => mr.MessageId, messageIds),
            Builders<MessageRead>.Filter.Eq(mr => mr.UserId, userId)
        );

        var readMessages = await _collection.Find(filter)
            .Project(mr => mr.MessageId)
            .ToListAsync();

        return readMessages.ToHashSet();
    }

    public async Task<HashSet<string>> GetReadGroupMessageIdsAsync(string groupId, string userId)
    {
        // Get all message IDs for this group
        var messageIds = await _context.Messages
            .Find(m => m.GroupId == groupId && !m.IsDeleted)
            .Project(m => m.Id)
            .ToListAsync();

        if (!messageIds.Any()) return new HashSet<string>();

        // Get read message IDs
        var filter = Builders<MessageRead>.Filter.And(
            Builders<MessageRead>.Filter.In(mr => mr.MessageId, messageIds),
            Builders<MessageRead>.Filter.Eq(mr => mr.UserId, userId)
        );

        var readMessages = await _collection.Find(filter)
            .Project(mr => mr.MessageId)
            .ToListAsync();

        return readMessages.ToHashSet();
    }
}

