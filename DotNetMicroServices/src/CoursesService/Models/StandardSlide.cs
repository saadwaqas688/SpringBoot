using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CoursesService.Models;

public class StandardSlide
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("lessonId")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string LessonId { get; set; } = string.Empty;

    [BsonElement("type")]
    public string Type { get; set; } = "text"; // "video" | "image" | "text"

    [BsonElement("content")]
    public SlideContent Content { get; set; } = new();

    [BsonElement("order")]
    public int Order { get; set; }

    [BsonElement("title")]
    public string? Title { get; set; }
}

public class SlideContent
{
    [BsonElement("title")]
    public string? Title { get; set; }

    [BsonElement("items")]
    public List<SlideContentItem> Items { get; set; } = new();
}

public class SlideContentItem
{
    [BsonElement("text")]
    public string? Text { get; set; }

    [BsonElement("image")]
    public string? Image { get; set; }

    [BsonElement("video")]
    public string? Video { get; set; }

    [BsonElement("thumbnail")]
    public string? Thumbnail { get; set; }
}

