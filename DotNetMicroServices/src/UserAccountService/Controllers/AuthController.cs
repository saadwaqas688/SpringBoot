using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UserAccountService.DTOs;
using UserAccountService.Models;
using UserAccountService.Services;
using Shared.Common;
using System.ComponentModel.DataAnnotations;

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
                    Id = createdUser.Id ?? string.Empty,
                    Name = createdUser.Name,
                    Email = createdUser.Email,
                    Image = createdUser.Image,
                    Role = createdUser.Role,
                    Status = createdUser.Status,
                    CreatedAt = createdUser.CreatedAt
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
                    Id = user.Id ?? string.Empty,
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
            if (userIdClaim == null || string.IsNullOrEmpty(userIdClaim.Value))
            {
                return Unauthorized(ApiResponse<UserInfoDto>.ErrorResponse("Invalid token"));
            }

            var userId = userIdClaim.Value;
            var user = await _userAccountService.GetUserByIdAsync(userId);
            if (user == null)
            {
                return NotFound(ApiResponse<UserInfoDto>.ErrorResponse("User not found"));
            }

            _logger.LogInformation("UpdateProfile - Received DTO: Name={Name}, Gender={Gender}, DateOfBirth={DateOfBirth}, MobilePhone={MobilePhone}, Country={Country}, State={State}, City={City}, PostalCode={PostalCode}",
                dto.Name, dto.Gender, dto.DateOfBirth, dto.MobilePhone, dto.Country, dto.State, dto.City, dto.PostalCode);

            // Update user profile - only update fields that are provided in DTO
            // Pass the DTO and existing user to the service method
            var result = await _userAccountService.UpdateUserAsync(userId, user, dto);
            
            _logger.LogInformation("UpdateProfile - Updated user: Gender={Gender}, DateOfBirth={DateOfBirth}, MobilePhone={MobilePhone}, Country={Country}, State={State}, City={City}, PostalCode={PostalCode}",
                result?.Gender, result?.DateOfBirth, result?.MobilePhone, result?.Country, result?.State, result?.City, result?.PostalCode);
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
                Role = result.Role,
                Gender = result.Gender,
                DateOfBirth = result.DateOfBirth,
                MobilePhone = result.MobilePhone,
                Country = result.Country,
                State = result.State,
                City = result.City,
                PostalCode = result.PostalCode
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
            if (userIdClaim == null || string.IsNullOrEmpty(userIdClaim.Value))
            {
                return Unauthorized(ApiResponse<UserInfoDto>.ErrorResponse("Invalid token"));
            }

            var userId = userIdClaim.Value;
            var user = await _userAccountService.GetUserByIdAsync(userId);
            if (user == null)
            {
                return NotFound(ApiResponse<UserInfoDto>.ErrorResponse("User not found"));
            }

            var response = new UserInfoDto
            {
                Id = user.Id ?? string.Empty,
                Name = user.Name,
                Email = user.Email,
                Image = user.Image,
                Role = user.Role,
                Status = user.Status,
                CreatedAt = user.CreatedAt,
                Gender = user.Gender,
                DateOfBirth = user.DateOfBirth,
                MobilePhone = user.MobilePhone,
                Country = user.Country,
                State = user.State,
                City = user.City,
                PostalCode = user.PostalCode
            };

            return Ok(ApiResponse<UserInfoDto>.SuccessResponse(response, "User retrieved successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving current user");
            return StatusCode(500, ApiResponse<UserInfoDto>.ErrorResponse("An error occurred while retrieving user"));
        }
    }

    [HttpPost("refresh-token")]
    public async Task<ActionResult<ApiResponse<AuthResponseDto>>> RefreshToken([FromBody] RefreshTokenDto dto)
    {
        try
        {
            // For now, we'll validate the token and generate a new one
            // In production, you'd validate the refresh token from a token store
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || string.IsNullOrEmpty(userIdClaim.Value))
            {
                return Unauthorized(ApiResponse<AuthResponseDto>.ErrorResponse("Invalid token"));
            }

            var userId = userIdClaim.Value;
            var user = await _userAccountService.GetUserByIdAsync(userId);
            if (user == null)
            {
                return NotFound(ApiResponse<AuthResponseDto>.ErrorResponse("User not found"));
            }

            var token = _authService.GenerateJwtToken(user);
            var response = new AuthResponseDto
            {
                Token = token,
                User = new UserInfoDto
                {
                    Id = user.Id ?? string.Empty,
                    Name = user.Name,
                    Email = user.Email,
                    Image = user.Image,
                    Role = user.Role
                }
            };

            return Ok(ApiResponse<AuthResponseDto>.SuccessResponse(response, "Token refreshed successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error refreshing token");
            return StatusCode(500, ApiResponse<AuthResponseDto>.ErrorResponse("An error occurred while refreshing token"));
        }
    }

    [HttpPost("forgot-password")]
    public async Task<ActionResult<ApiResponse<string>>> ForgotPassword([FromBody] ForgotPasswordDto dto)
    {
        try
        {
            var user = await _userAccountService.GetUserByEmailAsync(dto.Email);
            if (user == null)
            {
                // Don't reveal if user exists for security
                return Ok(ApiResponse<string>.SuccessResponse("If the email exists, a password reset link has been sent", "Password reset email sent"));
            }

            // In production, generate a reset token and send email
            // For now, just return success
            return Ok(ApiResponse<string>.SuccessResponse("If the email exists, a password reset link has been sent", "Password reset email sent"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing forgot password");
            return StatusCode(500, ApiResponse<string>.ErrorResponse("An error occurred while processing request"));
        }
    }

    [HttpPost("reset-password")]
    public async Task<ActionResult<ApiResponse<string>>> ResetPassword([FromBody] ResetPasswordDto dto)
    {
        try
        {
            var user = await _userAccountService.GetUserByEmailAsync(dto.Email);
            if (user == null)
            {
                return BadRequest(ApiResponse<string>.ErrorResponse("Invalid reset token or email"));
            }

            // In production, validate the reset token
            // For now, just update the password
            user.PasswordHash = _authService.HashPassword(dto.NewPassword);
            user.UpdatedAt = DateTime.UtcNow;

            var updated = await _userAccountService.UpdateUserAsync(user.Id ?? string.Empty, user, null);
            if (updated == null)
            {
                return BadRequest(ApiResponse<string>.ErrorResponse("Failed to reset password"));
            }

            return Ok(ApiResponse<string>.SuccessResponse("Password reset successfully", "Password has been reset"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error resetting password");
            return StatusCode(500, ApiResponse<string>.ErrorResponse("An error occurred while resetting password"));
        }
    }

    [HttpGet("users")]
    public async Task<ActionResult<ApiResponse<PagedResponse<UserInfoDto>>>> GetUsers(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? searchTerm = null)
    {
        try
        {
            var pagedUsers = await _userAccountService.GetAllUsersAsync(page, pageSize, searchTerm);
            
            var userDtos = pagedUsers.Items.Select(u => new UserInfoDto
            {
                Id = u.Id ?? string.Empty,
                Name = u.Name,
                Email = u.Email,
                Image = u.Image,
                Role = u.Role,
                Status = u.Status,
                CreatedAt = u.CreatedAt,
                Gender = u.Gender,
                DateOfBirth = u.DateOfBirth,
                MobilePhone = u.MobilePhone,
                Country = u.Country,
                State = u.State,
                City = u.City,
                PostalCode = u.PostalCode
            }).ToList();

            var pagedResponse = new PagedResponse<UserInfoDto>
            {
                Items = userDtos,
                PageNumber = pagedUsers.PageNumber,
                PageSize = pagedUsers.PageSize,
                TotalCount = pagedUsers.TotalCount
            };

            return Ok(ApiResponse<PagedResponse<UserInfoDto>>.SuccessResponse(pagedResponse, "Users retrieved successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving users");
            return StatusCode(500, ApiResponse<PagedResponse<UserInfoDto>>.ErrorResponse("An error occurred while retrieving users"));
        }
    }

    [HttpPost("users")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<UserInfoDto>>> CreateUser([FromBody] CreateUserDto dto)
    {
        try
        {
            // Check if user already exists
            var existingUser = await _userAccountService.GetUserByEmailAsync(dto.Email);
            if (existingUser != null)
            {
                return BadRequest(ApiResponse<UserInfoDto>.ErrorResponse("User with this email already exists"));
            }

            // Create new user
            var user = new UserAccount
            {
                Name = $"{dto.FirstName} {dto.LastName}".Trim(),
                Email = dto.Email,
                PasswordHash = _authService.HashPassword(dto.Password),
                Image = dto.Image,
                Role = dto.Role ?? "user",
                Status = dto.Status ?? "active",
                Gender = dto.Gender,
                DateOfBirth = dto.DateOfBirth,
                MobilePhone = dto.MobilePhone,
                Country = dto.Country,
                State = dto.State,
                City = dto.City,
                PostalCode = dto.PostalCode
            };

            var createdUser = await _userAccountService.CreateUserAsync(user);

            var response = new UserInfoDto
            {
                Id = createdUser.Id ?? string.Empty,
                Name = createdUser.Name,
                Email = createdUser.Email,
                Image = createdUser.Image,
                Role = createdUser.Role,
                Status = createdUser.Status,
                CreatedAt = createdUser.CreatedAt,
                Gender = createdUser.Gender,
                DateOfBirth = createdUser.DateOfBirth,
                MobilePhone = createdUser.MobilePhone,
                Country = createdUser.Country,
                State = createdUser.State,
                City = createdUser.City,
                PostalCode = createdUser.PostalCode
            };

            return Ok(ApiResponse<UserInfoDto>.SuccessResponse(response, "User created successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating user");
            return StatusCode(500, ApiResponse<UserInfoDto>.ErrorResponse("An error occurred while creating user"));
        }
    }

    [HttpPut("users/{id}")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<UserInfoDto>>> UpdateUser(string id, [FromBody] UpdateUserDto dto)
    {
        try
        {
            var updatedUser = await _userAccountService.UpdateUserAdminAsync(id, dto);
            if (updatedUser == null)
            {
                return NotFound(ApiResponse<UserInfoDto>.ErrorResponse("User not found"));
            }

            var response = new UserInfoDto
            {
                Id = updatedUser.Id ?? string.Empty,
                Name = updatedUser.Name,
                Email = updatedUser.Email,
                Image = updatedUser.Image,
                Role = updatedUser.Role,
                Status = updatedUser.Status,
                CreatedAt = updatedUser.CreatedAt,
                Gender = updatedUser.Gender,
                DateOfBirth = updatedUser.DateOfBirth,
                MobilePhone = updatedUser.MobilePhone,
                Country = updatedUser.Country,
                State = updatedUser.State,
                City = updatedUser.City,
                PostalCode = updatedUser.PostalCode
            };

            return Ok(ApiResponse<UserInfoDto>.SuccessResponse(response, "User updated successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user {UserId}", id);
            return StatusCode(500, ApiResponse<UserInfoDto>.ErrorResponse("An error occurred while updating user"));
        }
    }

    [HttpDelete("users/{id}")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteUser(string id)
    {
        try
        {
            var deleted = await _userAccountService.DeleteUserAsync(id);
            if (!deleted)
            {
                return NotFound(ApiResponse<bool>.ErrorResponse("User not found"));
            }

            return Ok(ApiResponse<bool>.SuccessResponse(true, "User deleted successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting user {UserId}", id);
            return StatusCode(500, ApiResponse<bool>.ErrorResponse("An error occurred while deleting user"));
        }
    }

    [HttpPut("users/{id}/status")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<bool>>> UpdateUserStatus(string id, [FromBody] UpdateUserStatusDto dto)
    {
        try
        {
            if (string.IsNullOrEmpty(dto.Status) || (dto.Status != "active" && dto.Status != "inactive"))
            {
                return BadRequest(ApiResponse<bool>.ErrorResponse("Status must be 'active' or 'inactive'"));
            }

            var updated = await _userAccountService.UpdateUserStatusAsync(id, dto.Status);
            if (!updated)
            {
                return NotFound(ApiResponse<bool>.ErrorResponse("User not found"));
            }

            return Ok(ApiResponse<bool>.SuccessResponse(true, "User status updated successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user status {UserId}", id);
            return StatusCode(500, ApiResponse<bool>.ErrorResponse("An error occurred while updating user status"));
        }
    }
}

public class UpdateUserStatusDto
{
    public string Status { get; set; } = string.Empty;
}

