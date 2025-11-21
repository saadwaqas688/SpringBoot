namespace LMS.API.DTOs;

public class EnrollmentDto
{
    public string Id { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string? UserName { get; set; }
    public string CourseId { get; set; } = string.Empty;
    public string? CourseTitle { get; set; }
    public DateTime EnrolledAt { get; set; }
}

public class EnrollUserDto
{
    public string UserId { get; set; } = string.Empty;
    public string CourseId { get; set; } = string.Empty;
}

