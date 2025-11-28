using CoursesService.Models;
using Shared.Repositories;

namespace CoursesService.Repositories;

public interface IUserQuizAttemptRepository : IBaseRepository<UserQuizAttempt>
{
    Task<IEnumerable<UserQuizAttempt>> GetByUserIdAsync(string userId);
    Task<UserQuizAttempt?> GetLatestByUserAndQuizAsync(string userId, string quizId);
}

