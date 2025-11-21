using LMS.API.DTOs;

namespace LMS.API.Services;

public interface IEnrollmentService
{
    Task<List<EnrollmentDto>> GetAllEnrollmentsAsync();
    Task<EnrollmentDto> EnrollUserAsync(EnrollUserDto dto);
    Task DeleteEnrollmentAsync(string id);
    Task<List<EnrollmentDto>> GetUserEnrollmentsAsync(string userId);
    Task<bool> IsUserEnrolledAsync(string userId, string courseId);
}

