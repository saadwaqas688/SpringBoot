using MongoDB.Bson.Serialization.Attributes;

namespace DotNetMessaging.API.Models;

public class Chat : BaseEntity
{
    [BsonElement("user1Id")]
    public string User1Id { get; set; } = string.Empty;

    [BsonElement("user2Id")]
    public string User2Id { get; set; } = string.Empty;

    [BsonElement("lastMessageAt")]
    public DateTime? LastMessageAt { get; set; }
}
