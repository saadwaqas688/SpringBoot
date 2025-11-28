using Microsoft.AspNetCore.Mvc;
using CoursesService.Models;
using CoursesService.Repositories;
using Shared.Common;
using MongoDB.Driver;

namespace CoursesService.Controllers;

[ApiController]
[Route("api")]
public class SlidesController : ControllerBase
{
    private readonly IStandardSlideRepository _slideRepository;
    private readonly ILogger<SlidesController> _logger;

    public SlidesController(IStandardSlideRepository slideRepository, ILogger<SlidesController> logger)
    {
        _slideRepository = slideRepository;
        _logger = logger;
    }

    [HttpGet("lessons/{lessonId}/slides")]
    public async Task<ActionResult<ApiResponse<List<StandardSlide>>>> GetSlidesByLesson(string lessonId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        try
        {
            var filter = Builders<StandardSlide>.Filter.Eq(s => s.LessonId, lessonId);
            var sort = Builders<StandardSlide>.Sort.Ascending(s => s.Order);
            var pagedResult = await _slideRepository.GetPagedAsync(page, pageSize, filter, sort);
            
            // Ensure all slides have proper structure
            if (pagedResult.Items != null)
            {
                foreach (var slide in pagedResult.Items)
                {
                    // Ensure title has default value if missing
                    if (string.IsNullOrWhiteSpace(slide.Title))
                    {
                        slide.Title = "Untitled Slide";
                    }

                    // Ensure content structure is properly initialized
                    if (slide.Content == null)
                    {
                        slide.Content = new SlideContent
                        {
                            Title = slide.Title,
                            Items = new List<SlideContentItem>()
                        };
                    }
                    else
                    {
                        // Ensure content has title
                        if (string.IsNullOrWhiteSpace(slide.Content.Title))
                        {
                            slide.Content.Title = slide.Title;
                        }

                        // Ensure items list is initialized
                        if (slide.Content.Items == null)
                        {
                            slide.Content.Items = new List<SlideContentItem>();
                        }
                    }
                }
            }
            
            return Ok(ApiResponse<PagedResponse<StandardSlide>>.SuccessResponse(pagedResult, "Slides retrieved successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving slides for lesson {LessonId}", lessonId);
            return StatusCode(500, ApiResponse<List<StandardSlide>>.ErrorResponse("An error occurred while retrieving slides"));
        }
    }

    [HttpGet("slides/{id}")]
    public async Task<ActionResult<ApiResponse<StandardSlide>>> GetSlideById(string id)
    {
        try
        {
            var slide = await _slideRepository.GetByIdAsync(id);
            if (slide == null)
            {
                return NotFound(ApiResponse<StandardSlide>.ErrorResponse("Slide not found"));
            }
            
            // Ensure title has default value if missing
            if (string.IsNullOrWhiteSpace(slide.Title))
            {
                slide.Title = "Untitled Slide";
            }

            // Ensure content structure is properly initialized
            if (slide.Content == null)
            {
                slide.Content = new SlideContent
                {
                    Title = slide.Title,
                    Items = new List<SlideContentItem>()
                };
            }
            else
            {
                // Ensure content has title
                if (string.IsNullOrWhiteSpace(slide.Content.Title))
                {
                    slide.Content.Title = slide.Title;
                }

                // Ensure items list is initialized
                if (slide.Content.Items == null)
                {
                    slide.Content.Items = new List<SlideContentItem>();
                }
            }
            
            return Ok(ApiResponse<StandardSlide>.SuccessResponse(slide, "Slide retrieved successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving slide {SlideId}", id);
            return StatusCode(500, ApiResponse<StandardSlide>.ErrorResponse("An error occurred while retrieving slide"));
        }
    }

    [HttpPost("slides")]
    public async Task<ActionResult<ApiResponse<StandardSlide>>> CreateSlide([FromBody] StandardSlide slide)
    {
        try
        {
            // Set default title if not provided
            if (string.IsNullOrWhiteSpace(slide.Title))
            {
                slide.Title = "Untitled Slide";
            }

            // Ensure content is properly initialized with new structure
            if (slide.Content == null)
            {
                slide.Content = new SlideContent
                {
                    Title = slide.Title ?? "Untitled Slide",
                    Items = new List<SlideContentItem>()
                };
            }
            else
            {
                // Ensure content has title
                if (string.IsNullOrWhiteSpace(slide.Content.Title))
                {
                    slide.Content.Title = slide.Title ?? "Untitled Slide";
                }

                // Ensure items list is initialized
                if (slide.Content.Items == null)
                {
                    slide.Content.Items = new List<SlideContentItem>();
                }
            }

            var created = await _slideRepository.CreateAsync(slide);
            return CreatedAtAction(nameof(GetSlideById), new { id = created.Id },
                ApiResponse<StandardSlide>.SuccessResponse(created, "Slide created successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating slide");
            return StatusCode(500, ApiResponse<StandardSlide>.ErrorResponse("An error occurred while creating slide"));
        }
    }

    [HttpPut("slides/{id}")]
    public async Task<ActionResult<ApiResponse<StandardSlide>>> UpdateSlide(string id, [FromBody] StandardSlide slide)
    {
        try
        {
            slide.Id = id;

            // Ensure content is properly initialized with new structure
            if (slide.Content == null)
            {
                slide.Content = new SlideContent
                {
                    Title = slide.Title ?? "Untitled Slide",
                    Items = new List<SlideContentItem>()
                };
            }
            else
            {
                // Ensure content has title
                if (string.IsNullOrWhiteSpace(slide.Content.Title))
                {
                    slide.Content.Title = slide.Title ?? "Untitled Slide";
                }

                // Ensure items list is initialized
                if (slide.Content.Items == null)
                {
                    slide.Content.Items = new List<SlideContentItem>();
                }
            }

            var updated = await _slideRepository.UpdateAsync(id, slide);
            if (updated == null)
            {
                return NotFound(ApiResponse<StandardSlide>.ErrorResponse("Slide not found"));
            }
            return Ok(ApiResponse<StandardSlide>.SuccessResponse(updated, "Slide updated successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating slide {SlideId}", id);
            return StatusCode(500, ApiResponse<StandardSlide>.ErrorResponse("An error occurred while updating slide"));
        }
    }

    [HttpDelete("slides/{id}")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteSlide(string id)
    {
        try
        {
            var deleted = await _slideRepository.DeleteAsync(id);
            if (!deleted)
            {
                return NotFound(ApiResponse<bool>.ErrorResponse("Slide not found"));
            }
            return Ok(ApiResponse<bool>.SuccessResponse(true, "Slide deleted successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting slide {SlideId}", id);
            return StatusCode(500, ApiResponse<bool>.ErrorResponse("An error occurred while deleting slide"));
        }
    }
}

