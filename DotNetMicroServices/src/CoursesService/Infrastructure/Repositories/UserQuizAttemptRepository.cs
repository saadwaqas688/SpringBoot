using CoursesService.Models;
using MongoDB.Driver;
using Shared.Infrastructure.Repositories;

namespace CoursesService.Repositories;

public class UserQuizAttemptRepository : BaseRepository<UserQuizAttempt>, IUserQuizAttemptRepository
{
    public UserQuizAttemptRepository(IMongoCollection<UserQuizAttempt> collection, ILogger<UserQuizAttemptRepository> logger)
        : base(collection, logger)
    {
    }

    public async Task<IEnumerable<UserQuizAttempt>> GetByUserIdAsync(string userId)
    {
        var filter = Builders<UserQuizAttempt>.Filter.Eq(u => u.UserId, userId);
        var sort = Builders<UserQuizAttempt>.Sort.Descending(u => u.AttemptedAt);
        return await _collection.Find(filter).Sort(sort).ToListAsync();
    }

    public async Task<UserQuizAttempt?> GetLatestByUserAndQuizAsync(string userId, string quizId)
    {
        var filter = Builders<UserQuizAttempt>.Filter.And(
            Builders<UserQuizAttempt>.Filter.Eq(u => u.UserId, userId),
            Builders<UserQuizAttempt>.Filter.Eq(u => u.QuizId, quizId)
        );
        var sort = Builders<UserQuizAttempt>.Sort.Descending(u => u.AttemptedAt);
        return await _collection.Find(filter).Sort(sort).FirstOrDefaultAsync();
    }
}


