using CoursesService.Models;
using Shared.Infrastructure.Repositories;

namespace CoursesService.Repositories;

public interface IQuizQuestionRepository : IBaseRepository<QuizQuestion>
{
    Task<IEnumerable<QuizQuestion>> GetByQuizIdAsync(string quizId);
}


