using MongoDB.Driver;
using LMS.API.Models;

namespace LMS.API.Data;

public class MongoDbContext
{
    private readonly IMongoDatabase _database;

    public MongoDbContext(IMongoClient client, string databaseName)
    {
        _database = client.GetDatabase(databaseName);
    }

    public IMongoCollection<User> Users => _database.GetCollection<User>("Users");
    public IMongoCollection<Course> Courses => _database.GetCollection<Course>("Courses");
    public IMongoCollection<Lesson> Lessons => _database.GetCollection<Lesson>("Lessons");
    public IMongoCollection<LessonContent> LessonContents => _database.GetCollection<LessonContent>("LessonContents");
    public IMongoCollection<DiscussionPost> DiscussionPosts => _database.GetCollection<DiscussionPost>("DiscussionPosts");
    public IMongoCollection<LessonProgress> LessonProgresses => _database.GetCollection<LessonProgress>("LessonProgresses");
    public IMongoCollection<UserCourse> UserCourses => _database.GetCollection<UserCourse>("UserCourses");
}

