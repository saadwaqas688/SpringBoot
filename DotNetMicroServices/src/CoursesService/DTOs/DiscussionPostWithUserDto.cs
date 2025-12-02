namespace CoursesService.DTOs;

public class DiscussionPostWithUserDto
{
    public string? Id { get; set; }
    public string LessonId { get; set; } = string.Empty;
    public string? DiscussionId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string? ParentPostId { get; set; }
    public string Content { get; set; } = string.Empty;
    public List<string> Attachments { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    // User information
    public string? UserName { get; set; }
    public string? UserEmail { get; set; }
    public string? UserImage { get; set; }
}


