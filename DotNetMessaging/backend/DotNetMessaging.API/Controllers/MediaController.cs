// ============================================================================
// MEDIA CONTROLLER - FILE UPLOAD HANDLING
// ============================================================================
// This controller handles file uploads (images, videos, audio, documents) 
// for the messaging application. Files are stored on the server and messages
// are created with references to the uploaded media.
// ============================================================================

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using DotNetMessaging.API.DTOs;
using DotNetMessaging.API.Services;
using DotNetMessaging.API.Models;
using DotNetMessaging.API.Hubs;
using DotNetMessaging.API.Constants;

namespace DotNetMessaging.API.Controllers;

[ApiController]
[Route("api/[controller]")]  // Base route: /api/media
[Authorize]  // Requires JWT authentication - only logged-in users can upload files
public class MediaController : ControllerBase
{
    // Dependency injection - services needed for file upload operations
    private readonly IMessageService _messageService;      // Creates message records in database
    private readonly IWebHostEnvironment _environment;     // Provides path to wwwroot folder
    private readonly IHubContext<ChatHub> _hubContext;     // For real-time message broadcasting
    
    // Maximum file size limit: 50MB (50 * 1024 * 1024 bytes)
    // This prevents users from uploading extremely large files that could
    // consume server storage or cause performance issues
    private const long MaxFileSize = 50 * 1024 * 1024; // 50MB

    // Constructor - receives dependencies via dependency injection
    public MediaController(
        IMessageService messageService, 
        IWebHostEnvironment environment, 
        IHubContext<ChatHub> hubContext)
    {
        _messageService = messageService;
        _environment = environment;
        _hubContext = hubContext;
    }

    // ============================================================================
    // UPLOAD MEDIA ENDPOINT
    // ============================================================================
    // HTTP Method: POST
    // Route: POST /api/media/upload
    // Content-Type: multipart/form-data
    // 
    // Purpose: Uploads a file (image, video, audio, document) and creates a message
    // 
    // Request Flow:
    // 1. Client sends multipart/form-data with:
    //    - file: The actual file to upload
    //    - chatId: (optional) For one-on-one chat
    //    - groupId: (optional) For group chat
    //    - replyToMessageId: (optional) If replying to another message
    // 
    // 2. Controller validates file and saves it to disk
    // 
    // 3. Creates a message record in database with file reference
    // 
    // 4. Broadcasts message via SignalR to connected clients
    // 
    // 5. Returns the created message
    // 
    // File Storage Structure:
    // wwwroot/
    //   uploads/
    //     image/     (for images: jpg, png, gif, etc.)
    //     video/     (for videos: mp4, avi, etc.)
    //     audio/     (for audio: mp3, wav, etc.)
    //     document/   (for documents: pdf, docx, txt, etc.)
    // ============================================================================
    [HttpPost("upload")]
    public async Task<ActionResult<MessageDto>> UploadMedia(
        [FromForm] string? chatId,              // Optional: ID of one-on-one chat
        [FromForm] string? groupId,            // Optional: ID of group chat (either chatId or groupId required)
        [FromForm] string? replyToMessageId,   // Optional: ID of message being replied to
        [FromForm] IFormFile file)             // Required: The file being uploaded
    {
        // ========================================================================
        // STEP 1: VALIDATE FILE
        // ========================================================================
        // Check if file was provided and is not empty
        if (file == null || file.Length == 0)
            return BadRequest("No file uploaded");

        // Check if file size exceeds the maximum limit (50MB)
        // This prevents server storage abuse and performance issues
        if (file.Length > MaxFileSize)
            return BadRequest("File size exceeds maximum limit of 50MB");

        // ========================================================================
        // STEP 2: PREPARE FILE STORAGE
        // ========================================================================
        // Get current user ID from JWT token
        var userId = GetCurrentUserId();
        
        // Determine message type based on file's MIME type (Content-Type)
        // Examples: "image/jpeg" → Image, "video/mp4" → Video, "application/pdf" → Document
        var messageType = DetermineMessageType(file.ContentType);
        
        // Generate unique filename to prevent conflicts
        // Format: {GUID}_{original_filename}
        // Example: "a1b2c3d4-5678-90ab-cdef-1234567890ab_document.pdf"
        // The GUID ensures uniqueness even if multiple users upload files with same name
        var fileName = $"{Guid.NewGuid()}_{file.FileName}";
        
        // Build path to store file based on message type
        // Example: "C:\...\wwwroot\uploads\document"
        // _environment.WebRootPath points to the wwwroot folder
        var uploadsPath = Path.Combine(_environment.WebRootPath, "uploads", messageType.ToString().ToLower());
        
        // Create directory if it doesn't exist (e.g., first time uploading this file type)
        if (!Directory.Exists(uploadsPath))
            Directory.CreateDirectory(uploadsPath);

        // Full path where file will be saved
        var filePath = Path.Combine(uploadsPath, fileName);

        // ========================================================================
        // STEP 3: SAVE FILE TO DISK
        // ========================================================================
        // Write file to disk asynchronously
        // Using statement ensures file stream is properly disposed after use
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            // Copy uploaded file content to disk
            await file.CopyToAsync(stream);
        }

