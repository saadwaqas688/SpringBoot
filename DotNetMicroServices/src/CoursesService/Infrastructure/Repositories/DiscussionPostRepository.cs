using CoursesService.Models;
using CoursesService.DTOs;
using MongoDB.Driver;
using MongoDB.Bson;
using Shared.Infrastructure.Repositories;

namespace CoursesService.Repositories;

public class DiscussionPostRepository : BaseRepository<DiscussionPost>, IDiscussionPostRepository
{
    public DiscussionPostRepository(
        IMongoCollection<DiscussionPost> collection,
        ILogger<DiscussionPostRepository> logger)
        : base(collection, logger)
    {
    }

    public async Task<IEnumerable<DiscussionPost>> GetByLessonIdAsync(string lessonId)
    {
        var filter = Builders<DiscussionPost>.Filter.Eq(d => d.LessonId, lessonId);
        var sort = Builders<DiscussionPost>.Sort.Descending(d => d.CreatedAt);
        return await _collection.Find(filter).Sort(sort).ToListAsync();
    }

    public async Task<IEnumerable<DiscussionPost>> GetCommentsByPostIdAsync(string postId)
    {
        var filter = Builders<DiscussionPost>.Filter.Eq(d => d.ParentPostId, postId);
        var sort = Builders<DiscussionPost>.Sort.Ascending(d => d.CreatedAt);
        return await _collection.Find(filter).Sort(sort).ToListAsync();
    }

    public async Task<IEnumerable<DiscussionPost>> GetPostsByLessonIdAsync(string lessonId)
    {
        var filter = Builders<DiscussionPost>.Filter.And(
            Builders<DiscussionPost>.Filter.Eq(d => d.LessonId, lessonId),
            Builders<DiscussionPost>.Filter.Eq(d => d.ParentPostId, (string?)null)
        );
        var sort = Builders<DiscussionPost>.Sort.Descending(d => d.CreatedAt);
        return await _collection.Find(filter).Sort(sort).ToListAsync();
    }

    public async Task<IEnumerable<DiscussionPost>> GetPostsByDiscussionIdAsync(string discussionId)
    {
        var filter = Builders<DiscussionPost>.Filter.And(
            Builders<DiscussionPost>.Filter.Eq(d => d.DiscussionId, discussionId),
            Builders<DiscussionPost>.Filter.Eq(d => d.ParentPostId, (string?)null)
        );
        var sort = Builders<DiscussionPost>.Sort.Descending(d => d.CreatedAt);
        return await _collection.Find(filter).Sort(sort).ToListAsync();
    }

    public async Task<int> GetUniqueUserCountByDiscussionIdAsync(string discussionId)
    {
        var filter = Builders<DiscussionPost>.Filter.Eq(d => d.DiscussionId, discussionId);
        var posts = await _collection.Find(filter).ToListAsync();
        
        var uniqueUserIds = posts
            .Where(p => !string.IsNullOrEmpty(p.UserId))
            .Select(p => p.UserId)
            .Distinct()
            .Count();
        
        return uniqueUserIds;
    }

    public async Task<IEnumerable<DiscussionPostWithUserDto>> GetPostsByLessonIdWithUsersAsync(string lessonId)
    {
        // Use MongoDB aggregation with $lookup to join with UserAccounts collection
        var pipeline = new BsonDocument[]
        {
            new BsonDocument("$match", new BsonDocument
            {
                { "lessonId", ObjectId.Parse(lessonId) },
                { "parentPostId", BsonNull.Value }
            }),
            // Lookup user information from UserAccounts collection (same database)
            new BsonDocument("$lookup", new BsonDocument
            {
                { "from", "UserAccounts" },
                { "localField", "userId" },
                { "foreignField", "_id" },
                { "as", "user" }
            }),
            // Unwind user array (should be single user or empty)
            new BsonDocument("$unwind", new BsonDocument
            {
                { "path", "$user" },
                { "preserveNullAndEmptyArrays", true }
            }),
            // Project to desired format
            new BsonDocument("$project", new BsonDocument
            {
                { "_id", 1 },
                { "lessonId", 1 },
                { "discussionId", 1 },
                { "userId", 1 },
                { "parentPostId", 1 },
                { "content", 1 },
                { "attachments", 1 },
                { "createdAt", 1 },
                { "updatedAt", 1 },
                { "userName", new BsonDocument("$ifNull", new BsonArray { "$user.name", BsonNull.Value }) },
                { "userEmail", new BsonDocument("$ifNull", new BsonArray { "$user.email", BsonNull.Value }) },
                { "userImage", new BsonDocument("$ifNull", new BsonArray { "$user.image", BsonNull.Value }) }
            }),
            new BsonDocument("$sort", new BsonDocument { { "createdAt", -1 } })
        };

        var results = await _collection.Aggregate<BsonDocument>(pipeline).ToListAsync();
        return results.Select(doc => MapToDto(doc));
    }

