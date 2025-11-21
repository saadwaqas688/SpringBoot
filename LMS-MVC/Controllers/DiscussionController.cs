using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LMS_MVC.DTOs;
using LMS_MVC.Services;
using LMS_MVC.Extensions;

namespace LMS_MVC.Controllers;

[Authorize]
public class DiscussionController : Controller
{
    private readonly IDiscussionService _discussionService;
    private readonly ILessonContentService _contentService;

    public DiscussionController(
        IDiscussionService discussionService,
        ILessonContentService contentService)
    {
        _discussionService = discussionService;
        _contentService = contentService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(int contentId)
    {
        // Get content to retrieve lessonId
        var content = await _contentService.GetContentByIdAsync(contentId);
        if (content == null)
        {
            TempData["Error"] = "Content not found.";
            return RedirectToAction("Index", "Home");
        }

        var posts = await _discussionService.GetPostsByContentIdAsync(contentId);
        var lessonId = content.LessonId; // Get from content, not from posts
        ViewBag.ContentId = contentId;
        ViewBag.LessonId = lessonId;
        ViewBag.CurrentUserId = User.GetUserId();
        return View(posts);
    }

    [HttpGet]
    public async Task<IActionResult> ByContent(int contentId)
    {
        var posts = await _discussionService.GetPostsByContentIdAsync(contentId);
        ViewBag.ContentId = contentId;
        ViewBag.CurrentUserId = User.GetUserId();
        return View(posts);
    }

    [HttpGet]
    public async Task<IActionResult> ByLesson(int lessonId)
    {
        var posts = await _discussionService.GetPostsByLessonIdAsync(lessonId);
        ViewBag.LessonId = lessonId;
        return View(posts);
    }

    [HttpGet]
    public IActionResult Create(int contentId, int lessonId, int? parentPostId = null)
    {
        // Validate required parameters
        if (contentId <= 0)
        {
            TempData["Error"] = "Invalid content ID. Please select a valid lesson content.";
            return RedirectToAction("Index", "Home");
        }
        
        if (lessonId <= 0)
        {
            TempData["Error"] = "Invalid lesson ID. Please select a valid lesson.";
            return RedirectToAction("Index", "Home");
        }

        var dto = new CreateDiscussionPostDto
        {
            ContentId = contentId,
            LessonId = lessonId,
            ParentPostId = parentPostId
        };
        ViewBag.ContentId = contentId;
        ViewBag.LessonId = lessonId;
        ViewBag.ParentPostId = parentPostId;
        return View(dto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateDiscussionPostDto dto)
    {
        // Validate required parameters before model validation
        if (dto.ContentId <= 0 || dto.LessonId <= 0)
        {
            ModelState.AddModelError("", "Content ID and Lesson ID are required.");
            ViewBag.ContentId = dto.ContentId > 0 ? dto.ContentId : 0;
            ViewBag.LessonId = dto.LessonId > 0 ? dto.LessonId : 0;
            ViewBag.ParentPostId = dto.ParentPostId;
            return View(dto);
        }

        if (!ModelState.IsValid)
        {
            ViewBag.ContentId = dto.ContentId;
            ViewBag.LessonId = dto.LessonId;
            ViewBag.ParentPostId = dto.ParentPostId;
            return View(dto);
        }

        try
        {
            var userId = User.GetUserId()!;
            await _discussionService.CreatePostAsync(dto, userId);
            TempData["Success"] = dto.ParentPostId.HasValue ? "Reply posted successfully!" : "Post created successfully!";
            return RedirectToAction("Index", new { contentId = dto.ContentId });
        }
        catch (Exception ex)
        {
            var errorMessage = ex.Message;
            if (ex.InnerException != null)
            {
                errorMessage += $" {ex.InnerException.Message}";
            }
            TempData["Error"] = errorMessage;
            ViewBag.ContentId = dto.ContentId;
            ViewBag.LessonId = dto.LessonId;
            ViewBag.ParentPostId = dto.ParentPostId;
            return View(dto);
        }
    }

    [HttpGet]
    public IActionResult Reply(int parentPostId, int contentId, int lessonId)
    {
        return RedirectToAction("Create", new { contentId, lessonId, parentPostId });
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var post = await _discussionService.GetPostByIdAsync(id);
        if (post == null)
            return NotFound();

        var updateDto = new UpdateDiscussionPostDto
        {
            Content = post.Content
        };

        ViewBag.PostId = id;
        ViewBag.ContentId = post.ContentId;
        return View(updateDto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(int id, UpdateDiscussionPostDto dto, int contentId)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.PostId = id;
            ViewBag.ContentId = contentId;
            return View("Edit", dto);
        }

        try
        {
            var userId = User.GetUserId()!;
            var post = await _discussionService.UpdatePostAsync(id, dto, userId);
            TempData["Success"] = "Post updated successfully!";
            return RedirectToAction("Index", new { contentId = post.ContentId });
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
    public async Task<IActionResult> Delete(int id, int contentId)
    {
        try
        {
            var userId = User.GetUserId()!;
            await _discussionService.DeletePostAsync(id, userId);
            TempData["Success"] = "Post deleted successfully!";
            return RedirectToAction("Index", new { contentId });
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

