using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using DotNetMessaging.API.DTOs;
using DotNetMessaging.API.Services;

namespace DotNetMessaging.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ChatsController : ControllerBase
{
    private readonly IChatService _chatService;

    public ChatsController(IChatService chatService)
    {
        _chatService = chatService;
    }

    [HttpGet]
    public async Task<ActionResult<List<ChatDto>>> GetChats()
    {
        var userId = GetCurrentUserId();
        var chats = await _chatService.GetUserChatsAsync(userId);
        return Ok(chats);
    }

    [HttpPost("with/{otherUserId}")]
    public async Task<ActionResult<ChatDto>> GetOrCreateChat(string otherUserId)
    {
        var userId = GetCurrentUserId();

        if (userId == otherUserId)
            return BadRequest("Cannot create chat with yourself");

        var chat = await _chatService.GetOrCreateChatAsync(userId, otherUserId);
        
        if (chat == null)
            return NotFound("Other user not found");

        return Ok(chat);
    }

    private string GetCurrentUserId()
    {
        return User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
    }
}