    public async Task<IEnumerable<DiscussionPostWithUserDto>> GetCommentsByPostIdWithUsersAsync(string postId)
    {
        var pipeline = new BsonDocument[]
        {
            new BsonDocument("$match", new BsonDocument
            {
                { "parentPostId", ObjectId.Parse(postId) }
            }),
            // Lookup user information from UserAccounts collection
            new BsonDocument("$lookup", new BsonDocument
            {
                { "from", "UserAccounts" },
                { "localField", "userId" },
                { "foreignField", "_id" },
                { "as", "user" }
            }),
            // Unwind user array
            new BsonDocument("$unwind", new BsonDocument
            {
                { "path", "$user" },
                { "preserveNullAndEmptyArrays", true }
            }),
            // Project to desired format
            new BsonDocument("$project", new BsonDocument
            {
                { "_id", 1 },
                { "lessonId", 1 },
                { "discussionId", 1 },
                { "userId", 1 },
                { "parentPostId", 1 },
                { "content", 1 },
                { "attachments", 1 },
                { "createdAt", 1 },
                { "updatedAt", 1 },
                { "userName", new BsonDocument("$ifNull", new BsonArray { "$user.name", BsonNull.Value }) },
                { "userEmail", new BsonDocument("$ifNull", new BsonArray { "$user.email", BsonNull.Value }) },
                { "userImage", new BsonDocument("$ifNull", new BsonArray { "$user.image", BsonNull.Value }) }
            }),
            new BsonDocument("$sort", new BsonDocument { { "createdAt", 1 } })
        };

        var results = await _collection.Aggregate<BsonDocument>(pipeline).ToListAsync();
        return results.Select(doc => MapToDto(doc));
    }

    public async Task<IEnumerable<DiscussionPostWithUserDto>> GetPostsByDiscussionIdWithUsersAsync(string discussionId)
    {
        var pipeline = new BsonDocument[]
        {
            new BsonDocument("$match", new BsonDocument
            {
                { "discussionId", ObjectId.Parse(discussionId) },
                { "parentPostId", BsonNull.Value }
            }),
            // Lookup user information from UserAccounts collection
            new BsonDocument("$lookup", new BsonDocument
            {
                { "from", "UserAccounts" },
                { "localField", "userId" },
                { "foreignField", "_id" },
                { "as", "user" }
            }),
            // Unwind user array
            new BsonDocument("$unwind", new BsonDocument
            {
                { "path", "$user" },
                { "preserveNullAndEmptyArrays", true }
            }),
            // Project to desired format
            new BsonDocument("$project", new BsonDocument
            {
                { "_id", 1 },
                { "lessonId", 1 },
                { "discussionId", 1 },
                { "userId", 1 },
                { "parentPostId", 1 },
                { "content", 1 },
                { "attachments", 1 },
                { "createdAt", 1 },
                { "updatedAt", 1 },
                { "userName", new BsonDocument("$ifNull", new BsonArray { "$user.name", BsonNull.Value }) },
                { "userEmail", new BsonDocument("$ifNull", new BsonArray { "$user.email", BsonNull.Value }) },
                { "userImage", new BsonDocument("$ifNull", new BsonArray { "$user.image", BsonNull.Value }) }
            }),
            new BsonDocument("$sort", new BsonDocument { { "createdAt", -1 } })
        };

        var results = await _collection.Aggregate<BsonDocument>(pipeline).ToListAsync();
        return results.Select(doc => MapToDto(doc));
    }

    private DiscussionPostWithUserDto MapToDto(BsonDocument doc)
    {
        return new DiscussionPostWithUserDto
        {
            Id = doc["_id"].AsObjectId.ToString(),
            LessonId = doc.Contains("lessonId") && !doc["lessonId"].IsBsonNull 
                ? (doc["lessonId"].IsObjectId ? doc["lessonId"].AsObjectId.ToString() : doc["lessonId"].AsString)
                : string.Empty,
            DiscussionId = doc.Contains("discussionId") && !doc["discussionId"].IsBsonNull 
                ? (doc["discussionId"].IsObjectId ? doc["discussionId"].AsObjectId.ToString() : doc["discussionId"].AsString)
                : null,
            UserId = doc.Contains("userId") && !doc["userId"].IsBsonNull
                ? (doc["userId"].IsObjectId ? doc["userId"].AsObjectId.ToString() : doc["userId"].AsString)
                : string.Empty,
            ParentPostId = doc.Contains("parentPostId") && !doc["parentPostId"].IsBsonNull 
                ? (doc["parentPostId"].IsObjectId ? doc["parentPostId"].AsObjectId.ToString() : doc["parentPostId"].AsString)
                : null,
            Content = doc.Contains("content") && !doc["content"].IsBsonNull ? doc["content"].AsString : string.Empty,
            Attachments = doc.Contains("attachments") && doc["attachments"].IsBsonArray
                ? doc["attachments"].AsBsonArray.Select(a => a.AsString).ToList()
                : new List<string>(),
            CreatedAt = doc.Contains("createdAt") && !doc["createdAt"].IsBsonNull ? doc["createdAt"].ToUniversalTime() : DateTime.UtcNow,
            UpdatedAt = doc.Contains("updatedAt") && !doc["updatedAt"].IsBsonNull ? doc["updatedAt"].ToUniversalTime() : DateTime.UtcNow,
            UserName = doc.Contains("userName") && !doc["userName"].IsBsonNull && !string.IsNullOrEmpty(doc["userName"].AsString) 
                ? doc["userName"].AsString 
                : null,
            UserEmail = doc.Contains("userEmail") && !doc["userEmail"].IsBsonNull && !string.IsNullOrEmpty(doc["userEmail"].AsString)
                ? doc["userEmail"].AsString 
                : null,
            UserImage = doc.Contains("userImage") && !doc["userImage"].IsBsonNull && !string.IsNullOrEmpty(doc["userImage"].AsString)
                ? doc["userImage"].AsString 
                : null
        };
    }
}


