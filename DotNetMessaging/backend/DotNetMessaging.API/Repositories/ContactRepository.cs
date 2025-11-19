using MongoDB.Driver;
using DotNetMessaging.API.Data;
using DotNetMessaging.API.Models;

namespace DotNetMessaging.API.Repositories;

public class ContactRepository : Repository<Contact>, IContactRepository
{
    public ContactRepository(MongoDbContext context) : base(context, "contacts")
    {
        // Create compound unique index
        var indexKeys = Builders<Contact>.IndexKeys
            .Ascending(c => c.UserId)
            .Ascending(c => c.ContactUserId);
        var indexOptions = new CreateIndexOptions { Unique = true };
        _collection.Indexes.CreateOne(new CreateIndexModel<Contact>(indexKeys, indexOptions));
    }

    public async Task<IEnumerable<Contact>> GetUserContactsAsync(string userId)
    {
        var filter = Builders<Contact>.Filter.Eq(c => c.UserId, userId);
        return await _collection.Find(filter).ToListAsync();
    }

    public async Task<Contact?> GetContactAsync(string userId, string contactUserId)
    {
        var filter = Builders<Contact>.Filter.And(
            Builders<Contact>.Filter.Eq(c => c.UserId, userId),
            Builders<Contact>.Filter.Eq(c => c.ContactUserId, contactUserId)
        );
        
        return await _collection.Find(filter).FirstOrDefaultAsync();
    }
}

