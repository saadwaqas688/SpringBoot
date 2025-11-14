using CourseManagementAPI.Models;

namespace CourseManagementAPI.Repositories;

public interface IDiscussionRepository
{
    Task<List<Discussion>> GetAllAsync();
    Task<Discussion?> GetByIdAsync(string id);
    Task<List<Discussion>> GetByCourseIdAsync(string courseId);
    Task<Discussion> CreateAsync(Discussion discussion);
    Task<Discussion> UpdateAsync(Discussion discussion);
    Task DeleteAsync(string id);
}



