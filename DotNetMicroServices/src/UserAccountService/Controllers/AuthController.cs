using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UserAccountService.DTOs;
using UserAccountService.Models;
using UserAccountService.Services;
using Shared.Common;

namespace UserAccountService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserAccountService _userAccountService;
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        IUserAccountService userAccountService,
        IAuthService authService,
        ILogger<AuthController> logger)
    {
        _userAccountService = userAccountService;
        _authService = authService;
        _logger = logger;
    }

    [HttpPost("signup")]
    public async Task<ActionResult<ApiResponse<AuthResponseDto>>> SignUp([FromBody] SignUpDto dto)
    {
        try
        {
            // Check if user already exists
            var existingUser = await _userAccountService.GetUserByEmailAsync(dto.Email);
            if (existingUser != null)
            {
                return BadRequest(ApiResponse<AuthResponseDto>.ErrorResponse("User with this email already exists"));
            }

            // Create new user
            var user = new UserAccount
            {
                Name = dto.Name,
                Email = dto.Email,
                PasswordHash = _authService.HashPassword(dto.Password),
                Image = dto.Image,
                Role = "user" // Default role
            };

            var createdUser = await _userAccountService.CreateUserAsync(user);

            // Generate JWT token
            var token = _authService.GenerateJwtToken(createdUser);

            var response = new AuthResponseDto
            {
                Token = token,
                User = new UserInfoDto
                {
                    Id = createdUser.Id,
                    Name = createdUser.Name,
                    Email = createdUser.Email,
                    Image = createdUser.Image,
                    Role = createdUser.Role
                }
            };

            return Ok(ApiResponse<AuthResponseDto>.SuccessResponse(response, "User registered successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during signup");
            return StatusCode(500, ApiResponse<AuthResponseDto>.ErrorResponse("An error occurred during signup"));
        }
    }

    [HttpPost("signin")]
    public async Task<ActionResult<ApiResponse<AuthResponseDto>>> SignIn([FromBody] SignInDto dto)
    {
        try
        {
            var user = await _userAccountService.GetUserByEmailAsync(dto.Email);
            if (user == null)
            {
                return Unauthorized(ApiResponse<AuthResponseDto>.ErrorResponse("Invalid email or password"));
            }

            // Verify password
            if (!_authService.VerifyPassword(dto.Password, user.PasswordHash))
            {
                return Unauthorized(ApiResponse<AuthResponseDto>.ErrorResponse("Invalid email or password"));
            }

            // Generate JWT token
            var token = _authService.GenerateJwtToken(user);

            var response = new AuthResponseDto
            {
                Token = token,
                User = new UserInfoDto
                {
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Email,
                    Image = user.Image,
                    Role = user.Role
                }
            };

            return Ok(ApiResponse<AuthResponseDto>.SuccessResponse(response, "Sign in successful"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during signin");
            return StatusCode(500, ApiResponse<AuthResponseDto>.ErrorResponse("An error occurred during signin"));
        }
    }

    [Authorize]
    [HttpPut("profile")]
    public async Task<ActionResult<ApiResponse<UserInfoDto>>> UpdateProfile([FromBody] UpdateProfileDto dto)
    {
        try
        {
            // Get user ID from JWT token
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            {
                return Unauthorized(ApiResponse<UserInfoDto>.ErrorResponse("Invalid token"));
            }

            var user = await _userAccountService.GetUserByIdAsync(userId);
            if (user == null)
            {
                return NotFound(ApiResponse<UserInfoDto>.ErrorResponse("User not found"));
            }

            // Update user profile - only update fields that are provided in DTO
            // Pass the DTO and existing user to the service method
            var result = await _userAccountService.UpdateUserAsync(userId, user, dto);
            if (result == null)
            {
                return NotFound(ApiResponse<UserInfoDto>.ErrorResponse("User not found"));
            }

            var response = new UserInfoDto
            {
                Id = result.Id,
                Name = result.Name,
                Email = result.Email,
                Image = result.Image,
                Role = result.Role
            };

            return Ok(ApiResponse<UserInfoDto>.SuccessResponse(response, "Profile updated successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating profile");
            return StatusCode(500, ApiResponse<UserInfoDto>.ErrorResponse("An error occurred while updating profile"));
        }
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<ActionResult<ApiResponse<UserInfoDto>>> GetCurrentUser()
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            {
                return Unauthorized(ApiResponse<UserInfoDto>.ErrorResponse("Invalid token"));
            }

            var user = await _userAccountService.GetUserByIdAsync(userId);
            if (user == null)
            {
                return NotFound(ApiResponse<UserInfoDto>.ErrorResponse("User not found"));
            }

            var response = new UserInfoDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Image = user.Image,
                Role = user.Role
            };

            return Ok(ApiResponse<UserInfoDto>.SuccessResponse(response, "User retrieved successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving current user");
            return StatusCode(500, ApiResponse<UserInfoDto>.ErrorResponse("An error occurred while retrieving user"));
        }
    }
}

