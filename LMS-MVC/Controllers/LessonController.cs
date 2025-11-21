using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LMS_MVC.DTOs;
using LMS_MVC.Services;
using LMS_MVC.Extensions;

namespace LMS_MVC.Controllers;

[Authorize]
public class LessonController : Controller
{
    private readonly ILessonService _lessonService;
    private readonly ICourseService _courseService;
    private readonly ILessonContentService _contentService;

    public LessonController(
        ILessonService lessonService, 
        ICourseService courseService,
        ILessonContentService contentService)
    {
        _lessonService = lessonService;
        _courseService = courseService;
        _contentService = contentService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(int courseId)
    {
        var lessons = await _lessonService.GetLessonsByCourseIdAsync(courseId);
        ViewBag.CourseId = courseId;
        return View(lessons);
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var lesson = await _lessonService.GetLessonByIdAsync(id);
        if (lesson == null)
            return NotFound();

        return View(lesson);
    }

    [HttpGet]
    [Authorize(Roles = "ADMIN")]
    public IActionResult Create(int courseId)
    {
        ViewBag.CourseId = courseId;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> Create(CreateLessonDto dto)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.CourseId = dto.CourseId;
            return View(dto);
        }

        try
        {
            var userId = User.GetUserId()!;
            await _lessonService.CreateLessonAsync(dto, userId);
            return RedirectToAction(nameof(Index), new { courseId = dto.CourseId });
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
        var lesson = await _lessonService.GetLessonByIdAsync(id);
        if (lesson == null)
            return NotFound();

        var updateDto = new UpdateLessonDto
        {
            Title = lesson.Title,
            Description = lesson.Description,
            Order = lesson.Order
        };

        ViewBag.LessonId = id;
        return View(updateDto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> Edit(int id, UpdateLessonDto dto)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.LessonId = id;
            return View(dto);
        }

        try
        {
            var userId = User.GetUserId()!;
            var lesson = await _lessonService.UpdateLessonAsync(id, dto, userId);
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
            var lesson = await _lessonService.GetLessonByIdAsync(id);
            if (lesson == null)
                return NotFound();

            await _lessonService.DeleteLessonAsync(id, userId);
            return RedirectToAction(nameof(Index), new { courseId = lesson.CourseId });
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

    [HttpGet]
    [Authorize(Roles = "ADMIN")]
    public IActionResult CreateWithContent(int courseId)
    {
        var model = new CreateLessonWithContentDto
        {
            CourseId = courseId,
            Contents = new List<CreateLessonContentItemDto>()
        };
        ViewBag.CourseId = courseId;
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> CreateWithContent(CreateLessonWithContentDto dto)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.CourseId = dto.CourseId;
            return View(dto);
        }

        try
        {
            var userId = User.GetUserId()!;
            
            // Create lesson first
            var createLessonDto = new CreateLessonDto
            {
                CourseId = dto.CourseId,
                Title = dto.Title,
                Description = dto.Description,
                Order = dto.Order
            };
            
            var lesson = await _lessonService.CreateLessonAsync(createLessonDto, userId);
            
            // Create lesson contents
            foreach (var contentItem in dto.Contents)
            {
                if (string.IsNullOrWhiteSpace(contentItem.Type))
                    continue;
                    
                var contentData = new ContentDataDto();
                
                // Map data based on type
                if (contentItem.Type == "slide")
                {
                    contentData.Text = contentItem.Text;
                    contentData.ImageUrl = contentItem.ImageUrl;
                }
                else if (contentItem.Type == "video")
                {
                    contentData.VideoUrl = contentItem.VideoUrl;
                    contentData.Duration = contentItem.Duration;
                }
                else if (contentItem.Type == "quiz")
                {
                    contentData.Question = contentItem.Question;
                    contentData.Options = contentItem.Options?.Where(o => !string.IsNullOrWhiteSpace(o)).ToList() ?? new List<string>();
                    contentData.CorrectAnswer = contentItem.CorrectAnswer;
                }
                // Discussion type doesn't need data fields
                
                var createContentDto = new CreateLessonContentDto
                {
                    LessonId = lesson.Id,
                    Type = contentItem.Type,
                    Order = contentItem.Order,
                    Title = contentItem.Title,
                    Data = contentData
                };
                
                await _contentService.CreateContentAsync(createContentDto, userId);
            }
            
            TempData["Success"] = "Lesson with content created successfully!";
            return RedirectToAction(nameof(Index), new { courseId = dto.CourseId });
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
            ViewBag.CourseId = dto.CourseId;
            return View(dto);
        }
    }
}

