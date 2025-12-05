using Microsoft.AspNetCore.Mvc;
using Shared.Core.Common;
using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace CoursesService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UploadController : ControllerBase
{
    private readonly ILogger<UploadController> _logger;
    private readonly string _uploadPath;
    private readonly IWebHostEnvironment _env;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UploadController(
        ILogger<UploadController> logger, 
        IWebHostEnvironment env,
        IHttpContextAccessor httpContextAccessor)
    {
        _logger = logger;
        _env = env;
        _httpContextAccessor = httpContextAccessor;
        _uploadPath = Path.Combine(env.ContentRootPath, "wwwroot", "uploads", "courses");
        
        // Ensure upload directory exists
        if (!Directory.Exists(_uploadPath))
        {
            Directory.CreateDirectory(_uploadPath);
        }

        // Ensure video upload directory exists
        var videoUploadPath = Path.Combine(env.ContentRootPath, "wwwroot", "uploads", "videos");
        if (!Directory.Exists(videoUploadPath))
        {
            Directory.CreateDirectory(videoUploadPath);
        }
    }

    [HttpPost("image")]
    public async Task<ActionResult<ApiResponse<string>>> UploadImage(IFormFile file)
    {
        try
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(ApiResponse<string>.ErrorResponse("No file uploaded"));
            }

            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
            
            // Define allowed file types
            var allowedImageExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
            var allowedVideoExtensions = new[] { ".mp4", ".webm", ".ogg", ".mov", ".avi" };
            
            // Determine file type
            bool isImage = allowedImageExtensions.Contains(fileExtension);
            bool isVideo = allowedVideoExtensions.Contains(fileExtension);
            
            if (!isImage && !isVideo)
            {
                return BadRequest(ApiResponse<string>.ErrorResponse(
                    "Invalid file type. Only images (jpg, jpeg, png, gif, webp) and videos (mp4, webm, ogg, mov, avi) are allowed."));
            }

            // Apply appropriate file size limits
            if (isImage)
            {
                const long maxImageSize = 5 * 1024 * 1024; // 5MB for images
                if (file.Length > maxImageSize)
                {
                    return BadRequest(ApiResponse<string>.ErrorResponse("File size exceeds 5MB limit for images"));
                }
            }
            else if (isVideo)
            {
                const long maxVideoSize = 50 * 1024 * 1024; // 50MB for videos
                if (file.Length > maxVideoSize)
                {
                    return BadRequest(ApiResponse<string>.ErrorResponse("File size exceeds 50MB limit for videos"));
                }
            }

            // Generate unique filename
            var fileName = $"{Guid.NewGuid()}{fileExtension}";
            
            // Determine upload path based on file type
            string uploadDirectory;
            string urlPath;
            string successMessage;
            
            if (isImage)
            {
                uploadDirectory = _uploadPath;
                urlPath = "uploads/courses";
                successMessage = "Image uploaded successfully";
            }
            else // isVideo
            {
                uploadDirectory = Path.Combine(_env.ContentRootPath, "wwwroot", "uploads", "videos");
                urlPath = "uploads/videos";
                successMessage = "Video uploaded successfully";
            }
            
            var filePath = Path.Combine(uploadDirectory, fileName);

            // Save file
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Return URL
            var request = _httpContextAccessor.HttpContext?.Request;
            var baseUrl = $"{request?.Scheme}://{request?.Host}";
            var fileUrl = $"{baseUrl}/{urlPath}/{fileName}";
            
            _logger.LogInformation("File uploaded successfully: {FileName} ({Type})", fileName, isImage ? "Image" : "Video");
            return Ok(ApiResponse<string>.SuccessResponse(fileUrl, successMessage));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading file");
            return StatusCode(500, ApiResponse<string>.ErrorResponse($"Error uploading file: {ex.Message}"));
        }
    }

    [HttpDelete("image/{fileName}")]
    public ActionResult<ApiResponse<bool>> DeleteImage(string fileName)
    {
        try
        {
            // Determine file type from extension
            var fileExtension = Path.GetExtension(fileName).ToLowerInvariant();
            var allowedImageExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
            var allowedVideoExtensions = new[] { ".mp4", ".webm", ".ogg", ".mov", ".avi" };
            
            bool isImage = allowedImageExtensions.Contains(fileExtension);
            bool isVideo = allowedVideoExtensions.Contains(fileExtension);
            
            string filePath;
            string successMessage;
            
            if (isImage)
            {
                filePath = Path.Combine(_uploadPath, fileName);
                successMessage = "Image deleted successfully";
            }
            else if (isVideo)
            {
                var videoUploadPath = Path.Combine(_env.ContentRootPath, "wwwroot", "uploads", "videos");
                filePath = Path.Combine(videoUploadPath, fileName);
                successMessage = "Video deleted successfully";
            }
            else
            {
                // Try both locations
                var imagePath = Path.Combine(_uploadPath, fileName);
                var videoPath = Path.Combine(_env.ContentRootPath, "wwwroot", "uploads", "videos", fileName);
                
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                    _logger.LogInformation("File deleted successfully: {FileName}", fileName);
                    return Ok(ApiResponse<bool>.SuccessResponse(true, "File deleted successfully"));
                }
                else if (System.IO.File.Exists(videoPath))
                {
                    System.IO.File.Delete(videoPath);
                    _logger.LogInformation("File deleted successfully: {FileName}", fileName);
                    return Ok(ApiResponse<bool>.SuccessResponse(true, "File deleted successfully"));
                }
                else
                {
                    return NotFound(ApiResponse<bool>.ErrorResponse("File not found"));
                }
            }
            
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
                _logger.LogInformation("File deleted successfully: {FileName}", fileName);
                return Ok(ApiResponse<bool>.SuccessResponse(true, successMessage));
            }
            return NotFound(ApiResponse<bool>.ErrorResponse("File not found"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting file");
            return StatusCode(500, ApiResponse<bool>.ErrorResponse($"Error deleting file: {ex.Message}"));
        }
    }

}


