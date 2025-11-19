using MongoDB.Bson.Serialization.Attributes;

namespace DotNetMessaging.API.Models;

public class Contact : BaseEntity
{
    [BsonElement("userId")]
    public string UserId { get; set; } = string.Empty;

    [BsonElement("contactUserId")]
    public string ContactUserId { get; set; } = string.Empty;

    [BsonElement("displayName")]
    public string? DisplayName { get; set; }
}
