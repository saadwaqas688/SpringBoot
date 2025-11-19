using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using DotNetMessaging.API.DTOs;
using DotNetMessaging.API.Services;

namespace DotNetMessaging.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IContactService _contactService;

    public UsersController(IUserService userService, IContactService contactService)
    {
        _userService = userService;
        _contactService = contactService;
    }

    [HttpGet("me")]
    public async Task<ActionResult<UserDto>> GetCurrentUser()
    {
        var userId = GetCurrentUserId();
        var user = await _userService.GetUserByIdAsync(userId);
        
        if (user == null)
            return NotFound();

        return Ok(user);
    }

    [HttpGet("search")]
    public async Task<ActionResult<List<UserDto>>> SearchUsers([FromQuery] string query)
    {
        var userId = GetCurrentUserId();
        var users = await _contactService.SearchUsersAsync(userId, query);
        return Ok(users);
    }

    [HttpGet("contacts")]
    public async Task<ActionResult<List<UserDto>>> GetContacts()
    {
        var userId = GetCurrentUserId();
        var contacts = await _contactService.GetUserContactsAsync(userId);
        return Ok(contacts);
    }

    [HttpPost("contacts/{contactUserId}")]
    public async Task<ActionResult> AddContact(string contactUserId)
    {
        var userId = GetCurrentUserId();
        var success = await _contactService.AddContactAsync(userId, contactUserId);

        if (success)
            return Ok();
        
        return BadRequest("Contact already exists or invalid user");
    }

    [HttpDelete("contacts/{contactUserId}")]
    public async Task<ActionResult> RemoveContact(string contactUserId)
    {
        var userId = GetCurrentUserId();
        var success = await _contactService.RemoveContactAsync(userId, contactUserId);

        if (success)
            return Ok();
        
        return NotFound();
    }

    [HttpPut("me/status")]
    public async Task<ActionResult> UpdateStatus([FromBody] string? status)
    {
        var userId = GetCurrentUserId();
        var user = await _userService.UpdateUserStatusAsync(userId, status);
        
        if (user == null)
            return NotFound();

        return Ok(user);
    }

    private string GetCurrentUserId()
    {
        return User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
    }
}
