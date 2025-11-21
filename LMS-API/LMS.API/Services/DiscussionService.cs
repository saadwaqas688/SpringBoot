using AutoMapper;
using LMS.API.DTOs;
using LMS.API.Models;
using LMS.API.Repositories;

namespace LMS.API.Services;

public class DiscussionService : IDiscussionService
{
    private readonly IDiscussionPostRepository _postRepository;
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public DiscussionService(IDiscussionPostRepository postRepository, IUserRepository userRepository, IMapper mapper)
    {
        _postRepository = postRepository;
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<List<DiscussionPostDto>> GetPostsByContentIdAsync(string contentId)
    {
        var posts = await _postRepository.GetByContentIdAsync(contentId);
        var postDtos = new List<DiscussionPostDto>();

        foreach (var post in posts.Where(p => string.IsNullOrEmpty(p.ParentPostId)))
        {
            var postDto = await MapToDtoAsync(post);
            var replies = await _postRepository.GetRepliesAsync(post.Id);
            postDto.Replies = replies.Select(r => _mapper.Map<DiscussionPostDto>(r)).ToList();
            foreach (var reply in postDto.Replies)
            {
                var replyUser = await _userRepository.GetByIdAsync(reply.UserId);
                reply.UserName = replyUser?.Name;
            }
            postDtos.Add(postDto);
        }

        return postDtos;
    }

    public async Task<DiscussionPostDto> CreatePostAsync(CreateDiscussionPostDto dto, string userId)
    {
        var post = _mapper.Map<DiscussionPost>(dto);
        post.UserId = userId;
        post.CreatedAt = DateTime.UtcNow;
        post.UpdatedAt = DateTime.UtcNow;

        var created = await _postRepository.CreateAsync(post);
        return await MapToDtoAsync(created);
    }

    public async Task<DiscussionPostDto> UpdatePostAsync(string id, UpdateDiscussionPostDto dto, string userId)
    {
        var post = await _postRepository.GetByIdAsync(id);
        if (post == null)
            throw new Exception("Post not found");

        if (post.UserId != userId)
            throw new UnauthorizedAccessException("You can only update your own posts");

        _mapper.Map(dto, post);
        post.UpdatedAt = DateTime.UtcNow;

        var updated = await _postRepository.UpdateAsync(post);
        return await MapToDtoAsync(updated);
    }

    public async Task DeletePostAsync(string id, string userId)
    {
        var post = await _postRepository.GetByIdAsync(id);
        if (post == null)
            throw new Exception("Post not found");

        if (post.UserId != userId)
            throw new UnauthorizedAccessException("You can only delete your own posts");

        await _postRepository.DeleteAsync(id);
    }

    private async Task<DiscussionPostDto> MapToDtoAsync(DiscussionPost post)
    {
        var dto = _mapper.Map<DiscussionPostDto>(post);
        var user = await _userRepository.GetByIdAsync(post.UserId);
        dto.UserName = user?.Name;
        return dto;
    }
}

