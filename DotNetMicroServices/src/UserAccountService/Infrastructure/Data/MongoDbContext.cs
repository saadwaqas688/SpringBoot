using MongoDB.Driver;
using UserAccountService.Models;

namespace UserAccountService.Infrastructure.Data;

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
        catch (MongoCommandException ex) when (ex.CodeName == "IndexOptionsConflict" || ex.CodeName == "IndexKeySpecsConflict" || ex.CodeName == "IndexAlreadyExists")
        {
            // Index already exists or conflicts, which is fine
            // We can ignore this error
        }
        catch (MongoWriteException ex) when (ex.WriteError?.Code == 85 || ex.WriteError?.Code == 86)
        {
            // Index already exists (error codes 85/86)
            // We can ignore this error
        }
    }
}

