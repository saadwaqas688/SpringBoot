namespace LMS_MVC.Models;

public class LessonProgress
{
    public string UserId { get; set; } = string.Empty;
    public int LessonId { get; set; }
    public int CourseId { get; set; }
    public bool IsCompleted { get; set; } = false;
    public DateTime? CompletedAt { get; set; }

    // Navigation properties
    public User? User { get; set; }
    public Lesson? Lesson { get; set; }
    public Course? Course { get; set; }
}

