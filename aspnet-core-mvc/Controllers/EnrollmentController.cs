using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CourseManagementAPI.DTOs;
using CourseManagementAPI.Models;
using CourseManagementAPI.Services;
using CourseManagementAPI.Repositories;

namespace CourseManagementAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EnrollmentController : ControllerBase
{
    private readonly IEnrollmentService _enrollmentService;
    private readonly IJwtService _jwtService;
    private readonly IUserRepository _userRepository;

    public EnrollmentController(
        IEnrollmentService enrollmentService,
        IJwtService jwtService,
        IUserRepository userRepository)
    {
        _enrollmentService = enrollmentService;
        _jwtService = jwtService;
        _userRepository = userRepository;
    }

    [HttpPost]
    public async Task<ActionResult<List<CourseEnrollment>>> EnrollUsers([FromBody] EnrollUsersRequest request)
    {
        try
        {
            var grantedBy = _jwtService.GetUserIdFromToken(Request.Headers["Authorization"].ToString().Replace("Bearer ", ""));
            var enrollments = await _enrollmentService.EnrollUsersAsync(request.UserIds, request.CourseId, grantedBy);
            return StatusCode(201, enrollments);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("course/{courseId}")]
    public async Task<ActionResult<List<EnrolledUserDto>>> GetEnrolledUsers(string courseId)
    {
        try
        {
            var enrollments = await _enrollmentService.GetEnrollmentsByCourseIdAsync(courseId);
            var enrolledUsers = new List<EnrolledUserDto>();

            foreach (var enrollment in enrollments)
            {
                var user = await _userRepository.GetByIdAsync(enrollment.UserId);
                if (user != null)
                {
                    enrolledUsers.Add(new EnrolledUserDto
                    {
                        UserId = user.Id!,
                        Email = user.Email,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        EnrolledAt = enrollment.EnrolledAt
                    });
                }
            }

            return Ok(enrolledUsers);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("my-courses")]
    public async Task<ActionResult<List<Course>>> GetMyCourses()
    {
        try
        {
            var userId = _jwtService.GetUserIdFromToken(Request.Headers["Authorization"].ToString().Replace("Bearer ", ""));
            var courses = await _enrollmentService.GetCoursesByUserIdAsync(userId);
            return Ok(courses);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}