        // ========================================================================
        // STEP 4: CREATE URL FOR FILE ACCESS
        // ========================================================================
        // Build URL path that clients can use to access the file
        // Example: "/uploads/document/a1b2c3d4-5678-90ab-cdef-1234567890ab_document.pdf"
        // This URL will be served by UseStaticFiles() middleware in Program.cs
        var mediaUrl = $"/uploads/{messageType.ToString().ToLower()}/{fileName}";

        // ========================================================================
        // STEP 5: CREATE MESSAGE REQUEST
        // ========================================================================
        // Prepare data to create a message record in the database
        var request = new CreateMessageRequest
        {
            ChatId = chatId,                    // One-on-one chat ID (if applicable)
            GroupId = groupId,                   // Group chat ID (if applicable)
            ReplyToMessageId = replyToMessageId, // Message being replied to (if applicable)
            Content = file.FileName,             // Display text (shows original filename)
            Type = messageType.ToString(),        // Message type: "Image", "Video", "Audio", "Document"
            MediaUrl = mediaUrl,                  // URL to access the file
            MediaType = file.ContentType,         // MIME type (e.g., "image/jpeg", "application/pdf")
            MediaFileName = file.FileName,        // Original filename
            MediaSize = file.Length               // File size in bytes
        };

        // ========================================================================
        // STEP 6: CREATE MESSAGE AND BROADCAST
        // ========================================================================
        try
        {
            // Create message record in database
            // Service validates user has access to chat/group and creates message
            var message = await _messageService.CreateMessageAsync(request, userId);

            // ====================================================================
            // STEP 7: BROADCAST VIA SIGNALR (REAL-TIME NOTIFICATION)
            // ====================================================================
            // Notify all connected clients in the chat/group about the new message
            // This enables instant message delivery without polling
            
            if (chatId != null)
            {
                // One-on-one chat: Send to all clients in this chat's SignalR group
                await _hubContext.Clients.Group(SignalREvents.GetChatGroupName(chatId))
                    .SendAsync(SignalREvents.NewMessage, message);
            }
            else if (groupId != null)
            {
                // Group chat: Send to all clients in this group's SignalR group
                await _hubContext.Clients.Group(SignalREvents.GetGroupChatName(groupId))
                    .SendAsync(SignalREvents.NewGroupMessage, message);
            }

            // Return created message to client
            return Ok(message);
        }
        catch (Exception ex)
        {
            // ====================================================================
            // ERROR HANDLING: CLEANUP ON FAILURE
            // ====================================================================
            // If message creation fails (e.g., user doesn't have access, database error),
            // delete the uploaded file to prevent orphaned files on disk
            // This is important for storage management
            if (System.IO.File.Exists(filePath))
                System.IO.File.Delete(filePath);
            
            // Return error to client
            return BadRequest(ex.Message);
        }
    }

    // ============================================================================
    // DETERMINE MESSAGE TYPE HELPER METHOD
    // ============================================================================
    // Purpose: Categorizes uploaded file based on its MIME type (Content-Type)
    // 
    // How it works:
    // - Checks the Content-Type header from the uploaded file
    // - Maps MIME type prefixes to MessageType enum values
    // - Used to organize files into appropriate folders (image/, video/, etc.)
    // 
    // MIME Type Examples:
    // - "image/jpeg", "image/png", "image/gif" → MessageType.Image
    // - "video/mp4", "video/avi", "video/quicktime" → MessageType.Video
    // - "audio/mpeg", "audio/wav", "audio/ogg" → MessageType.Audio
    // - "application/pdf", "application/msword", "text/plain" → MessageType.Document
    // 
    // Returns: MessageType enum value (Image, Video, Audio, or Document)
    // ============================================================================
    private MessageType DetermineMessageType(string contentType)
    {
        // Check if Content-Type starts with "image/" (covers all image formats)
        if (contentType.StartsWith("image/"))
            return MessageType.Image;
        
        // Check if Content-Type starts with "video/" (covers all video formats)
        if (contentType.StartsWith("video/"))
            return MessageType.Video;
        
        // Check if Content-Type starts with "audio/" (covers all audio formats)
        if (contentType.StartsWith("audio/"))
            return MessageType.Audio;
        
        // Default to Document for everything else
        // This includes: PDFs, Word docs, text files, spreadsheets, etc.
        return MessageType.Document;
    }

    // ============================================================================
    // GET CURRENT USER ID HELPER METHOD
    // ============================================================================
    // Purpose: Extracts the current authenticated user's ID from JWT token claims
    // 
    // How it works:
    // 1. After JWT authentication middleware validates the token, it populates
    //    HttpContext.User with claims from the token
    // 
    // 2. The JWT token contains a claim with type ClaimTypes.NameIdentifier
    //    This claim holds the user's ID
    // 
    // 3. User.FindFirst() searches for a claim with the specified type
    // 
    // 4. The ! (null-forgiving operator) tells compiler we're sure it's not null
    //    (because [Authorize] ensures user is authenticated)
    // 
    // 5. .Value extracts the string value of the claim (the user ID)
    // 
    // Returns: The user ID as a string (e.g., "user123")
    // ============================================================================
    private string GetCurrentUserId()
    {
        // User property is inherited from ControllerBase
        // It contains the ClaimsPrincipal with all JWT token claims
        // 
        // FindFirst() searches for a claim by its type
        // ClaimTypes.NameIdentifier is the standard claim type for user ID
        // 
        // The ! operator asserts this will not be null (safe because [Authorize] is on controller)
        // .Value gets the actual string value of the claim
        return User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
    }

    // ============================================================================
    // NESTJS EQUIVALENT CODE:
    // ============================================================================
    // 
    // // media.controller.ts
    // import { Controller, Post, UseInterceptors, UploadedFile, Body } from '@nestjs/common';
    // import { FileInterceptor } from '@nestjs/platform-express';
    // import { UseGuards } from '@nestjs/common';
    // import { AuthGuard } from '@nestjs/passport';
    // 
    // @Controller('media')
    // @UseGuards(AuthGuard('jwt'))
    // export class MediaController {
    //   constructor(
    //     private readonly messageService: MessageService,
    //     private readonly hubContext: ChatGateway
    //   ) {}
    // 
    //   @Post('upload')
    //   @UseInterceptors(FileInterceptor('file', {
    //     limits: { fileSize: 50 * 1024 * 1024 }, // 50MB
    //     storage: diskStorage({
    //       destination: './uploads',
    //       filename: (req, file, cb) => {
    //         const uniqueName = `${uuid()}_${file.originalname}`;
    //         cb(null, uniqueName);
    //       }
    //     })
    //   }))
    //   async uploadMedia(
    //     @UploadedFile() file: Express.Multer.File,
    //     @Body('chatId') chatId?: string,
    //     @Body('groupId') groupId?: string,
    //     @Body('replyToMessageId') replyToMessageId?: string
    //   ) {
    //     if (!file) {
    //       throw new BadRequestException('No file uploaded');
    //     }
    // 
    //     const userId = req.user.userId;
    //     const messageType = this.determineMessageType(file.mimetype);
    //     const mediaUrl = `/uploads/${messageType}/${file.filename}`;
    // 
    //     const message = await this.messageService.createMessage({
    //       chatId,
    //       groupId,
    //       replyToMessageId,
    //       content: file.originalname,
    //       type: messageType,
    //       mediaUrl,
    //       mediaType: file.mimetype,
    //       mediaFileName: file.originalname,
    //       mediaSize: file.size
    //     }, userId);
    // 
    //     // Broadcast via Socket.IO
    //     if (chatId) {
    //       this.hubContext.server.to(`Chat_${chatId}`).emit('NewMessage', message);
    //     } else if (groupId) {
    //       this.hubContext.server.to(`Group_${groupId}`).emit('NewGroupMessage', message);
    //     }
    // 
    //     return message;
    //   }
    // 
    //   private determineMessageType(mimetype: string): MessageType {
    //     if (mimetype.startsWith('image/')) return MessageType.Image;
    //     if (mimetype.startsWith('video/')) return MessageType.Video;
    //     if (mimetype.startsWith('audio/')) return MessageType.Audio;
    //     return MessageType.Document;
    //   }
    // }
    // 
    // ============================================================================
    // PLAIN NODE.JS/EXPRESS EQUIVALENT CODE:
    // ============================================================================
    // 
    // // media.controller.js
    // const express = require('express');
    // const multer = require('multer');
    // const path = require('path');
    // const { v4: uuidv4 } = require('uuid');
    // 
    // const router = express.Router();
    // const MAX_FILE_SIZE = 50 * 1024 * 1024; // 50MB
    // 
    // // Configure multer for file uploads
    // const storage = multer.diskStorage({
    //   destination: (req, file, cb) => {
    //     const messageType = determineMessageType(file.mimetype);
    //     const uploadPath = path.join(__dirname, '../uploads', messageType);
    //     require('fs').mkdirSync(uploadPath, { recursive: true });
    //     cb(null, uploadPath);
    //   },
    //   filename: (req, file, cb) => {
    //     const uniqueName = `${uuidv4()}_${file.originalname}`;
    //     cb(null, uniqueName);
    //   }
    // });
    // 
    // const upload = multer({
    //   storage: storage,
    //   limits: { fileSize: MAX_FILE_SIZE }
    // });
    // 
    // // POST /api/media/upload
    // router.post('/upload', authenticateToken, upload.single('file'), async (req, res) => {
    //   try {
    //     if (!req.file) {
    //       return res.status(400).json({ error: 'No file uploaded' });
    //     }
    // 
    //     const { chatId, groupId, replyToMessageId } = req.body;
    //     const userId = req.user.sub || req.user.nameid;
    //     const messageType = determineMessageType(req.file.mimetype);
    //     const mediaUrl = `/uploads/${messageType}/${req.file.filename}`;
    // 
    //     const message = await messageService.createMessage({
    //       chatId,
    //       groupId,
    //       replyToMessageId,
    //       content: req.file.originalname,
    //       type: messageType,
    //       mediaUrl,
    //       mediaType: req.file.mimetype,
    //       mediaFileName: req.file.originalname,
    //       mediaSize: req.file.size
    //     }, userId);
    // 
    //     // Broadcast via Socket.IO
    //     if (chatId) {
    //       io.to(`Chat_${chatId}`).emit('NewMessage', message);
    //     } else if (groupId) {
    //       io.to(`Group_${groupId}`).emit('NewGroupMessage', message);
    //     }
    // 
    //     res.json(message);
    //   } catch (error) {
    //     // Cleanup on error
    //     if (req.file && require('fs').existsSync(req.file.path)) {
    //       require('fs').unlinkSync(req.file.path);
    //     }
    //     res.status(400).json({ error: error.message });
    //   }
    // });
    // 
    // function determineMessageType(mimetype) {
    //   if (mimetype.startsWith('image/')) return 'Image';
    //   if (mimetype.startsWith('video/')) return 'Video';
    //   if (mimetype.startsWith('audio/')) return 'Audio';
    //   return 'Document';
    // }
    // 
    // module.exports = router;
    // ============================================================================
}
