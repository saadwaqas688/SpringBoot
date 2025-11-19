using MongoDB.Driver;
using DotNetMessaging.API.Models;

namespace DotNetMessaging.API.Data;

public class MongoDbContext
{
    private readonly IMongoDatabase _database;

    public MongoDbContext(IMongoDatabase database)
    {
        _database = database;
    }

    public IMongoCollection<T> GetCollection<T>(string name) where T : Models.BaseEntity
    {
        return _database.GetCollection<T>(name);
    }

    public IMongoCollection<User> Users => _database.GetCollection<User>("users");
    public IMongoCollection<Chat> Chats => _database.GetCollection<Chat>("chats");
    public IMongoCollection<Message> Messages => _database.GetCollection<Message>("messages");
    public IMongoCollection<Group> Groups => _database.GetCollection<Group>("groups");
    public IMongoCollection<GroupMember> GroupMembers => _database.GetCollection<GroupMember>("groupMembers");
    public IMongoCollection<Contact> Contacts => _database.GetCollection<Contact>("contacts");
    public IMongoCollection<MessageReaction> MessageReactions => _database.GetCollection<MessageReaction>("messageReactions");
    public IMongoCollection<MessageRead> MessageReads => _database.GetCollection<MessageRead>("messageReads");
}

