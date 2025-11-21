namespace LMS.API.DTOs;

public class LessonProgressDto
{
    public string Id { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string LessonId { get; set; } = string.Empty;
    public string CourseId { get; set; } = string.Empty;
    public bool IsCompleted { get; set; }
    public DateTime? CompletedAt { get; set; }
}

public class UpdateLessonProgressDto
{
    public bool IsCompleted { get; set; }
}

