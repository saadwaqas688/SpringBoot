using CourseManagementAPI.Models;

namespace CourseManagementAPI.Repositories;

public interface ICourseEnrollmentRepository
{
    Task<List<CourseEnrollment>> GetByCourseIdAsync(string courseId);
    Task<List<CourseEnrollment>> GetByUserIdAsync(string userId);
    Task<CourseEnrollment?> GetByCourseIdAndUserIdAsync(string courseId, string userId);
    Task<CourseEnrollment> CreateAsync(CourseEnrollment enrollment);
    Task DeleteAsync(string id);
}



