using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LMS_MVC.DTOs;
using LMS_MVC.Models;
using LMS_MVC.Services;

namespace LMS_MVC.Controllers;

[Authorize(Roles = "ADMIN")]
public class UserController : Controller
{
    private readonly UserManager<User> _userManager;
    private readonly ICourseService _courseService;
    private readonly IEnrollmentService _enrollmentService;

    public UserController(
        UserManager<User> userManager,
        ICourseService courseService,
        IEnrollmentService enrollmentService)
    {
        _userManager = userManager;
        _courseService = courseService;
        _enrollmentService = enrollmentService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var users = await _userManager.Users
            .OrderBy(u => u.CreatedAt)
            .ToListAsync();

        var userDtos = new List<UserDto>();
        
        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault() ?? "USER";
            
            userDtos.Add(new UserDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email ?? string.Empty,
                Role = role,
                CreatedAt = user.CreatedAt
            });
        }

        return View(userDtos);
    }

    [HttpGet]
    public async Task<IActionResult> EnrollUsers()
    {
        var courses = await _courseService.GetAllCoursesAsync();
        var users = await _userManager.Users
            .OrderBy(u => u.Name)
            .ToListAsync();

        var userDtos = users.Select(u => new UserDto
        {
            Id = u.Id,
            Name = u.Name,
            Email = u.Email ?? string.Empty
        }).ToList();

        ViewBag.Courses = courses.Select(c => new { c.Id, c.Title }).ToList();
        ViewBag.Users = userDtos.Select(u => new { u.Id, Name = $"{u.Name} ({u.Email})" }).ToList();

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EnrollUsers(EnrollUserDto dto)
    {
        if (!ModelState.IsValid)
        {
            var courses = await _courseService.GetAllCoursesAsync();
            var users = await _userManager.Users
                .OrderBy(u => u.Name)
                .ToListAsync();

            var userDtos = users.Select(u => new UserDto
            {
                Id = u.Id,
                Name = u.Name,
                Email = u.Email ?? string.Empty
            }).ToList();

            ViewBag.Courses = courses.Select(c => new { c.Id, c.Title }).ToList();
            ViewBag.Users = userDtos.Select(u => new { u.Id, Name = $"{u.Name} ({u.Email})" }).ToList();
            return View(dto);
        }

        try
        {
            await _enrollmentService.EnrollUserAsync(dto);
            TempData["Success"] = "User successfully enrolled in the course!";
            return RedirectToAction(nameof(EnrollUsers));
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
            
            var courses = await _courseService.GetAllCoursesAsync();
            var users = await _userManager.Users
                .OrderBy(u => u.Name)
                .ToListAsync();

            var userDtos = users.Select(u => new UserDto
            {
                Id = u.Id,
                Name = u.Name,
                Email = u.Email ?? string.Empty
            }).ToList();

            ViewBag.Courses = courses.Select(c => new { c.Id, c.Title }).ToList();
            ViewBag.Users = userDtos.Select(u => new { u.Id, Name = $"{u.Name} ({u.Email})" }).ToList();
            return View(dto);
        }
    }
}

