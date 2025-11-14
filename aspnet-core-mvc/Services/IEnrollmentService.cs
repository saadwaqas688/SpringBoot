using CourseManagementAPI.Models;

namespace CourseManagementAPI.Services;

public interface IEnrollmentService
{
    Task<List<CourseEnrollment>> EnrollUsersAsync(List<string> userIds, string courseId, string grantedBy);
    Task<List<CourseEnrollment>> GetEnrollmentsByCourseIdAsync(string courseId);
    Task<List<Course>> GetCoursesByUserIdAsync(string userId);
}



