using CourseManagementAPI.Models;
using MongoDB.Driver;

namespace CourseManagementAPI.Repositories;

public class CourseEnrollmentRepository : ICourseEnrollmentRepository
{
    private readonly IMongoCollection<CourseEnrollment> _enrollments;

    public CourseEnrollmentRepository(IMongoDatabase database)
    {
        _enrollments = database.GetCollection<CourseEnrollment>("courseEnrollments");
    }

    public async Task<List<CourseEnrollment>> GetByCourseIdAsync(string courseId)
    {
        return await _enrollments.Find(e => e.CourseId == courseId).ToListAsync();
    }

    public async Task<List<CourseEnrollment>> GetByUserIdAsync(string userId)
    {
        return await _enrollments.Find(e => e.UserId == userId).ToListAsync();
    }

    public async Task<CourseEnrollment?> GetByCourseIdAndUserIdAsync(string courseId, string userId)
    {
        return await _enrollments.Find(e => e.CourseId == courseId && e.UserId == userId).FirstOrDefaultAsync();
    }

    public async Task<CourseEnrollment> CreateAsync(CourseEnrollment enrollment)
    {
        await _enrollments.InsertOneAsync(enrollment);
        return enrollment;
    }

    public async Task DeleteAsync(string id)
    {
        await _enrollments.DeleteOneAsync(e => e.Id == id);
    }
}



