namespace LMS_MVC.Models;

public class UserCourse
{
    public string UserId { get; set; } = string.Empty;
    public int CourseId { get; set; }
    public DateTime EnrolledAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public User? User { get; set; }
    public Course? Course { get; set; }
}

