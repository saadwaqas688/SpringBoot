using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using DotNetMessaging.API.DTOs;
using DotNetMessaging.API.Services;
using DotNetMessaging.API.Models;
using DotNetMessaging.API.Hubs;

namespace DotNetMessaging.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MediaController : ControllerBase
{
    private readonly IMessageService _messageService;
    private readonly IWebHostEnvironment _environment;
    private readonly IHubContext<ChatHub> _hubContext;
    private const long MaxFileSize = 50 * 1024 * 1024; // 50MB

    public MediaController(IMessageService messageService, IWebHostEnvironment environment, IHubContext<ChatHub> hubContext)
    {
        _messageService = messageService;
        _environment = environment;
        _hubContext = hubContext;
    }

    [HttpPost("upload")]
    public async Task<ActionResult<MessageDto>> UploadMedia(
        [FromForm] string? chatId,
        [FromForm] string? groupId,
        [FromForm] string? replyToMessageId,
        [FromForm] IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file uploaded");

        if (file.Length > MaxFileSize)
            return BadRequest("File size exceeds maximum limit of 50MB");

        var userId = GetCurrentUserId();
        var messageType = DetermineMessageType(file.ContentType);
        var fileName = $"{Guid.NewGuid()}_{file.FileName}";
        var uploadsPath = Path.Combine(_environment.WebRootPath, "uploads", messageType.ToString().ToLower());
        
        if (!Directory.Exists(uploadsPath))
            Directory.CreateDirectory(uploadsPath);

        var filePath = Path.Combine(uploadsPath, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        var mediaUrl = $"/uploads/{messageType.ToString().ToLower()}/{fileName}";

        var request = new CreateMessageRequest
        {
            ChatId = chatId,
            GroupId = groupId,
            ReplyToMessageId = replyToMessageId,
            Content = file.FileName,
            Type = messageType.ToString(),
            MediaUrl = mediaUrl,
            MediaType = file.ContentType,
            MediaFileName = file.FileName,
            MediaSize = file.Length
        };

        try
        {
            var message = await _messageService.CreateMessageAsync(request, userId);

            // Send via SignalR
            if (chatId != null)
            {
                await _hubContext.Clients.Group($"Chat_{chatId}").SendAsync("NewMessage", message);
            }
            else if (groupId != null)
            {
                await _hubContext.Clients.Group($"Group_{groupId}").SendAsync("NewGroupMessage", message);
            }

            return Ok(message);
        }
        catch (Exception ex)
        {
            // Clean up uploaded file if message creation fails
            if (System.IO.File.Exists(filePath))
                System.IO.File.Delete(filePath);
            
            return BadRequest(ex.Message);
        }
    }

    private MessageType DetermineMessageType(string contentType)
    {
        if (contentType.StartsWith("image/"))
            return MessageType.Image;
        if (contentType.StartsWith("video/"))
            return MessageType.Video;
        if (contentType.StartsWith("audio/"))
            return MessageType.Audio;
        
        return MessageType.Document;
    }

    private string GetCurrentUserId()
    {
        return User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
    }
}
