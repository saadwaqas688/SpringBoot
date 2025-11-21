using AutoMapper;
using LMS.API.DTOs;
using LMS.API.Models;
using LMS.API.Repositories;

namespace LMS.API.Services;

public class LessonService : ILessonService
{
    private readonly ILessonRepository _lessonRepository;
    private readonly ILessonContentRepository _contentRepository;
    private readonly IMapper _mapper;

    public LessonService(ILessonRepository lessonRepository, ILessonContentRepository contentRepository, IMapper mapper)
    {
        _lessonRepository = lessonRepository;
        _contentRepository = contentRepository;
        _mapper = mapper;
    }

    public async Task<List<LessonDto>> GetAllLessonsAsync()
    {
        var lessons = await _lessonRepository.GetAllAsync();
        var lessonDtos = new List<LessonDto>();

        foreach (var lesson in lessons)
        {
            lessonDtos.Add(await MapToDtoAsync(lesson));
        }

        return lessonDtos;
    }

    public async Task<LessonDto?> GetLessonByIdAsync(string id)
    {
        var lesson = await _lessonRepository.GetByIdAsync(id);
        return lesson != null ? await MapToDtoAsync(lesson) : null;
    }

    public async Task<List<LessonDto>> GetLessonsByCourseIdAsync(string courseId)
    {
        var lessons = await _lessonRepository.GetByCourseIdAsync(courseId);
        var lessonDtos = new List<LessonDto>();

        foreach (var lesson in lessons)
        {
            lessonDtos.Add(await MapToDtoAsync(lesson));
        }

        return lessonDtos.OrderBy(l => l.Order).ToList();
    }

    public async Task<LessonDto> CreateLessonAsync(CreateLessonDto dto)
    {
        var lesson = _mapper.Map<Lesson>(dto);
        lesson.CreatedAt = DateTime.UtcNow;
        lesson.UpdatedAt = DateTime.UtcNow;

        var created = await _lessonRepository.CreateAsync(lesson);
        return await MapToDtoAsync(created);
    }

    public async Task<LessonDto> UpdateLessonAsync(string id, UpdateLessonDto dto)
    {
        var lesson = await _lessonRepository.GetByIdAsync(id);
        if (lesson == null)
            throw new Exception("Lesson not found");

        _mapper.Map(dto, lesson);
        lesson.UpdatedAt = DateTime.UtcNow;

        var updated = await _lessonRepository.UpdateAsync(lesson);
        return await MapToDtoAsync(updated);
    }

    public async Task DeleteLessonAsync(string id)
    {
        if (!await _lessonRepository.ExistsAsync(id))
            throw new Exception("Lesson not found");

        await _lessonRepository.DeleteAsync(id);
    }

    private async Task<LessonDto> MapToDtoAsync(Lesson lesson)
    {
        var dto = _mapper.Map<LessonDto>(lesson);
        var contents = await _contentRepository.GetByLessonIdAsync(lesson.Id);
        dto.Contents = contents.Select(c => _mapper.Map<LessonContentDto>(c)).ToList();
        return dto;
    }
}

