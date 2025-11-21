using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace LMS.API.Models;

public class UserCourse
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;

    [BsonElement("userId")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string UserId { get; set; } = string.Empty;

    [BsonElement("courseId")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string CourseId { get; set; } = string.Empty;

    [BsonElement("enrolledAt")]
    public DateTime EnrolledAt { get; set; } = DateTime.UtcNow;
}

