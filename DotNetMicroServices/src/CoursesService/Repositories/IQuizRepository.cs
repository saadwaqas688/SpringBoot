using CoursesService.Models;
using Shared.Repositories;
using Shared.Common;

namespace CoursesService.Repositories;

public interface IQuizRepository : IBaseRepository<Quiz>
{
    Task<Quiz?> GetByLessonIdAsync(string lessonId);
    Task<PagedResponse<QuizWithQuestionCountDto>> GetAllWithQuestionCountAsync(int page, int pageSize);
}

public class QuizWithQuestionCountDto
{
    public string? Id { get; set; }
    public string LessonId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int QuestionsCount { get; set; }
}

