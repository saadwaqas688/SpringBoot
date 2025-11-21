namespace LMS_MVC.Models;

public class Lesson
{
    public int Id { get; set; }
    public int CourseId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Order { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public Course? Course { get; set; }
    public ICollection<LessonContent> Contents { get; set; } = new List<LessonContent>();
    public ICollection<LessonProgress> LessonProgresses { get; set; } = new List<LessonProgress>();
}

