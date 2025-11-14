using System.ComponentModel.DataAnnotations;

namespace CourseManagementAPI.DTOs;

public class EnrollUsersRequest
{
    [Required]
    public List<string> UserIds { get; set; } = new();

    [Required]
    public string CourseId { get; set; } = string.Empty;
}



