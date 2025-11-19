using MongoDB.Driver;
using DotNetMessaging.API.Data;
using DotNetMessaging.API.Models;

namespace DotNetMessaging.API.Repositories;

public class ChatRepository : Repository<Chat>, IChatRepository
{
    public ChatRepository(MongoDbContext context) : base(context, "chats")
    {
        // Create compound index for user pairs
        var indexKeys = Builders<Chat>.IndexKeys
            .Ascending(c => c.User1Id)
            .Ascending(c => c.User2Id);
        var indexOptions = new CreateIndexOptions { Unique = true };
        _collection.Indexes.CreateOne(new CreateIndexModel<Chat>(indexKeys, indexOptions));
    }

    public async Task<Chat?> GetChatBetweenUsersAsync(string user1Id, string user2Id)
    {
        var filter = Builders<Chat>.Filter.Or(
            Builders<Chat>.Filter.And(
                Builders<Chat>.Filter.Eq(c => c.User1Id, user1Id),
                Builders<Chat>.Filter.Eq(c => c.User2Id, user2Id)
            ),
            Builders<Chat>.Filter.And(
                Builders<Chat>.Filter.Eq(c => c.User1Id, user2Id),
                Builders<Chat>.Filter.Eq(c => c.User2Id, user1Id)
            )
        );
        
        return await _collection.Find(filter).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<Chat>> GetUserChatsAsync(string userId)
    {
        var filter = Builders<Chat>.Filter.Or(
            Builders<Chat>.Filter.Eq(c => c.User1Id, userId),
            Builders<Chat>.Filter.Eq(c => c.User2Id, userId)
        );
        
        var sort = Builders<Chat>.Sort.Descending(c => c.LastMessageAt).Descending(c => c.CreatedAt);
        
        return await _collection.Find(filter).Sort(sort).ToListAsync();
    }
}

