namespace LMS.API.DTOs;

public class CourseDto
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? ThumbnailUrl { get; set; }
    public string Category { get; set; } = string.Empty;
    public string Level { get; set; } = string.Empty;
    public string CreatedBy { get; set; } = string.Empty;
    public string? CreatorName { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public int LessonCount { get; set; }
    public int EnrollmentCount { get; set; }
}

public class CreateCourseDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? ThumbnailUrl { get; set; }
    public string Category { get; set; } = string.Empty;
    public string Level { get; set; } = string.Empty;
}

public class UpdateCourseDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? ThumbnailUrl { get; set; }
    public string Category { get; set; } = string.Empty;
    public string Level { get; set; } = string.Empty;
}

