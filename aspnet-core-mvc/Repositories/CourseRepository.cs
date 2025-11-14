using CourseManagementAPI.Models;
using MongoDB.Driver;

namespace CourseManagementAPI.Repositories;

public class CourseRepository : ICourseRepository
{
    private readonly IMongoCollection<Course> _courses;

    public CourseRepository(IMongoDatabase database)
    {
        _courses = database.GetCollection<Course>("courses");
    }

    public async Task<List<Course>> GetAllAsync()
    {
        return await _courses.Find(_ => true).ToListAsync();
    }

    public async Task<Course?> GetByIdAsync(string id)
    {
        return await _courses.Find(c => c.Id == id).FirstOrDefaultAsync();
    }

    public async Task<Course> CreateAsync(Course course)
    {
        await _courses.InsertOneAsync(course);
        return course;
    }

    public async Task<Course> UpdateAsync(Course course)
    {
        await _courses.ReplaceOneAsync(c => c.Id == course.Id, course);
        return course;
    }

    public async Task DeleteAsync(string id)
    {
        await _courses.DeleteOneAsync(c => c.Id == id);
    }
}



