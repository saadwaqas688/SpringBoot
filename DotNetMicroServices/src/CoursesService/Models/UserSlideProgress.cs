using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CoursesService.Models;

public class UserSlideProgress
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("userId")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string UserId { get; set; } = string.Empty;

    [BsonElement("slideId")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string SlideId { get; set; } = string.Empty;

    [BsonElement("isViewed")]
    public bool IsViewed { get; set; } = false;

    [BsonElement("isCompleted")]
    public bool IsCompleted { get; set; } = false;

    [BsonElement("viewedAt")]
    public DateTime? ViewedAt { get; set; }

    [BsonElement("completedAt")]
    public DateTime? CompletedAt { get; set; }

    [BsonElement("timeSpent")]
    public int TimeSpent { get; set; } = 0; // in seconds
}


