using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LMS_MVC.DTOs;
using LMS_MVC.Services;
using LMS_MVC.Extensions;

namespace LMS_MVC.Controllers;

[Authorize]
public class EnrollmentController : Controller
{
    private readonly IEnrollmentService _enrollmentService;
    private readonly ICourseService _courseService;

    public EnrollmentController(
        IEnrollmentService enrollmentService,
        ICourseService courseService)
    {
        _enrollmentService = enrollmentService;
        _courseService = courseService;
    }

    [HttpGet]
    public async Task<IActionResult> MyEnrollments()
    {
        var userId = User.GetUserId()!;
        var enrollments = await _enrollmentService.GetUserEnrollmentsAsync(userId);
        return View(enrollments);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Enroll(int courseId)
    {
        try
        {
            var userId = User.GetUserId()!;
            var dto = new EnrollUserDto
            {
                UserId = userId,
                CourseId = courseId
            };

            await _enrollmentService.EnrollUserAsync(dto);
            return RedirectToAction(nameof(MyEnrollments));
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
            return RedirectToAction("Details", "Course", new { id = courseId });
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Unenroll(int courseId)
    {
        var userId = User.GetUserId()!;
        await _enrollmentService.UnenrollUserAsync(userId, courseId);
        return RedirectToAction(nameof(MyEnrollments));
    }

    [HttpGet]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> CourseEnrollments(int courseId)
    {
        var enrollments = await _enrollmentService.GetCourseEnrollmentsAsync(courseId);
        ViewBag.CourseId = courseId;
        return View(enrollments);
    }

    [HttpGet]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> Enrollments(int? courseId = null, string? searchTerm = null)
    {
        var allCourses = await _courseService.GetAllCoursesAsync();
        ViewBag.Courses = allCourses;
        ViewBag.SelectedCourseId = courseId;
        ViewBag.SearchTerm = searchTerm;

        List<EnrollmentDto> enrollments;

        if (courseId.HasValue && courseId.Value > 0)
        {
            enrollments = await _enrollmentService.GetCourseEnrollmentsAsync(courseId.Value);
        }
        else
        {
            // Get all enrollments from all courses
            enrollments = new List<EnrollmentDto>();
            foreach (var course in allCourses)
            {
                var courseEnrollments = await _enrollmentService.GetCourseEnrollmentsAsync(course.Id);
                enrollments.AddRange(courseEnrollments);
            }
        }

        // Apply search filter if search term is provided
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            enrollments = enrollments
                .Where(e => e.CourseTitle != null && 
                           e.CourseTitle.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        // Order by course title and then by enrolled date
        enrollments = enrollments
            .OrderBy(e => e.CourseTitle)
            .ThenByDescending(e => e.EnrolledAt)
            .ToList();

        return View(enrollments);
    }
}

