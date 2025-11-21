using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LMS_MVC.DTOs;
using LMS_MVC.Services;
using LMS_MVC.Extensions;

namespace LMS_MVC.Controllers;

[Authorize]
public class LessonProgressController : Controller
{
    private readonly ILessonProgressService _progressService;

    public LessonProgressController(ILessonProgressService progressService)
    {
        _progressService = progressService;
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(int lessonId, int courseId, UpdateLessonProgressDto dto)
    {
        try
        {
            var userId = User.GetUserId()!;
            await _progressService.UpdateProgressAsync(userId, lessonId, courseId, dto);
            return RedirectToAction("Details", "Lesson", new { id = lessonId });
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
            return RedirectToAction("Details", "Lesson", new { id = lessonId });
        }
    }

    [HttpGet]
    public async Task<IActionResult> CourseProgress(int courseId)
    {
        var userId = User.Identity!.Name!;
        var progresses = await _progressService.GetProgressByCourseAsync(userId, courseId);
        ViewBag.CourseId = courseId;
        return View(progresses);
    }
}

