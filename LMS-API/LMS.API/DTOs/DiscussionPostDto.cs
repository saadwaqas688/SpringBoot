namespace LMS.API.DTOs;

public class DiscussionPostDto
{
    public string Id { get; set; } = string.Empty;
    public string LessonId { get; set; } = string.Empty;
    public string ContentId { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string? UserName { get; set; }
    public string Content { get; set; } = string.Empty;
    public string? ParentPostId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<DiscussionPostDto> Replies { get; set; } = new();
}

public class CreateDiscussionPostDto
{
    public string LessonId { get; set; } = string.Empty;
    public string ContentId { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string? ParentPostId { get; set; }
}

public class UpdateDiscussionPostDto
{
    public string Content { get; set; } = string.Empty;
}

