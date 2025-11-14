using CourseManagementAPI.Models;
using CourseManagementAPI.Repositories;

namespace CourseManagementAPI.Services;

public class DiscussionService : IDiscussionService
{
    private readonly IDiscussionRepository _discussionRepository;
    private readonly ICourseRepository _courseRepository;

    public DiscussionService(IDiscussionRepository discussionRepository, ICourseRepository courseRepository)
    {
        _discussionRepository = discussionRepository;
        _courseRepository = courseRepository;
    }

    public async Task<List<Discussion>> GetAllDiscussionsAsync()
    {
        return await _discussionRepository.GetAllAsync();
    }

    public async Task<Discussion?> GetDiscussionByIdAsync(string id)
    {
        return await _discussionRepository.GetByIdAsync(id);
    }

    public async Task<List<Discussion>> GetDiscussionsByCourseIdAsync(string courseId)
    {
        return await _discussionRepository.GetByCourseIdAsync(courseId);
    }

    public async Task<Discussion> CreateDiscussionAsync(Discussion discussion, string userId)
    {
        var course = await _courseRepository.GetByIdAsync(discussion.CourseId);
        if (course == null)
        {
            throw new Exception("Course not found with id: " + discussion.CourseId);
        }

        discussion.CreatedBy = userId;
        discussion.CreatedAt = DateTime.UtcNow;
        discussion.UpdatedAt = DateTime.UtcNow;
        return await _discussionRepository.CreateAsync(discussion);
    }

    public async Task<Discussion> UpdateDiscussionAsync(string id, Discussion discussionDetails, string userId)
    {
        var discussion = await _discussionRepository.GetByIdAsync(id);
        if (discussion == null)
        {
            throw new Exception("Discussion not found with id: " + id);
        }

        if (discussion.CreatedBy != userId)
        {
            throw new Exception("You can only update your own discussions");
        }

        discussion.Title = discussionDetails.Title;
        discussion.Description = discussionDetails.Description;
        discussion.UpdatedAt = DateTime.UtcNow;
        return await _discussionRepository.UpdateAsync(discussion);
    }

    public async Task DeleteDiscussionAsync(string id, string userId)
    {
        var discussion = await _discussionRepository.GetByIdAsync(id);
        if (discussion == null)
        {
            throw new Exception("Discussion not found with id: " + id);
        }

        if (discussion.CreatedBy != userId)
        {
            throw new Exception("You can only delete your own discussions");
        }

        await _discussionRepository.DeleteAsync(id);
    }
}



