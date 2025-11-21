using AutoMapper;
using LMS_MVC.DTOs;
using LMS_MVC.Models;
using LMS_MVC.Repositories;

namespace LMS_MVC.Services;

public class LessonProgressService : ILessonProgressService
{
    private readonly ILessonProgressRepository _progressRepository;
    private readonly IMapper _mapper;

    public LessonProgressService(ILessonProgressRepository progressRepository, IMapper mapper)
    {
        _progressRepository = progressRepository;
        _mapper = mapper;
    }

    public async Task<LessonProgressDto?> GetProgressAsync(string userId, int lessonId)
    {
        var progress = await _progressRepository.GetByUserAndLessonAsync(userId, lessonId);
        return progress != null ? _mapper.Map<LessonProgressDto>(progress) : null;
    }

    public async Task<List<LessonProgressDto>> GetProgressByCourseAsync(string userId, int courseId)
    {
        var progresses = await _progressRepository.GetByUserAndCourseAsync(userId, courseId);
        return progresses.Select(p => _mapper.Map<LessonProgressDto>(p)).ToList();
    }

    public async Task<LessonProgressDto> UpdateProgressAsync(string userId, int lessonId, int courseId, UpdateLessonProgressDto dto)
    {
        var progress = new LessonProgress
        {
            UserId = userId,
            LessonId = lessonId,
            CourseId = courseId,
            IsCompleted = dto.IsCompleted,
            CompletedAt = dto.IsCompleted ? DateTime.UtcNow : null
        };

        var updated = await _progressRepository.CreateOrUpdateAsync(progress);
        return _mapper.Map<LessonProgressDto>(updated);
    }
}

