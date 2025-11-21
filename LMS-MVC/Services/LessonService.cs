using AutoMapper;
using LMS_MVC.DTOs;
using LMS_MVC.Models;
using LMS_MVC.Repositories;

namespace LMS_MVC.Services;

public class LessonService : ILessonService
{
    private readonly ILessonRepository _lessonRepository;
    private readonly ICourseRepository _courseRepository;
    private readonly ILessonContentRepository _contentRepository;
    private readonly IMapper _mapper;

    public LessonService(
        ILessonRepository lessonRepository,
        ICourseRepository courseRepository,
        ILessonContentRepository contentRepository,
        IMapper mapper)
    {
        _lessonRepository = lessonRepository;
        _courseRepository = courseRepository;
        _contentRepository = contentRepository;
        _mapper = mapper;
    }

    public async Task<List<LessonDto>> GetLessonsByCourseIdAsync(int courseId)
    {
        var lessons = await _lessonRepository.GetByCourseIdAsync(courseId);
        return lessons.Select(l => MapToDto(l)).ToList();
    }

    public async Task<LessonDto?> GetLessonByIdAsync(int id)
    {
        var lesson = await _lessonRepository.GetByIdAsync(id);
        return lesson != null ? MapToDto(lesson) : null;
    }

    public async Task<LessonDto> CreateLessonAsync(CreateLessonDto dto, string userId)
    {
        var course = await _courseRepository.GetByIdAsync(dto.CourseId);
        if (course == null)
            throw new Exception("Course not found");

        if (course.CreatedBy != userId)
            throw new UnauthorizedAccessException("You can only add lessons to your own courses");

        var lesson = _mapper.Map<Lesson>(dto);
        lesson.CreatedAt = DateTime.UtcNow;
        lesson.UpdatedAt = DateTime.UtcNow;

        var created = await _lessonRepository.CreateAsync(lesson);
        return MapToDto(created);
    }

    public async Task<LessonDto> UpdateLessonAsync(int id, UpdateLessonDto dto, string userId)
    {
        var lesson = await _lessonRepository.GetByIdAsync(id);
        if (lesson == null)
            throw new Exception("Lesson not found");

        var course = await _courseRepository.GetByIdAsync(lesson.CourseId);
        if (course == null || course.CreatedBy != userId)
            throw new UnauthorizedAccessException("You can only update lessons in your own courses");

        _mapper.Map(dto, lesson);
        lesson.UpdatedAt = DateTime.UtcNow;

        var updated = await _lessonRepository.UpdateAsync(lesson);
        return MapToDto(updated);
    }

    public async Task DeleteLessonAsync(int id, string userId)
    {
        var lesson = await _lessonRepository.GetByIdAsync(id);
        if (lesson == null)
            throw new Exception("Lesson not found");

        var course = await _courseRepository.GetByIdAsync(lesson.CourseId);
        if (course == null || course.CreatedBy != userId)
            throw new UnauthorizedAccessException("You can only delete lessons in your own courses");

        await _lessonRepository.DeleteAsync(id);
    }

    private LessonDto MapToDto(Lesson lesson)
    {
        var dto = _mapper.Map<LessonDto>(lesson);
        var contents = lesson.Contents?.Select(c => MapContentToDto(c)).ToList() ?? new List<LessonContentDto>();
        dto.Contents = contents;
        return dto;
    }

    private LessonContentDto MapContentToDto(LessonContent content)
    {
        var dto = _mapper.Map<LessonContentDto>(content);
        // Deserialize JSON data
        if (!string.IsNullOrEmpty(content.Data))
        {
            try
            {
                dto.Data = System.Text.Json.JsonSerializer.Deserialize<ContentDataDto>(content.Data);
            }
            catch { }
        }
        return dto;
    }
}

