using AutoMapper;
using LMS_MVC.DTOs;
using LMS_MVC.Models;
using LMS_MVC.Repositories;
using Microsoft.AspNetCore.Identity;

namespace LMS_MVC.Services;

public class CourseService : ICourseService
{
    private readonly ICourseRepository _courseRepository;
    private readonly IEnrollmentRepository _enrollmentRepository;
    private readonly IMapper _mapper;
    private readonly UserManager<User> _userManager;

    public CourseService(
        ICourseRepository courseRepository,
        IEnrollmentRepository enrollmentRepository,
        IMapper mapper,
        UserManager<User> userManager)
    {
        _courseRepository = courseRepository;
        _enrollmentRepository = enrollmentRepository;
        _mapper = mapper;
        _userManager = userManager;
    }

    public async Task<List<CourseDto>> GetAllCoursesAsync()
    {
        var courses = await _courseRepository.GetAllAsync();
        return courses.Select(c => MapToDto(c)).ToList();
    }

    public async Task<CourseDto?> GetCourseByIdAsync(int id)
    {
        var course = await _courseRepository.GetByIdAsync(id);
        return course != null ? MapToDto(course) : null;
    }

    public async Task<CourseDto> CreateCourseAsync(CreateCourseDto dto, string userId)
    {
        var course = _mapper.Map<Course>(dto);
        course.CreatedBy = userId;
        course.CreatedAt = DateTime.UtcNow;
        course.UpdatedAt = DateTime.UtcNow;

        var created = await _courseRepository.CreateAsync(course);
        return MapToDto(created);
    }

    public async Task<CourseDto> UpdateCourseAsync(int id, UpdateCourseDto dto, string userId)
    {
        var course = await _courseRepository.GetByIdAsync(id);
        if (course == null)
            throw new Exception("Course not found");

        if (course.CreatedBy != userId)
            throw new UnauthorizedAccessException("You can only update your own courses");

        _mapper.Map(dto, course);
        course.UpdatedAt = DateTime.UtcNow;

        var updated = await _courseRepository.UpdateAsync(course);
        return MapToDto(updated);
    }

    public async Task DeleteCourseAsync(int id, string userId)
    {
        var course = await _courseRepository.GetByIdAsync(id);
        if (course == null)
            throw new Exception("Course not found");

        if (course.CreatedBy != userId)
            throw new UnauthorizedAccessException("You can only delete your own courses");

        await _courseRepository.DeleteAsync(id);
    }

    public async Task<List<CourseDto>> GetUserCoursesAsync(string userId)
    {
        var enrollments = await _enrollmentRepository.GetByUserIdAsync(userId);
        return enrollments.Select(e => MapToDto(e.Course!)).ToList();
    }

    private CourseDto MapToDto(Course course)
    {
        var dto = _mapper.Map<CourseDto>(course);
        dto.CreatorName = course.Creator?.Name;
        dto.LessonCount = course.Lessons?.Count ?? 0;
        dto.EnrollmentCount = course.UserCourses?.Count ?? 0;
        return dto;
    }
}

