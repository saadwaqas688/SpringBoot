using CoursesService.Models;
using MongoDB.Driver;
using Shared.Repositories;

namespace CoursesService.Repositories;

public class UserQuizAnswerRepository : BaseRepository<UserQuizAnswer>, IUserQuizAnswerRepository
{
    public UserQuizAnswerRepository(IMongoCollection<UserQuizAnswer> collection, ILogger<UserQuizAnswerRepository> logger)
        : base(collection, logger)
    {
    }

    public async Task<IEnumerable<UserQuizAnswer>> GetByAttemptIdAsync(string attemptId)
    {
        var filter = Builders<UserQuizAnswer>.Filter.Eq(a => a.AttemptId, attemptId);
        return await _collection.Find(filter).ToListAsync();
    }
}

