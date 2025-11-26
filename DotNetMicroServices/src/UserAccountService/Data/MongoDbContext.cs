using MongoDB.Driver;
using UserAccountService.Models;

namespace UserAccountService.Data;

public class MongoDbContext
{
    private readonly IMongoDatabase _database;

    public MongoDbContext(IMongoClient mongoClient, string databaseName)
    {
        _database = mongoClient.GetDatabase(databaseName);
    }

    public IMongoCollection<UserAccount> UserAccounts => _database.GetCollection<UserAccount>("UserAccounts");

    public async Task CreateIndexesAsync()
    {
        // Create unique index on email
        var indexOptions = new CreateIndexOptions { Unique = true };
        var indexDefinition = Builders<UserAccount>.IndexKeys.Ascending(u => u.Email);
        var indexModel = new CreateIndexModel<UserAccount>(indexDefinition, indexOptions);
        
        try
        {
            await UserAccounts.Indexes.CreateOneAsync(indexModel);
        }
        catch
        {
            // Index might already exist, ignore
        }
    }
}

