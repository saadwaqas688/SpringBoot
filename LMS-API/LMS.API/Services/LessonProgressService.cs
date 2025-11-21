using AutoMapper;
using LMS.API.DTOs;
using LMS.API.Models;
using LMS.API.Repositories;

namespace LMS.API.Services;

public class LessonProgressService : ILessonProgressService
{
    private readonly ILessonProgressRepository _progressRepository;
    private readonly IMapper _mapper;

    public LessonProgressService(ILessonProgressRepository progressRepository, IMapper mapper)
    {
        _progressRepository = progressRepository;
        _mapper = mapper;
    }

    public async Task<LessonProgressDto> UpdateProgressAsync(string userId, string lessonId, string courseId, UpdateLessonProgressDto dto)
    {
        var existing = await _progressRepository.GetByUserAndLessonAsync(userId, lessonId);
        
        if (existing != null)
        {
            existing.IsCompleted = dto.IsCompleted;
            existing.CompletedAt = dto.IsCompleted ? DateTime.UtcNow : null;
            var updated = await _progressRepository.UpdateAsync(existing);
            return _mapper.Map<LessonProgressDto>(updated);
        }
        else
        {
            var progress = new LessonProgress
            {
                UserId = userId,
                LessonId = lessonId,
                CourseId = courseId,
                IsCompleted = dto.IsCompleted,
                CompletedAt = dto.IsCompleted ? DateTime.UtcNow : null
            };
            var created = await _progressRepository.CreateAsync(progress);
            return _mapper.Map<LessonProgressDto>(created);
        }
    }

    public async Task<LessonProgressDto?> GetProgressAsync(string userId, string lessonId)
    {
        var progress = await _progressRepository.GetByUserAndLessonAsync(userId, lessonId);
        return progress != null ? _mapper.Map<LessonProgressDto>(progress) : null;
    }

    public async Task<List<LessonProgressDto>> GetUserProgressAsync(string userId)
    {
        var progresses = await _progressRepository.GetByUserIdAsync(userId);
        return progresses.Select(p => _mapper.Map<LessonProgressDto>(p)).ToList();
    }
}

