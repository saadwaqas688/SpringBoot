namespace LMS_MVC.DTOs;

public class LessonProgressDto
{
    public string UserId { get; set; } = string.Empty;
    public int LessonId { get; set; }
    public int CourseId { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime? CompletedAt { get; set; }
}

public class UpdateLessonProgressDto
{
    public bool IsCompleted { get; set; }
}

