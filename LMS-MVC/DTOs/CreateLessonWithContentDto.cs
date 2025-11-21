namespace LMS_MVC.DTOs;

public class CreateLessonWithContentDto
{
    public int CourseId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Order { get; set; }
    public List<CreateLessonContentItemDto> Contents { get; set; } = new();
}

public class CreateLessonContentItemDto
{
    public string Type { get; set; } = string.Empty; // slide | video | quiz | discussion
    public int Order { get; set; }
    public string Title { get; set; } = string.Empty;
    
    // Slide fields
    public string? Text { get; set; }
    public string? ImageUrl { get; set; }
    
    // Video fields
    public string? VideoUrl { get; set; }
    public int? Duration { get; set; }
    
    // Quiz fields
    public string? Question { get; set; }
    public List<string>? Options { get; set; }
    public string? CorrectAnswer { get; set; }
}

