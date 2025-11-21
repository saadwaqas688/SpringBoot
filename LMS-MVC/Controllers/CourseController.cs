using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LMS_MVC.DTOs;
using LMS_MVC.Services;
using LMS_MVC.Extensions;

namespace LMS_MVC.Controllers;

[Authorize]
public class CourseController : Controller
{
    private readonly ICourseService _courseService;

    public CourseController(ICourseService courseService)
    {
        _courseService = courseService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var courses = await _courseService.GetAllCoursesAsync();
        return View(courses);
    }

    [HttpGet]
    public async Task<IActionResult> MyCourses()
    {
        var userId = User.GetUserId()!;
        var courses = await _courseService.GetUserCoursesAsync(userId);
        return View(courses);
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var course = await _courseService.GetCourseByIdAsync(id);
        if (course == null)
            return NotFound();

        return View(course);
    }

    [HttpGet]
    [Authorize(Roles = "ADMIN")]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> Create(CreateCourseDto dto)
    {
        if (!ModelState.IsValid)
            return View(dto);

        var userId = User.GetUserId()!;
        await _courseService.CreateCourseAsync(dto, userId);
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> Edit(int id)
    {
        var course = await _courseService.GetCourseByIdAsync(id);
        if (course == null)
            return NotFound();

        var updateDto = new UpdateCourseDto
        {
            Title = course.Title,
            Description = course.Description,
            ThumbnailUrl = course.ThumbnailUrl,
            Category = course.Category,
            Level = course.Level
        };

        return View(updateDto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> Edit(int id, UpdateCourseDto dto)
    {
        if (!ModelState.IsValid)
            return View(dto);

        try
        {
            var userId = User.GetUserId()!;
            await _courseService.UpdateCourseAsync(id, dto, userId);
            return RedirectToAction(nameof(Details), new { id });
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (Exception)
        {
            return NotFound();
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var userId = User.GetUserId()!;
            await _courseService.DeleteCourseAsync(id, userId);
            return RedirectToAction(nameof(Index));
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (Exception)
        {
            return NotFound();
        }
    }
}

