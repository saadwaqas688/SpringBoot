namespace CourseManagementAPI.DTOs;

public class EnrolledUserDto
{
    public string UserId { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime EnrolledAt { get; set; }
}



