namespace LMS_MVC.DTOs;

public class DiscussionPostDto
{
    public int Id { get; set; }
    public int LessonId { get; set; }
    public int ContentId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string? UserName { get; set; }
    public string Content { get; set; } = string.Empty;
    public int? ParentPostId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<DiscussionPostDto> Replies { get; set; } = new();
}

public class CreateDiscussionPostDto
{
    public int LessonId { get; set; }
    public int ContentId { get; set; }
    public string Content { get; set; } = string.Empty;
    public int? ParentPostId { get; set; }
}

public class UpdateDiscussionPostDto
{
    public string Content { get; set; } = string.Empty;
}

