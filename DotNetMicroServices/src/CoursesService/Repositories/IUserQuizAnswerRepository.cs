using CoursesService.Models;
using Shared.Repositories;

namespace CoursesService.Repositories;

public interface IUserQuizAnswerRepository : IBaseRepository<UserQuizAnswer>
{
    Task<IEnumerable<UserQuizAnswer>> GetByAttemptIdAsync(string attemptId);
}

