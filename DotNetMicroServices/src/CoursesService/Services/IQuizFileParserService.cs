using CoursesService.Models;

namespace CoursesService.Services;

public interface IQuizFileParserService
{
    Task<List<QuizQuestionData>> ParseFileAsync(Stream fileStream, string fileName);
}

public class QuizQuestionData
{
    public string Question { get; set; } = string.Empty;
    public List<QuizOptionData> Options { get; set; } = new();
    public int Order { get; set; }
}

public class QuizOptionData
{
    public string Value { get; set; } = string.Empty;
    public bool IsCorrect { get; set; }
}

