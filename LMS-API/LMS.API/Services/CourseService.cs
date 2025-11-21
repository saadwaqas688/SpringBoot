using AutoMapper;
using LMS.API.DTOs;
using LMS.API.Models;
using LMS.API.Repositories;

namespace LMS.API.Services;

public class CourseService : ICourseService
{
    private readonly ICourseRepository _courseRepository;
    private readonly IEnrollmentRepository _enrollmentRepository;
    private readonly ILessonRepository _lessonRepository;
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public CourseService(
        ICourseRepository courseRepository,
        IEnrollmentRepository enrollmentRepository,
        ILessonRepository lessonRepository,
        IUserRepository userRepository,
        IMapper mapper)
    {
        _courseRepository = courseRepository;
        _enrollmentRepository = enrollmentRepository;
        _lessonRepository = lessonRepository;
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<List<CourseDto>> GetAllCoursesAsync()
    {
        var courses = await _courseRepository.GetAllAsync();
        var courseDtos = new List<CourseDto>();

        foreach (var course in courses)
        {
            courseDtos.Add(await MapToDtoAsync(course));
        }

        return courseDtos;
    }

    public async Task<CourseDto?> GetCourseByIdAsync(string id)
    {
        var course = await _courseRepository.GetByIdAsync(id);
        return course != null ? await MapToDtoAsync(course) : null;
    }

    public async Task<CourseDto> CreateCourseAsync(CreateCourseDto dto, string userId)
    {
        var course = _mapper.Map<Course>(dto);
        course.CreatedBy = userId;
        course.CreatedAt = DateTime.UtcNow;
        course.UpdatedAt = DateTime.UtcNow;

        var created = await _courseRepository.CreateAsync(course);
        return await MapToDtoAsync(created);
    }

    public async Task<CourseDto> UpdateCourseAsync(string id, UpdateCourseDto dto, string userId)
    {
        var course = await _courseRepository.GetByIdAsync(id);
        if (course == null)
            throw new Exception("Course not found");

        if (course.CreatedBy != userId)
            throw new UnauthorizedAccessException("You can only update your own courses");

        _mapper.Map(dto, course);
        course.UpdatedAt = DateTime.UtcNow;

        var updated = await _courseRepository.UpdateAsync(course);
        return await MapToDtoAsync(updated);
    }

    public async Task DeleteCourseAsync(string id, string userId)
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
        var courseDtos = new List<CourseDto>();

        foreach (var enrollment in enrollments)
        {
            var course = await _courseRepository.GetByIdAsync(enrollment.CourseId);
            if (course != null)
            {
                courseDtos.Add(await MapToDtoAsync(course));
            }
        }

        return courseDtos;
    }

    private async Task<CourseDto> MapToDtoAsync(Course course)
    {
        var dto = _mapper.Map<CourseDto>(course);
        
        var creator = await _userRepository.GetByIdAsync(course.CreatedBy);
        dto.CreatorName = creator?.Name;

        var lessons = await _lessonRepository.GetByCourseIdAsync(course.Id);
        dto.LessonCount = lessons.Count;

        var enrollments = await _enrollmentRepository.GetByCourseIdAsync(course.Id);
        dto.EnrollmentCount = enrollments.Count;

        return dto;
    }
}

