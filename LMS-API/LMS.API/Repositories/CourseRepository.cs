using LMS.API.Data;
using LMS.API.Models;
using MongoDB.Driver;

namespace LMS.API.Repositories;

public class CourseRepository : ICourseRepository
{
    private readonly MongoDbContext _context;

    public CourseRepository(MongoDbContext context)
    {
        _context = context;
    }

    public async Task<List<Course>> GetAllAsync()
    {
        return await _context.Courses.Find(_ => true).ToListAsync();
    }

    public async Task<Course?> GetByIdAsync(string id)
    {
        return await _context.Courses.Find(c => c.Id == id).FirstOrDefaultAsync();
    }

    public async Task<Course> CreateAsync(Course course)
    {
        await _context.Courses.InsertOneAsync(course);
        return course;
    }

    public async Task<Course> UpdateAsync(Course course)
    {
        await _context.Courses.ReplaceOneAsync(c => c.Id == course.Id, course);
        return course;
    }

    public async Task DeleteAsync(string id)
    {
        await _context.Courses.DeleteOneAsync(c => c.Id == id);
    }

    public async Task<bool> ExistsAsync(string id)
    {
        var course = await _context.Courses.Find(c => c.Id == id).FirstOrDefaultAsync();
        return course != null;
    }

    public async Task<List<Course>> GetByCreatorIdAsync(string userId)
    {
        return await _context.Courses.Find(c => c.CreatedBy == userId).ToListAsync();
    }
}

