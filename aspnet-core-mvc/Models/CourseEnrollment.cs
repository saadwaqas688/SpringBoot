using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CourseManagementAPI.Models;

public class CourseEnrollment
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("courseId")]
    public string CourseId { get; set; } = string.Empty;

    [BsonElement("userId")]
    public string UserId { get; set; } = string.Empty;

    [BsonElement("grantedBy")]
    public string? GrantedBy { get; set; }

    [BsonElement("enrolledAt")]
    public DateTime EnrolledAt { get; set; } = DateTime.UtcNow;
}

