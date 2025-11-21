namespace LMS_MVC.Models;

public class LessonContent
{
    public int Id { get; set; }
    public int LessonId { get; set; }
    public string Type { get; set; } = string.Empty; // slide | video | quiz | discussion
    public int Order { get; set; }
    public string Title { get; set; } = string.Empty;
    
    // JSON data stored as string, will be deserialized in service layer
    public string Data { get; set; } = string.Empty;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public Lesson? Lesson { get; set; }
    public ICollection<DiscussionPost> DiscussionPosts { get; set; } = new List<DiscussionPost>();
}

