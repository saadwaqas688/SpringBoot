namespace LMS_MVC.Models;

public class Course
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? ThumbnailUrl { get; set; }
    public string Category { get; set; } = string.Empty;
    public string Level { get; set; } = string.Empty;
    public string CreatedBy { get; set; } = string.Empty; // UserId
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public User? Creator { get; set; }
    public ICollection<UserCourse> UserCourses { get; set; } = new List<UserCourse>();
    public ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();
}

