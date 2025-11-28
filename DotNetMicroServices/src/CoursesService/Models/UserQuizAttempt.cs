using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CoursesService.Models;

public class UserQuizAttempt
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("userId")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string UserId { get; set; } = string.Empty;

    [BsonElement("quizId")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string QuizId { get; set; } = string.Empty;

    [BsonElement("score")]
    public int Score { get; set; } = 0;

    [BsonElement("totalQuestions")]
    public int TotalQuestions { get; set; } = 0;

    [BsonElement("correctAnswers")]
    public int CorrectAnswers { get; set; } = 0;

    [BsonElement("attemptedAt")]
    public DateTime AttemptedAt { get; set; } = DateTime.UtcNow;
}

