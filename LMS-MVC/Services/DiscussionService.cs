using AutoMapper;
using LMS_MVC.DTOs;
using LMS_MVC.Models;
using LMS_MVC.Repositories;

namespace LMS_MVC.Services;

public class DiscussionService : IDiscussionService
{
    private readonly IDiscussionPostRepository _postRepository;
    private readonly ILessonContentService _contentService;
    private readonly ILessonService _lessonService;
    private readonly IMapper _mapper;

    public DiscussionService(
        IDiscussionPostRepository postRepository,
        ILessonContentService contentService,
        ILessonService lessonService,
        IMapper mapper)
    {
        _postRepository = postRepository;
        _contentService = contentService;
        _lessonService = lessonService;
        _mapper = mapper;
    }

    public async Task<List<DiscussionPostDto>> GetPostsByContentIdAsync(int contentId)
    {
        var posts = await _postRepository.GetByContentIdAsync(contentId);
        return posts.Select(p => MapToDto(p)).ToList();
    }

    public async Task<List<DiscussionPostDto>> GetPostsByLessonIdAsync(int lessonId)
    {
        var posts = await _postRepository.GetByLessonIdAsync(lessonId);
        return posts.Select(p => MapToDto(p)).ToList();
    }

    public async Task<DiscussionPostDto?> GetPostByIdAsync(int id)
    {
        var post = await _postRepository.GetByIdAsync(id);
        return post != null ? MapToDto(post) : null;
    }

    public async Task<DiscussionPostDto> CreatePostAsync(CreateDiscussionPostDto dto, string userId)
    {
        // Validate required fields
        if (string.IsNullOrWhiteSpace(dto.Content))
            throw new Exception("Post content cannot be empty");
        
        if (dto.ContentId <= 0)
            throw new Exception("Invalid content ID");
        
        if (dto.LessonId <= 0)
            throw new Exception("Invalid lesson ID");

        // Validate that content and lesson exist
        var content = await _contentService.GetContentByIdAsync(dto.ContentId);
        if (content == null)
            throw new Exception("Content not found");

        var lesson = await _lessonService.GetLessonByIdAsync(dto.LessonId);
        if (lesson == null)
            throw new Exception("Lesson not found");

        // Validate parent post if replying
        if (dto.ParentPostId.HasValue && dto.ParentPostId.Value > 0)
        {
            var parentPost = await _postRepository.GetByIdAsync(dto.ParentPostId.Value);
            if (parentPost == null)
                throw new Exception("Parent post not found");
        }

        var post = _mapper.Map<DiscussionPost>(dto);
        post.UserId = userId;
        post.CreatedAt = DateTime.UtcNow;
        post.UpdatedAt = DateTime.UtcNow;
        
        // If ParentPostId is 0 or null, set it to null
        if (dto.ParentPostId.HasValue && dto.ParentPostId.Value <= 0)
        {
            post.ParentPostId = null;
        }

        try
        {
            var created = await _postRepository.CreateAsync(post);
            return MapToDto(created);
        }
        catch (Exception ex)
        {
            throw new Exception($"Error creating post: {ex.Message}", ex);
        }
    }

    public async Task<DiscussionPostDto> UpdatePostAsync(int id, UpdateDiscussionPostDto dto, string userId)
    {
        var post = await _postRepository.GetByIdAsync(id);
        if (post == null)
            throw new Exception("Post not found");

        if (post.UserId != userId)
            throw new UnauthorizedAccessException("You can only update your own posts");

        post.Content = dto.Content;
        post.UpdatedAt = DateTime.UtcNow;

        var updated = await _postRepository.UpdateAsync(post);
        return MapToDto(updated);
    }

    public async Task DeletePostAsync(int id, string userId)
    {
        var post = await _postRepository.GetByIdAsync(id);
        if (post == null)
            throw new Exception("Post not found");

        if (post.UserId != userId)
            throw new UnauthorizedAccessException("You can only delete your own posts");

        await _postRepository.DeleteAsync(id);
    }

    private DiscussionPostDto MapToDto(DiscussionPost post)
    {
        var dto = _mapper.Map<DiscussionPostDto>(post);
        dto.UserName = post.User?.Name;
        dto.Replies = post.Replies?.Select(r => MapToDto(r)).ToList() ?? new List<DiscussionPostDto>();
        return dto;
    }
}

