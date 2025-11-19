using MongoDB.Bson.Serialization.Attributes;

namespace DotNetMessaging.API.Models;

public class Group : BaseEntity
{
    [BsonElement("name")]
    public string Name { get; set; } = string.Empty;

    [BsonElement("description")]
    public string? Description { get; set; }

    [BsonElement("profilePictureUrl")]
    public string? ProfilePictureUrl { get; set; }

    [BsonElement("createdById")]
    public string CreatedById { get; set; } = string.Empty;

    [BsonElement("lastMessageAt")]
    public DateTime? LastMessageAt { get; set; }
}
