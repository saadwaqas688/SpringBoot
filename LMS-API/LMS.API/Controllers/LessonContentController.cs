using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LMS.API.DTOs;
using LMS.API.Services;
using System.Text.Json;

namespace LMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class LessonContentController : ControllerBase
{
    private readonly ILessonContentService _contentService;

    public LessonContentController(ILessonContentService contentService)
    {
        _contentService = contentService;
    }

    [HttpGet]
    public async Task<ActionResult<List<LessonContentDto>>> GetAllContents()
    {
        var contents = await _contentService.GetAllContentsAsync();
        return Ok(contents);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<LessonContentDto>> GetContentById(string id)
    {
        var content = await _contentService.GetContentByIdAsync(id);
        if (content == null)
            return NotFound();

        return Ok(content);
    }

    [HttpGet("lesson/{lessonId}")]
    public async Task<ActionResult<List<LessonContentDto>>> GetContentsByLessonId(string lessonId)
    {
        var contents = await _contentService.GetContentsByLessonIdAsync(lessonId);
        return Ok(contents);
    }

    [HttpPost]
    [Authorize(Roles = "ADMIN")]
    public async Task<ActionResult<LessonContentDto>> CreateContent([FromBody] CreateLessonContentDto dto)
    {
        try
        {
            var content = await _contentService.CreateContentAsync(dto);
            return CreatedAtAction(nameof(GetContentById), new { id = content.Id }, content);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "ADMIN")]
    public async Task<ActionResult<LessonContentDto>> UpdateContent(string id, [FromBody] UpdateLessonContentDto dto)
    {
        try
        {
            var content = await _contentService.UpdateContentAsync(id, dto);
            return Ok(content);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> DeleteContent(string id)
    {
        try
        {
            await _contentService.DeleteContentAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}

