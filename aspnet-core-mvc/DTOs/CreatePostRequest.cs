using System.ComponentModel.DataAnnotations;

namespace CourseManagementAPI.DTOs;

public class CreatePostRequest
{
    [Required(ErrorMessage = "Discussion ID is required")]
    public string DiscussionId { get; set; } = string.Empty;

    [Required(ErrorMessage = "Course ID is required")]
    public string CourseId { get; set; } = string.Empty;

    [Required(ErrorMessage = "Post content is required")]
    [MinLength(1, ErrorMessage = "Content must be at least 1 character")]
    [MaxLength(5000, ErrorMessage = "Content must not exceed 5000 characters")]
    public string Content { get; set; } = string.Empty;
}

