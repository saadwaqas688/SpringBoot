using CoursesService.Models;
using CoursesService.Repositories;
using CoursesService.DTOs;
using Shared.Common;

namespace CoursesService.Services;

public class DiscussionPostService : IDiscussionPostService
{
    private readonly IDiscussionPostRepository _postRepository;
    private readonly ILogger<DiscussionPostService> _logger;

    public DiscussionPostService(
        IDiscussionPostRepository postRepository,
        ILogger<DiscussionPostService> logger)
    {
        _postRepository = postRepository;
        _logger = logger;
    }

    public async Task<ApiResponse<List<DiscussionPostWithUserDto>>> GetPostsByLessonAsync(string lessonId, int page = 1, int pageSize = 10)
    {
        try
        {
            var posts = await _postRepository.GetPostsByLessonIdWithUsersAsync(lessonId);
            var pagedPosts = posts.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            return ApiResponse<List<DiscussionPostWithUserDto>>.SuccessResponse(pagedPosts, "Posts retrieved successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving posts for lesson {LessonId}", lessonId);
            return ApiResponse<List<DiscussionPostWithUserDto>>.ErrorResponse("An error occurred while retrieving posts");
        }
    }

    public async Task<ApiResponse<DiscussionPost>> GetPostByIdAsync(string postId)
    {
        try
        {
            var post = await _postRepository.GetByIdAsync(postId);
            if (post == null)
            {
                return ApiResponse<DiscussionPost>.ErrorResponse("Post not found");
            }
            return ApiResponse<DiscussionPost>.SuccessResponse(post, "Post retrieved successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving post {PostId}", postId);
            return ApiResponse<DiscussionPost>.ErrorResponse("An error occurred while retrieving post");
        }
    }

    public async Task<ApiResponse<DiscussionPost>> CreatePostAsync(DiscussionPost post)
    {
        try
        {
            post.CreatedAt = DateTime.UtcNow;
            post.UpdatedAt = DateTime.UtcNow;
            var created = await _postRepository.CreateAsync(post);
            return ApiResponse<DiscussionPost>.SuccessResponse(created, "Post created successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating post");
            return ApiResponse<DiscussionPost>.ErrorResponse("An error occurred while creating post");
        }
    }

    public async Task<ApiResponse<DiscussionPost>> UpdatePostAsync(string postId, DiscussionPost post)
    {
        try
        {
            post.Id = postId;
            post.UpdatedAt = DateTime.UtcNow;
            var updated = await _postRepository.UpdateAsync(postId, post);
            if (updated == null)
            {
                return ApiResponse<DiscussionPost>.ErrorResponse("Post not found");
            }
            return ApiResponse<DiscussionPost>.SuccessResponse(updated, "Post updated successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating post {PostId}", postId);
            return ApiResponse<DiscussionPost>.ErrorResponse("An error occurred while updating post");
        }
    }

    public async Task<ApiResponse<bool>> DeletePostAsync(string postId)
    {
        try
        {
            var deleted = await _postRepository.DeleteAsync(postId);
            if (!deleted)
            {
                return ApiResponse<bool>.ErrorResponse("Post not found");
            }
            return ApiResponse<bool>.SuccessResponse(true, "Post deleted successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting post {PostId}", postId);
            return ApiResponse<bool>.ErrorResponse("An error occurred while deleting post");
        }
    }

    public async Task<ApiResponse<List<DiscussionPostWithUserDto>>> GetCommentsAsync(string postId, int page = 1, int pageSize = 10)
    {
        try
        {
            var comments = await _postRepository.GetCommentsByPostIdWithUsersAsync(postId);
            var pagedComments = comments.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            return ApiResponse<List<DiscussionPostWithUserDto>>.SuccessResponse(pagedComments, "Comments retrieved successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving comments for post {PostId}", postId);
            return ApiResponse<List<DiscussionPostWithUserDto>>.ErrorResponse("An error occurred while retrieving comments");
        }
    }
}




