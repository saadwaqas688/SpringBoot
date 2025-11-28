using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CoursesService.Models;

public class UserActivityLog
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("userId")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string UserId { get; set; } = string.Empty;

    [BsonElement("activityType")]
    public string ActivityType { get; set; } = string.Empty; // "course_started", "lesson_completed", "quiz_taken", etc.

    [BsonElement("entityType")]
    public string EntityType { get; set; } = string.Empty; // "course", "lesson", "quiz", "slide", etc.

    [BsonElement("entityId")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string EntityId { get; set; } = string.Empty;

    [BsonElement("metadata")]
    public Dictionary<string, object>? Metadata { get; set; }

    [BsonElement("timestamp")]
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}


