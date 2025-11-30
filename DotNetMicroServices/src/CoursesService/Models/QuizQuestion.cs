using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CoursesService.Models;

public class QuizQuestion
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("quizId")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string QuizId { get; set; } = string.Empty;

    [BsonElement("question")]
    public string Question { get; set; } = string.Empty;

    [BsonElement("options")]
    public List<QuizOption> Options { get; set; } = new();

    [BsonElement("order")]
    public int Order { get; set; }

    [BsonElement("type")]
    public string Type { get; set; } = "quiz";
}

public class QuizOption
{
    [BsonElement("value")]
    public string Value { get; set; } = string.Empty;

    [BsonElement("isCorrect")]
    public bool IsCorrect { get; set; }
}

