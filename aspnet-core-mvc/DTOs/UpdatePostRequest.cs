using System.ComponentModel.DataAnnotations;

namespace CourseManagementAPI.DTOs;

public class UpdatePostRequest
{
    [Required(ErrorMessage = "Post content is required")]
    [MinLength(1, ErrorMessage = "Content must be at least 1 character")]
    [MaxLength(5000, ErrorMessage = "Content must not exceed 5000 characters")]
    public string Content { get; set; } = string.Empty;
}

