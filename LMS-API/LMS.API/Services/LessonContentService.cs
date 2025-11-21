using AutoMapper;
using LMS.API.DTOs;
using LMS.API.Models;
using LMS.API.Repositories;
using System.Text.Json;

namespace LMS.API.Services;

public class LessonContentService : ILessonContentService
{
    private readonly ILessonContentRepository _contentRepository;
    private readonly IMapper _mapper;

    public LessonContentService(ILessonContentRepository contentRepository, IMapper mapper)
    {
        _contentRepository = contentRepository;
        _mapper = mapper;
    }

    public async Task<List<LessonContentDto>> GetAllContentsAsync()
    {
        var contents = await _contentRepository.GetAllAsync();
        return contents.Select(c => _mapper.Map<LessonContentDto>(c)).ToList();
    }

    public async Task<LessonContentDto?> GetContentByIdAsync(string id)
    {
        var content = await _contentRepository.GetByIdAsync(id);
        return content != null ? _mapper.Map<LessonContentDto>(content) : null;
    }

    public async Task<List<LessonContentDto>> GetContentsByLessonIdAsync(string lessonId)
    {
        var contents = await _contentRepository.GetByLessonIdAsync(lessonId);
        return contents.Select(c => _mapper.Map<LessonContentDto>(c)).ToList();
    }

    public async Task<LessonContentDto> CreateContentAsync(CreateLessonContentDto dto)
    {
        var content = _mapper.Map<LessonContent>(dto);
        content.CreatedAt = DateTime.UtcNow;
        content.UpdatedAt = DateTime.UtcNow;

        var created = await _contentRepository.CreateAsync(content);
        return _mapper.Map<LessonContentDto>(created);
    }

    public async Task<LessonContentDto> UpdateContentAsync(string id, UpdateLessonContentDto dto)
    {
        var content = await _contentRepository.GetByIdAsync(id);
        if (content == null)
            throw new Exception("Content not found");

        _mapper.Map(dto, content);
        content.UpdatedAt = DateTime.UtcNow;

        var updated = await _contentRepository.UpdateAsync(content);
        return _mapper.Map<LessonContentDto>(updated);
    }

    public async Task DeleteContentAsync(string id)
    {
        if (!await _contentRepository.ExistsAsync(id))
            throw new Exception("Content not found");

        await _contentRepository.DeleteAsync(id);
    }
}

