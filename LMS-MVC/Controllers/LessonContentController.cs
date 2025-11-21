using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LMS_MVC.DTOs;
using LMS_MVC.Services;
using LMS_MVC.Extensions;

namespace LMS_MVC.Controllers;

[Authorize]
public class LessonContentController : Controller
{
    private readonly ILessonContentService _contentService;
    private readonly ILessonService _lessonService;

    public LessonContentController(
        ILessonContentService contentService,
        ILessonService lessonService)
    {
        _contentService = contentService;
        _lessonService = lessonService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(int lessonId)
    {
        var contents = await _contentService.GetContentsByLessonIdAsync(lessonId);
        var lesson = await _lessonService.GetLessonByIdAsync(lessonId);
        ViewBag.LessonId = lessonId;
        ViewBag.Lesson = lesson;
        return View(contents);
    }

    [HttpGet]
    [Authorize(Roles = "ADMIN")]
    public IActionResult Create(int lessonId)
    {
        ViewBag.LessonId = lessonId;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> Create(CreateLessonContentDto dto)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.LessonId = dto.LessonId;
            return View(dto);
        }

        try
        {
            var userId = User.GetUserId()!;
            await _contentService.CreateContentAsync(dto, userId);
            return RedirectToAction(nameof(Index), new { lessonId = dto.LessonId });
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
    }

    [HttpGet]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> Edit(int id)
    {
        var content = await _contentService.GetContentByIdAsync(id);
        if (content == null)
            return NotFound();

        var updateDto = new UpdateLessonContentDto
        {
            Type = content.Type,
            Order = content.Order,
            Title = content.Title,
            Data = content.Data
        };

        ViewBag.ContentId = id;
        ViewBag.LessonId = content.LessonId;
        return View(updateDto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> Edit(int id, UpdateLessonContentDto dto)
    {
        if (!ModelState.IsValid)
        {
            var content = await _contentService.GetContentByIdAsync(id);
            ViewBag.ContentId = id;
            ViewBag.LessonId = content?.LessonId ?? 0;
            return View(dto);
        }

        try
        {
            var userId = User.GetUserId()!;
            var content = await _contentService.UpdateContentAsync(id, dto, userId);
            return RedirectToAction(nameof(Index), new { lessonId = content.LessonId });
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
            var content = await _contentService.GetContentByIdAsync(id);
            if (content == null)
                return NotFound();

            await _contentService.DeleteContentAsync(id, userId);
            return RedirectToAction(nameof(Index), new { lessonId = content.LessonId });
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

