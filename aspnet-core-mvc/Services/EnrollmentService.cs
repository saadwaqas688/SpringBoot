using CourseManagementAPI.Models;
using CourseManagementAPI.Repositories;

namespace CourseManagementAPI.Services;

public class EnrollmentService : IEnrollmentService
{
    private readonly ICourseEnrollmentRepository _enrollmentRepository;
    private readonly ICourseRepository _courseRepository;
    private readonly IUserRepository _userRepository;

    public EnrollmentService(
        ICourseEnrollmentRepository enrollmentRepository,
        ICourseRepository courseRepository,
        IUserRepository userRepository)
    {
        _enrollmentRepository = enrollmentRepository;
        _courseRepository = courseRepository;
        _userRepository = userRepository;
    }

    public async Task<List<CourseEnrollment>> EnrollUsersAsync(List<string> userIds, string courseId, string grantedBy)
    {
        var course = await _courseRepository.GetByIdAsync(courseId);
        if (course == null)
        {
            throw new Exception("Course not found with id: " + courseId);
        }

        var enrollments = new List<CourseEnrollment>();
        foreach (var userId in userIds)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                throw new Exception("User not found with id: " + userId);
            }

            var existingEnrollment = await _enrollmentRepository.GetByCourseIdAndUserIdAsync(courseId, userId);
            if (existingEnrollment != null)
            {
                continue; // User already enrolled
            }

            var enrollment = new CourseEnrollment
            {
                CourseId = courseId,
                UserId = userId,
                GrantedBy = grantedBy,
                EnrolledAt = DateTime.UtcNow
            };

            await _enrollmentRepository.CreateAsync(enrollment);
            enrollments.Add(enrollment);
        }

        return enrollments;
    }

    public async Task<List<CourseEnrollment>> GetEnrollmentsByCourseIdAsync(string courseId)
    {
        return await _enrollmentRepository.GetByCourseIdAsync(courseId);
    }

    public async Task<List<Course>> GetCoursesByUserIdAsync(string userId)
    {
        var enrollments = await _enrollmentRepository.GetByUserIdAsync(userId);
        var courseIds = enrollments.Select(e => e.CourseId).ToList();
        
        var courses = new List<Course>();
        foreach (var courseId in courseIds)
        {
            var course = await _courseRepository.GetByIdAsync(courseId);
            if (course != null)
            {
                courses.Add(course);
            }
        }

        return courses;
    }
}



