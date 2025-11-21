namespace LMS_MVC.DTOs;

public class EnrollmentDto
{
    public string UserId { get; set; } = string.Empty;
    public string? UserName { get; set; }
    public int CourseId { get; set; }
    public string? CourseTitle { get; set; }
    public DateTime EnrolledAt { get; set; }
}

public class EnrollUserDto
{
    public string UserId { get; set; } = string.Empty;
    public int CourseId { get; set; }
}

