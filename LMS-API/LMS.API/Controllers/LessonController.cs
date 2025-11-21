using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LMS.API.DTOs;
using LMS.API.Services;

namespace LMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class LessonController : ControllerBase
{
    private readonly ILessonService _lessonService;

    public LessonController(ILessonService lessonService)
    {
        _lessonService = lessonService;
    }

    [HttpGet]
    public async Task<ActionResult<List<LessonDto>>> GetAllLessons()
    {
        var lessons = await _lessonService.GetAllLessonsAsync();
        return Ok(lessons);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<LessonDto>> GetLessonById(string id)
    {
        var lesson = await _lessonService.GetLessonByIdAsync(id);
        if (lesson == null)
            return NotFound();

        return Ok(lesson);
    }

    [HttpGet("course/{courseId}")]
    public async Task<ActionResult<List<LessonDto>>> GetLessonsByCourseId(string courseId)
    {
        var lessons = await _lessonService.GetLessonsByCourseIdAsync(courseId);
        return Ok(lessons);
    }

    [HttpPost]
    [Authorize(Roles = "ADMIN")]
    public async Task<ActionResult<LessonDto>> CreateLesson([FromBody] CreateLessonDto dto)
    {
        try
        {
            var lesson = await _lessonService.CreateLessonAsync(dto);
            return CreatedAtAction(nameof(GetLessonById), new { id = lesson.Id }, lesson);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "ADMIN")]
    public async Task<ActionResult<LessonDto>> UpdateLesson(string id, [FromBody] UpdateLessonDto dto)
    {
        try
        {
            var lesson = await _lessonService.UpdateLessonAsync(id, dto);
            return Ok(lesson);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> DeleteLesson(string id)
    {
        try
        {
            await _lessonService.DeleteLessonAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}

