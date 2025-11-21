using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace LMS.API.Models;

public class LessonContent
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;

    [BsonElement("lessonId")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string LessonId { get; set; } = string.Empty;

    [BsonElement("type")]
    public string Type { get; set; } = string.Empty; // slide | video | quiz | discussion

    [BsonElement("order")]
    public int Order { get; set; }

    [BsonElement("title")]
    public string Title { get; set; } = string.Empty;

    [BsonElement("data")]
    public string Data { get; set; } = string.Empty; // JSON data stored as string

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [BsonElement("updatedAt")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

