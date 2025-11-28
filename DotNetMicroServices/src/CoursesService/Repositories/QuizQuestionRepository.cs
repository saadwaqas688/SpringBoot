using CoursesService.Models;
using MongoDB.Driver;
using Shared.Repositories;

namespace CoursesService.Repositories;

public class QuizQuestionRepository : BaseRepository<QuizQuestion>, IQuizQuestionRepository
{
    public QuizQuestionRepository(IMongoCollection<QuizQuestion> collection, ILogger<QuizQuestionRepository> logger)
        : base(collection, logger)
    {
    }

    public async Task<IEnumerable<QuizQuestion>> GetByQuizIdAsync(string quizId)
    {
        var filter = Builders<QuizQuestion>.Filter.Eq(q => q.QuizId, quizId);
        var sort = Builders<QuizQuestion>.Sort.Ascending(q => q.Order);
        return await _collection.Find(filter).Sort(sort).ToListAsync();
    }
}

