using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CoursesService.Models;

public class UserLessonProgress
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("userId")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string UserId { get; set; } = string.Empty;

    [BsonElement("lessonId")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string LessonId { get; set; } = string.Empty;

    [BsonElement("isCompleted")]
    public bool IsCompleted { get; set; } = false;

    [BsonElement("startedAt")]
    public DateTime? StartedAt { get; set; }

    [BsonElement("completedAt")]
    public DateTime? CompletedAt { get; set; }
}

