using MongoDB.Bson.Serialization.Attributes;

namespace DotNetMessaging.API.Models;

public class MessageReaction : BaseEntity
{
    [BsonElement("messageId")]
    public string MessageId { get; set; } = string.Empty;

    [BsonElement("userId")]
    public string UserId { get; set; } = string.Empty;

    [BsonElement("emoji")]
    public string Emoji { get; set; } = string.Empty;
}
