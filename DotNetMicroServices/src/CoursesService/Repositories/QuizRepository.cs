using CoursesService.Models;
using MongoDB.Driver;
using Shared.Repositories;

namespace CoursesService.Repositories;

public class QuizRepository : BaseRepository<Quiz>, IQuizRepository
{
    public QuizRepository(IMongoCollection<Quiz> collection, ILogger<QuizRepository> logger)
        : base(collection, logger)
    {
    }

    public async Task<Quiz?> GetByLessonIdAsync(string lessonId)
    {
        var filter = Builders<Quiz>.Filter.Eq(q => q.LessonId, lessonId);
        return await _collection.Find(filter).FirstOrDefaultAsync();
    }
}

