using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CoursesService.Models;

public class Quiz
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("lessonId")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string LessonId { get; set; } = string.Empty;

    [BsonElement("title")]
    public string Title { get; set; } = string.Empty;

    [BsonElement("description")]
    public string Description { get; set; } = string.Empty;
}

