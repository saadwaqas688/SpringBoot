using MongoDB.Bson.Serialization.Attributes;

namespace DotNetMessaging.API.Models;

public class Message : BaseEntity
{
    [BsonElement("chatId")]
    public string? ChatId { get; set; }

    [BsonElement("groupId")]
    public string? GroupId { get; set; }

    [BsonElement("senderId")]
    public string SenderId { get; set; } = string.Empty;

    [BsonElement("content")]
    public string Content { get; set; } = string.Empty;

    [BsonElement("type")]
    [BsonRepresentation(MongoDB.Bson.BsonType.String)]
    public MessageType Type { get; set; } = MessageType.Text;

    [BsonElement("mediaUrl")]
    public string? MediaUrl { get; set; }

    [BsonElement("mediaType")]
    public string? MediaType { get; set; }

    [BsonElement("mediaFileName")]
    public string? MediaFileName { get; set; }

    [BsonElement("mediaSize")]
    public long? MediaSize { get; set; }

    [BsonElement("replyToMessageId")]
    public string? ReplyToMessageId { get; set; }

    [BsonElement("isDeleted")]
    public bool IsDeleted { get; set; } = false;
}

public enum MessageType
{
    Text,
    Image,
    Video,
    Audio,
    Document,
    Location
}
