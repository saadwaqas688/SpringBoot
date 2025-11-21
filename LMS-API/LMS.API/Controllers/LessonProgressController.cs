using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LMS.API.DTOs;
using LMS.API.Services;
using System.Security.Claims;

namespace LMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class LessonProgressController : ControllerBase
{
    private readonly ILessonProgressService _progressService;

    public LessonProgressController(ILessonProgressService progressService)
    {
        _progressService = progressService;
    }

    [HttpPut("lesson/{lessonId}/course/{courseId}")]
    public async Task<ActionResult<LessonProgressDto>> UpdateProgress(string lessonId, string courseId, [FromBody] UpdateLessonProgressDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        try
        {
            var progress = await _progressService.UpdateProgressAsync(userId, lessonId, courseId, dto);
            return Ok(progress);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("lesson/{lessonId}")]
    public async Task<ActionResult<LessonProgressDto>> GetProgress(string lessonId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var progress = await _progressService.GetProgressAsync(userId, lessonId);
        if (progress == null)
            return NotFound();

        return Ok(progress);
    }

    [HttpGet("my-progress")]
    public async Task<ActionResult<List<LessonProgressDto>>> GetMyProgress()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var progress = await _progressService.GetUserProgressAsync(userId);
        return Ok(progress);
    }
}

