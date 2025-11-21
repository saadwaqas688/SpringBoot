namespace LMS_MVC.DTOs;

public class LessonContentDto
{
    public int Id { get; set; }
    public int LessonId { get; set; }
    public string Type { get; set; } = string.Empty;
    public int Order { get; set; }
    public string Title { get; set; } = string.Empty;
    public ContentDataDto? Data { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class ContentDataDto
{
    public string? Text { get; set; }
    public string? ImageUrl { get; set; }
    public string? VideoUrl { get; set; }
    public int? Duration { get; set; }
    public string? Question { get; set; }
    public List<string>? Options { get; set; }
    public string? CorrectAnswer { get; set; }
}

public class CreateLessonContentDto
{
    public int LessonId { get; set; }
    public string Type { get; set; } = string.Empty;
    public int Order { get; set; }
    public string Title { get; set; } = string.Empty;
    public ContentDataDto? Data { get; set; }
}

public class UpdateLessonContentDto
{
    public string Type { get; set; } = string.Empty;
    public int Order { get; set; }
    public string Title { get; set; } = string.Empty;
    public ContentDataDto? Data { get; set; }
}

