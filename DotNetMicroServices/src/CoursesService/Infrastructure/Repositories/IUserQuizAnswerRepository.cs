using CoursesService.Models;
using Shared.Infrastructure.Repositories;

namespace CoursesService.Repositories;

public interface IUserQuizAnswerRepository : IBaseRepository<UserQuizAnswer>
{
    Task<IEnumerable<UserQuizAnswer>> GetByAttemptIdAsync(string attemptId);
}


