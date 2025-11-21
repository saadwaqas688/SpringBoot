using LMS_MVC.DTOs;

namespace LMS_MVC.Services;

public interface IEnrollmentService
{
    Task<List<EnrollmentDto>> GetUserEnrollmentsAsync(string userId);
    Task<List<EnrollmentDto>> GetCourseEnrollmentsAsync(int courseId);
    Task<EnrollmentDto> EnrollUserAsync(EnrollUserDto dto);
    Task UnenrollUserAsync(string userId, int courseId);
    Task<bool> IsEnrolledAsync(string userId, int courseId);
}

