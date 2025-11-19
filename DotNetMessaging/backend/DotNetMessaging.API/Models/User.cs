using MongoDB.Bson.Serialization.Attributes;

namespace DotNetMessaging.API.Models;

public class User : BaseEntity
{
    [BsonElement("username")]
    public string Username { get; set; } = string.Empty;

    [BsonElement("email")]
    public string Email { get; set; } = string.Empty;

    [BsonElement("passwordHash")]
    public string PasswordHash { get; set; } = string.Empty;

    [BsonElement("profilePictureUrl")]
    public string? ProfilePictureUrl { get; set; }

    [BsonElement("status")]
    public string? Status { get; set; }

    [BsonElement("isOnline")]
    public bool IsOnline { get; set; }

    [BsonElement("lastSeen")]
    public DateTime? LastSeen { get; set; }
}
