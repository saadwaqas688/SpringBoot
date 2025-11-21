using AutoMapper;
using LMS_MVC.DTOs;
using LMS_MVC.Models;
using LMS_MVC.Repositories;

namespace LMS_MVC.Services;

public class LessonContentService : ILessonContentService
{
    private readonly ILessonContentRepository _contentRepository;
    private readonly ILessonRepository _lessonRepository;
    private readonly ICourseRepository _courseRepository;
    private readonly IMapper _mapper;

    public LessonContentService(
        ILessonContentRepository contentRepository,
        ILessonRepository lessonRepository,
        ICourseRepository courseRepository,
        IMapper mapper)
    {
        _contentRepository = contentRepository;
        _lessonRepository = lessonRepository;
        _courseRepository = courseRepository;
        _mapper = mapper;
    }

    public async Task<List<LessonContentDto>> GetContentsByLessonIdAsync(int lessonId)
    {
        var contents = await _contentRepository.GetByLessonIdAsync(lessonId);
        return contents.Select(c => MapToDto(c)).ToList();
    }

    public async Task<LessonContentDto?> GetContentByIdAsync(int id)
    {
        var content = await _contentRepository.GetByIdAsync(id);
        return content != null ? MapToDto(content) : null;
    }

    public async Task<LessonContentDto> CreateContentAsync(CreateLessonContentDto dto, string userId)
    {
        var lesson = await _lessonRepository.GetByIdAsync(dto.LessonId);
        if (lesson == null)
            throw new Exception("Lesson not found");

        var course = await _courseRepository.GetByIdAsync(lesson.CourseId);
        if (course == null || course.CreatedBy != userId)
            throw new UnauthorizedAccessException("You can only add content to lessons in your own courses");

        var content = _mapper.Map<LessonContent>(dto);
        content.Data = dto.Data != null 
            ? System.Text.Json.JsonSerializer.Serialize(dto.Data) 
            : string.Empty;
        content.CreatedAt = DateTime.UtcNow;
        content.UpdatedAt = DateTime.UtcNow;

        var created = await _contentRepository.CreateAsync(content);
        return MapToDto(created);
    }

    public async Task<LessonContentDto> UpdateContentAsync(int id, UpdateLessonContentDto dto, string userId)
    {
        var content = await _contentRepository.GetByIdAsync(id);
        if (content == null)
            throw new Exception("Content not found");

        var lesson = await _lessonRepository.GetByIdAsync(content.LessonId);
        if (lesson == null)
            throw new Exception("Lesson not found");

        var course = await _courseRepository.GetByIdAsync(lesson.CourseId);
        if (course == null || course.CreatedBy != userId)
            throw new UnauthorizedAccessException("You can only update content in your own courses");

        _mapper.Map(dto, content);
        content.Data = dto.Data != null 
            ? System.Text.Json.JsonSerializer.Serialize(dto.Data) 
            : string.Empty;
        content.UpdatedAt = DateTime.UtcNow;

        var updated = await _contentRepository.UpdateAsync(content);
        return MapToDto(updated);
    }

    public async Task DeleteContentAsync(int id, string userId)
    {
        var content = await _contentRepository.GetByIdAsync(id);
        if (content == null)
            throw new Exception("Content not found");

        var lesson = await _lessonRepository.GetByIdAsync(content.LessonId);
        if (lesson == null)
            throw new Exception("Lesson not found");

        var course = await _courseRepository.GetByIdAsync(lesson.CourseId);
        if (course == null || course.CreatedBy != userId)
            throw new UnauthorizedAccessException("You can only delete content in your own courses");

        await _contentRepository.DeleteAsync(id);
    }

    private LessonContentDto MapToDto(LessonContent content)
    {
        var dto = _mapper.Map<LessonContentDto>(content);
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

