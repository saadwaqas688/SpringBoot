using CoursesService.Models;
using MongoDB.Driver;
using MongoDB.Bson;
using Shared.Infrastructure.Repositories;
using Shared.Core.Common;

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

    public async Task<PagedResponse<QuizWithQuestionCountDto>> GetAllWithQuestionCountAsync(int page, int pageSize)
    {
        try
        {
            // Get total count first (before pagination)
            var totalCount = await _collection.CountDocumentsAsync(_ => true);

            // Build aggregation pipeline
            var pipeline = new BsonDocument[]
            {
                // Lookup quiz questions to count them
                // Match quizId (ObjectId) with quiz _id (ObjectId)
                new BsonDocument("$lookup", new BsonDocument
                {
                    { "from", "quiz_questions" },
                    { "let", new BsonDocument("quizId", "$_id") },
                    { "pipeline", new BsonArray
                    {
                        new BsonDocument("$match", new BsonDocument
                        {
                            { "$expr", new BsonDocument
                            {
                                { "$eq", new BsonArray
                                {
                                    new BsonDocument("$toObjectId", "$quizId"),
                                    "$$quizId"
                                }}
                            }}
                        })
                    }},
                    { "as", "questions" }
                }),
                // Add field with question count
                new BsonDocument("$addFields", new BsonDocument
                {
                    { "questionsCount", new BsonDocument("$size", "$questions") }
                }),
                // Project to desired format
                new BsonDocument("$project", new BsonDocument
                {
                    { "_id", 1 },
                    { "lessonId", 1 },
                    { "title", 1 },
                    { "description", 1 },
                    { "questionsCount", 1 }
                }),
                // Sort by _id descending (newest first)
                new BsonDocument("$sort", new BsonDocument { { "_id", -1 } }),
                // Apply pagination
                new BsonDocument("$skip", (page - 1) * pageSize),
                new BsonDocument("$limit", pageSize)
            };

            var results = await _collection.Aggregate<BsonDocument>(pipeline).ToListAsync();

            // Map results to DTOs
            var items = results.Select(doc => new QuizWithQuestionCountDto
            {
                Id = doc["_id"].ToString(),
                LessonId = doc.Contains("lessonId") ? doc["lessonId"].ToString() : string.Empty,
                Title = doc.Contains("title") ? doc["title"].ToString() : string.Empty,
                Description = doc.Contains("description") ? doc["description"].ToString() : string.Empty,
                QuestionsCount = doc.Contains("questionsCount") ? doc["questionsCount"].AsInt32 : 0
            }).ToList();

            return new PagedResponse<QuizWithQuestionCountDto>
            {
                Items = items,
                PageNumber = page,
                PageSize = pageSize,
                TotalCount = (int)totalCount
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting quizzes with question count");
            throw;
        }
    }
}


