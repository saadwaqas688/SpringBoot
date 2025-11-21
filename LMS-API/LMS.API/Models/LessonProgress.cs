using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace LMS.API.Models;

public class LessonProgress
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;

    [BsonElement("userId")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string UserId { get; set; } = string.Empty;

    [BsonElement("lessonId")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string LessonId { get; set; } = string.Empty;

    [BsonElement("courseId")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string CourseId { get; set; } = string.Empty;

    [BsonElement("isCompleted")]
    public bool IsCompleted { get; set; } = false;

    [BsonElement("completedAt")]
    public DateTime? CompletedAt { get; set; }
}

