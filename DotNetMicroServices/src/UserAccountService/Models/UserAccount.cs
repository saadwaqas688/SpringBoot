using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace UserAccountService.Models;

[BsonIgnoreExtraElements]
public class UserAccount
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }
    [BsonElement("name")]
    public string Name { get; set; } = string.Empty;
    [BsonElement("email")]
    public string Email { get; set; } = string.Empty;
    [BsonElement("passwordHash")]
    public string PasswordHash { get; set; } = string.Empty;
    [BsonElement("image")]
    public string? Image { get; set; }
    [BsonElement("role")]
    public string Role { get; set; } = "user"; // Default role is "user"
    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; }
    [BsonElement("updatedAt")]
    public DateTime UpdatedAt { get; set; }
}

