using LMS.API.Models;

namespace LMS.API.Repositories;

public interface ICourseRepository
{
    Task<List<Course>> GetAllAsync();
    Task<Course?> GetByIdAsync(string id);
    Task<Course> CreateAsync(Course course);
    Task<Course> UpdateAsync(Course course);
    Task DeleteAsync(string id);
    Task<bool> ExistsAsync(string id);
    Task<List<Course>> GetByCreatorIdAsync(string userId);
}

