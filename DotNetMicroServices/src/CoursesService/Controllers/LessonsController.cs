using Microsoft.AspNetCore.Mvc;
using CoursesService.Models;
using CoursesService.Services;
using Shared.Core.Common;

namespace CoursesService.Controllers;

[ApiController]
[Route("api")]
public class LessonsController : ControllerBase
{
    private readonly ILessonService _lessonService;

    public LessonsController(ILessonService lessonService)
    {
        _lessonService = lessonService;
    }

    [HttpGet("courses/{courseId}/lessons")]
    public async Task<ActionResult<ApiResponse<List<Lesson>>>> GetLessonsByCourse(string courseId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var response = await _lessonService.GetLessonsByCourseAsync(courseId, page, pageSize);
        return response.Success ? Ok(response) : StatusCode(500, response);
    }

    [HttpGet("lessons/{id}")]
    public async Task<ActionResult<ApiResponse<Lesson>>> GetLessonById(string id)
    {
        var response = await _lessonService.GetLessonByIdAsync(id);
        if (!response.Success && response.Message == "Lesson not found")
        {
            return NotFound(response);
        }
        return response.Success ? Ok(response) : StatusCode(500, response);
    }

    [HttpPost("lessons")]
    public async Task<ActionResult<ApiResponse<Lesson>>> CreateLesson([FromBody] Lesson lesson)
    {
        var response = await _lessonService.CreateLessonAsync(lesson);
        if (response.Success && response.Data != null)
        {
            return CreatedAtAction(nameof(GetLessonById), new { id = response.Data.Id }, response);
        }
        return StatusCode(500, response);
    }

    [HttpPut("lessons/{id}")]
    public async Task<ActionResult<ApiResponse<Lesson>>> UpdateLesson(string id, [FromBody] Lesson lesson)
    {
        var response = await _lessonService.UpdateLessonAsync(id, lesson);
        if (!response.Success && response.Message == "Lesson not found")
        {
            return NotFound(response);
        }
        return response.Success ? Ok(response) : StatusCode(500, response);
    }

    [HttpDelete("lessons/{id}")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteLesson(string id)
    {
        var response = await _lessonService.DeleteLessonAsync(id);
        if (!response.Success && response.Message == "Lesson not found")
        {
            return NotFound(response);
        }
        return response.Success ? Ok(response) : StatusCode(500, response);
    }
}



