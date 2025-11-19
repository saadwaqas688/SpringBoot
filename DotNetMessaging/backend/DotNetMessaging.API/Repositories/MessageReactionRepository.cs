using MongoDB.Driver;
using DotNetMessaging.API.Data;
using DotNetMessaging.API.Models;

namespace DotNetMessaging.API.Repositories;

public class MessageReactionRepository : Repository<MessageReaction>, IMessageReactionRepository
{
    public MessageReactionRepository(MongoDbContext context) : base(context, "messageReactions")
    {
        // Create compound unique index
        var indexKeys = Builders<MessageReaction>.IndexKeys
            .Ascending(mr => mr.MessageId)
            .Ascending(mr => mr.UserId)
            .Ascending(mr => mr.Emoji);
        var indexOptions = new CreateIndexOptions { Unique = true };
        _collection.Indexes.CreateOne(new CreateIndexModel<MessageReaction>(indexKeys, indexOptions));
    }

    public async Task<IEnumerable<MessageReaction>> GetMessageReactionsAsync(string messageId)
    {
        var filter = Builders<MessageReaction>.Filter.Eq(mr => mr.MessageId, messageId);
        return await _collection.Find(filter).ToListAsync();
    }

    public async Task<MessageReaction?> GetReactionAsync(string messageId, string userId, string emoji)
    {
        var filter = Builders<MessageReaction>.Filter.And(
            Builders<MessageReaction>.Filter.Eq(mr => mr.MessageId, messageId),
            Builders<MessageReaction>.Filter.Eq(mr => mr.UserId, userId),
            Builders<MessageReaction>.Filter.Eq(mr => mr.Emoji, emoji)
        );
        
        return await _collection.Find(filter).FirstOrDefaultAsync();
    }
}

