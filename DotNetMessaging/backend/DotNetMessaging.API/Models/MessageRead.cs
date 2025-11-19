using MongoDB.Bson.Serialization.Attributes;

namespace DotNetMessaging.API.Models;

public class MessageRead : BaseEntity
{
    [BsonElement("messageId")]
    public string MessageId { get; set; } = string.Empty;

    [BsonElement("userId")]
    public string UserId { get; set; } = string.Empty;

    [BsonElement("readAt")]
    public DateTime ReadAt { get; set; } = DateTime.UtcNow;
}

