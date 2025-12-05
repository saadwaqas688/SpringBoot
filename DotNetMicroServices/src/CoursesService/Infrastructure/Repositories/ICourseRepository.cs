using CoursesService.Models;
using MongoDB.Driver;
using Shared.Infrastructure.Repositories;

namespace CoursesService.Repositories;

public interface ICourseRepository : IBaseRepository<Course>
{
    Task<IEnumerable<Course>> GetByStatusAsync(string status);
    Task<IEnumerable<Course>> SearchAsync(string searchTerm);
}


