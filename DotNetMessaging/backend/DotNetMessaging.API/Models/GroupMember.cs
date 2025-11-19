using MongoDB.Bson.Serialization.Attributes;

namespace DotNetMessaging.API.Models;

public class GroupMember : BaseEntity
{
    [BsonElement("groupId")]
    public string GroupId { get; set; } = string.Empty;

    [BsonElement("userId")]
    public string UserId { get; set; } = string.Empty;

    [BsonElement("role")]
    [BsonRepresentation(MongoDB.Bson.BsonType.String)]
    public GroupRole Role { get; set; } = GroupRole.Member;

    [BsonElement("joinedAt")]
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
}

public enum GroupRole
{
    Member,
    Admin
}
