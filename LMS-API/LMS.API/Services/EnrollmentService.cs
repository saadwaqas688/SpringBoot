using AutoMapper;
using LMS.API.DTOs;
using LMS.API.Models;
using LMS.API.Repositories;

namespace LMS.API.Services;

public class EnrollmentService : IEnrollmentService
{
    private readonly IEnrollmentRepository _enrollmentRepository;
    private readonly IUserRepository _userRepository;
    private readonly ICourseRepository _courseRepository;
    private readonly IMapper _mapper;

    public EnrollmentService(
        IEnrollmentRepository enrollmentRepository,
        IUserRepository userRepository,
        ICourseRepository courseRepository,
        IMapper mapper)
    {
        _enrollmentRepository = enrollmentRepository;
        _userRepository = userRepository;
        _courseRepository = courseRepository;
        _mapper = mapper;
    }

    public async Task<List<EnrollmentDto>> GetAllEnrollmentsAsync()
    {
        var enrollments = await _enrollmentRepository.GetAllAsync();
        var enrollmentDtos = new List<EnrollmentDto>();

        foreach (var enrollment in enrollments)
        {
            enrollmentDtos.Add(await MapToDtoAsync(enrollment));
        }

        return enrollmentDtos;
    }

    public async Task<EnrollmentDto> EnrollUserAsync(EnrollUserDto dto)
    {
        if (await _enrollmentRepository.ExistsAsync(dto.UserId, dto.CourseId))
        {
            throw new Exception("User is already enrolled in this course");
        }

        var enrollment = new UserCourse
        {
            UserId = dto.UserId,
            CourseId = dto.CourseId,
            EnrolledAt = DateTime.UtcNow
        };

        var created = await _enrollmentRepository.CreateAsync(enrollment);
        return await MapToDtoAsync(created);
    }

    public async Task DeleteEnrollmentAsync(string id)
    {
        if (await _enrollmentRepository.GetByIdAsync(id) == null)
            throw new Exception("Enrollment not found");

        await _enrollmentRepository.DeleteAsync(id);
    }

    public async Task<List<EnrollmentDto>> GetUserEnrollmentsAsync(string userId)
    {
        var enrollments = await _enrollmentRepository.GetByUserIdAsync(userId);
        var enrollmentDtos = new List<EnrollmentDto>();

        foreach (var enrollment in enrollments)
        {
            enrollmentDtos.Add(await MapToDtoAsync(enrollment));
        }

        return enrollmentDtos;
    }

    public async Task<bool> IsUserEnrolledAsync(string userId, string courseId)
    {
        return await _enrollmentRepository.ExistsAsync(userId, courseId);
    }

    private async Task<EnrollmentDto> MapToDtoAsync(UserCourse enrollment)
    {
        var dto = _mapper.Map<EnrollmentDto>(enrollment);
        
        var user = await _userRepository.GetByIdAsync(enrollment.UserId);
        dto.UserName = user?.Name;

        var course = await _courseRepository.GetByIdAsync(enrollment.CourseId);
        dto.CourseTitle = course?.Title;

        return dto;
    }
}

