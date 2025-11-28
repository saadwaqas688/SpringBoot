using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CoursesService.Models;

public class UserQuizAnswer
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("attemptId")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string AttemptId { get; set; } = string.Empty;

    [BsonElement("questionId")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string QuestionId { get; set; } = string.Empty;

    [BsonElement("selectedOption")]
    public string SelectedOption { get; set; } = string.Empty;

    [BsonElement("isCorrect")]
    public bool IsCorrect { get; set; } = false;
}

