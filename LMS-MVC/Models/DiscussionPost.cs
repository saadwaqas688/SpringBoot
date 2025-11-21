namespace LMS_MVC.Models;

public class DiscussionPost
{
    public int Id { get; set; }
    public int LessonId { get; set; }
    public int ContentId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public int? ParentPostId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public Lesson? Lesson { get; set; }
    public LessonContent? LessonContent { get; set; }
    public User? User { get; set; }
    public DiscussionPost? ParentPost { get; set; }
    public ICollection<DiscussionPost> Replies { get; set; } = new List<DiscussionPost>();
}

