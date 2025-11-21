using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LMS.API.DTOs;
using LMS.API.Services;
using System.Security.Claims;

namespace LMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EnrollmentController : ControllerBase
{
    private readonly IEnrollmentService _enrollmentService;

    public EnrollmentController(IEnrollmentService enrollmentService)
    {
        _enrollmentService = enrollmentService;
    }

    [HttpGet]
    [Authorize(Roles = "ADMIN")]
    public async Task<ActionResult<List<EnrollmentDto>>> GetAllEnrollments()
    {
        var enrollments = await _enrollmentService.GetAllEnrollmentsAsync();
        return Ok(enrollments);
    }

    [HttpPost]
    public async Task<ActionResult<EnrollmentDto>> EnrollUser([FromBody] EnrollUserDto dto)
    {
        try
        {
            var enrollment = await _enrollmentService.EnrollUserAsync(dto);
            return CreatedAtAction(nameof(GetEnrollmentById), new { id = enrollment.Id }, enrollment);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<EnrollmentDto>> GetEnrollmentById(string id)
    {
        var enrollments = await _enrollmentService.GetAllEnrollmentsAsync();
        var enrollment = enrollments.FirstOrDefault(e => e.Id == id);
        if (enrollment == null)
            return NotFound();

        return Ok(enrollment);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> DeleteEnrollment(string id)
    {
        try
        {
            await _enrollmentService.DeleteEnrollmentAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("my-enrollments")]
    public async Task<ActionResult<List<EnrollmentDto>>> GetMyEnrollments()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var enrollments = await _enrollmentService.GetUserEnrollmentsAsync(userId);
        return Ok(enrollments);
    }
}

