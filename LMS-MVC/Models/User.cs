using Microsoft.AspNetCore.Identity;

namespace LMS_MVC.Models;

public class User : IdentityUser
{
    public string Name { get; set; } = string.Empty;
    public string Role { get; set; } = "USER";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public ICollection<Course> CreatedCourses { get; set; } = new List<Course>();
    public ICollection<UserCourse> EnrolledCourses { get; set; } = new List<UserCourse>();
    public ICollection<DiscussionPost> Posts { get; set; } = new List<DiscussionPost>();
    public ICollection<LessonProgress> LessonProgresses { get; set; } = new List<LessonProgress>();
}

