using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CoursesService.Models;

public class DiscussionPost
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("lessonId")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string LessonId { get; set; } = string.Empty;

    [BsonElement("discussionId")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? DiscussionId { get; set; } // Optional: for discussion-type lessons

    [BsonElement("userId")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string UserId { get; set; } = string.Empty;

    [BsonElement("parentPostId")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? ParentPostId { get; set; } // null for posts, ObjectId for comments

    [BsonElement("content")]
    public string Content { get; set; } = string.Empty;

    [BsonElement("attachments")]
    public List<string> Attachments { get; set; } = new();

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [BsonElement("updatedAt")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

