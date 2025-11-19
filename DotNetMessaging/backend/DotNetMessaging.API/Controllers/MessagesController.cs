using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using DotNetMessaging.API.DTOs;
using DotNetMessaging.API.Services;
using DotNetMessaging.API.Hubs;

namespace DotNetMessaging.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MessagesController : ControllerBase
{
    private readonly IMessageService _messageService;
    private readonly IHubContext<ChatHub> _hubContext;

    public MessagesController(IMessageService messageService, IHubContext<ChatHub> hubContext)
    {
        _messageService = messageService;
        _hubContext = hubContext;
    }

    [HttpGet("chat/{chatId}")]
    public async Task<ActionResult<List<MessageDto>>> GetChatMessages(string chatId, [FromQuery] int skip = 0, [FromQuery] int take = 50)
    {
        try
    {
        var userId = GetCurrentUserId();
            var messages = await _messageService.GetChatMessagesAsync(chatId, userId, skip, take);
            // Mark messages as read when loading them
            await _messageService.MarkChatAsReadAsync(chatId, userId);
            return Ok(messages);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
    }

    [HttpGet("group/{groupId}")]
    public async Task<ActionResult<List<MessageDto>>> GetGroupMessages(string groupId, [FromQuery] int skip = 0, [FromQuery] int take = 50)
    {
        try
        {
            var userId = GetCurrentUserId();
            var messages = await _messageService.GetGroupMessagesAsync(groupId, userId, skip, take);
            // Mark messages as read when loading them
            await _messageService.MarkGroupAsReadAsync(groupId, userId);
            return Ok(messages);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
    }

    [HttpPost]
    public async Task<ActionResult<MessageDto>> CreateMessage([FromBody] CreateMessageRequest request)
    {
        try
        {
            var userId = GetCurrentUserId();
            var message = await _messageService.CreateMessageAsync(request, userId);

            // Send via SignalR
            if (request.ChatId != null)
            {
                await _hubContext.Clients.Group($"Chat_{request.ChatId}").SendAsync("NewMessage", message);
            }
            else if (request.GroupId != null)
            {
                await _hubContext.Clients.Group($"Group_{request.GroupId}").SendAsync("NewGroupMessage", message);
            }

            return Ok(message);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
    }

    [HttpPost("{messageId}/reaction")]
    public async Task<ActionResult> AddReaction(string messageId, [FromBody] AddReactionRequest request)
    {
        var userId = GetCurrentUserId();
        var success = await _messageService.AddReactionAsync(messageId, userId, request.Emoji);

        if (success)
        {
            // Notify via SignalR - we'll need to get the message to determine chat/group
            var message = await _messageService.GetMessageByIdAsync(messageId);
            if (message != null)
            {
                if (message.ChatId != null)
                {
                    await _hubContext.Clients.Group($"Chat_{message.ChatId}").SendAsync("MessageReactionUpdated", message);
                }
                else if (message.GroupId != null)
                {
                    await _hubContext.Clients.Group($"Group_{message.GroupId}").SendAsync("MessageReactionUpdated", message);
                }
            }
            return Ok();
        }

        return NotFound();
    }

    [HttpDelete("{messageId}")]
    public async Task<ActionResult> DeleteMessage(string messageId)
    {
        var userId = GetCurrentUserId();
        var success = await _messageService.DeleteMessageAsync(messageId, userId);

        if (success)
        {
            // Notify via SignalR
            await _hubContext.Clients.All.SendAsync("MessageDeleted", messageId);
            return Ok();
        }

        return NotFound();
    }

    private string GetCurrentUserId()
    {
        return User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
    }
}
