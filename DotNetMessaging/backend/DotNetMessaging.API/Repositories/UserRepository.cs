using MongoDB.Driver;
using DotNetMessaging.API.Data;
using DotNetMessaging.API.Models;

namespace DotNetMessaging.API.Repositories;

public class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(MongoDbContext context) : base(context, "users")
    {
        // Create indexes
        var indexOptions = new CreateIndexOptions { Unique = true };
        var emailIndex = new IndexKeysDefinitionBuilder<User>().Ascending(u => u.Email);
        var usernameIndex = new IndexKeysDefinitionBuilder<User>().Ascending(u => u.Username);
        
        _collection.Indexes.CreateOne(new CreateIndexModel<User>(emailIndex, indexOptions));
        _collection.Indexes.CreateOne(new CreateIndexModel<User>(usernameIndex, indexOptions));
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await FindOneAsync(u => u.Email == email);
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        return await FindOneAsync(u => u.Username == username);
    }

    public async Task<IEnumerable<User>> SearchUsersAsync(string searchTerm, string excludeUserId)
    {
        var filter = Builders<User>.Filter.And(
            Builders<User>.Filter.Ne(u => u.Id, excludeUserId),
            Builders<User>.Filter.Or(
                Builders<User>.Filter.Regex(u => u.Username, new MongoDB.Bson.BsonRegularExpression(searchTerm, "i")),
                Builders<User>.Filter.Regex(u => u.Email, new MongoDB.Bson.BsonRegularExpression(searchTerm, "i"))
            )
        );
        
        return await _collection.Find(filter).Limit(50).ToListAsync();
    }
}

