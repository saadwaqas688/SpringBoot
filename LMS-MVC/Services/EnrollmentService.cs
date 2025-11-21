using AutoMapper;
using LMS_MVC.DTOs;
using LMS_MVC.Models;
using LMS_MVC.Repositories;

namespace LMS_MVC.Services;

public class EnrollmentService : IEnrollmentService
{
    private readonly IEnrollmentRepository _enrollmentRepository;
    private readonly ICourseRepository _courseRepository;
    private readonly IMapper _mapper;

    public EnrollmentService(
        IEnrollmentRepository enrollmentRepository,
        ICourseRepository courseRepository,
        IMapper mapper)
    {
        _enrollmentRepository = enrollmentRepository;
        _courseRepository = courseRepository;
        _mapper = mapper;
    }

    public async Task<List<EnrollmentDto>> GetUserEnrollmentsAsync(string userId)
    {
        var enrollments = await _enrollmentRepository.GetByUserIdAsync(userId);
        return enrollments.Select(e => new EnrollmentDto
        {
            UserId = e.UserId,
            UserName = e.User?.Name,
            CourseId = e.CourseId,
            CourseTitle = e.Course?.Title,
            EnrolledAt = e.EnrolledAt
        }).ToList();
    }

    public async Task<List<EnrollmentDto>> GetCourseEnrollmentsAsync(int courseId)
    {
        var enrollments = await _enrollmentRepository.GetByCourseIdAsync(courseId);
        return enrollments.Select(e => new EnrollmentDto
        {
            UserId = e.UserId,
            UserName = e.User?.Name,
            CourseId = e.CourseId,
            CourseTitle = e.Course?.Title,
            EnrolledAt = e.EnrolledAt
        }).ToList();
    }

    public async Task<EnrollmentDto> EnrollUserAsync(EnrollUserDto dto)
    {
        var course = await _courseRepository.GetByIdAsync(dto.CourseId);
        if (course == null)
            throw new Exception("Course not found");

        var existing = await _enrollmentRepository.GetByUserAndCourseAsync(dto.UserId, dto.CourseId);
        if (existing != null)
            throw new Exception("User is already enrolled in this course");

        var enrollment = new UserCourse
        {
            UserId = dto.UserId,
            CourseId = dto.CourseId,
            EnrolledAt = DateTime.UtcNow
        };

        var created = await _enrollmentRepository.CreateAsync(enrollment);
        return new EnrollmentDto
        {
            UserId = created.UserId,
            UserName = created.User?.Name,
            CourseId = created.CourseId,
            CourseTitle = created.Course?.Title,
            EnrolledAt = created.EnrolledAt
        };
    }

    public async Task UnenrollUserAsync(string userId, int courseId)
    {
        await _enrollmentRepository.DeleteAsync(userId, courseId);
    }

    public async Task<bool> IsEnrolledAsync(string userId, int courseId)
    {
        return await _enrollmentRepository.IsEnrolledAsync(userId, courseId);
    }
}

