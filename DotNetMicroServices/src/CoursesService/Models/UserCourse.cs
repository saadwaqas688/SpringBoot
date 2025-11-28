using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CoursesService.Models;

public class UserCourse
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("userId")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string UserId { get; set; } = string.Empty;

    [BsonElement("courseId")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string CourseId { get; set; } = string.Empty;

    [BsonElement("progress")]
    public int Progress { get; set; } = 0; // 0-100

    [BsonElement("status")]
    public string Status { get; set; } = "not_started"; // "not_started" | "in_progress" | "completed"

    [BsonElement("assignedAt")]
    public DateTime AssignedAt { get; set; } = DateTime.UtcNow;

    [BsonElement("completedAt")]
    public DateTime? CompletedAt { get; set; }
}

