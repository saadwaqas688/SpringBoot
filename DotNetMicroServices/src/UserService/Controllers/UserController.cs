using Microsoft.AspNetCore.Mvc;
using UserService.DTOs;
using UserService.Models;
using UserService.Services;
using Shared.Common;

namespace UserService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ILogger<UserController> _logger;

    public UserController(IUserService userService, ILogger<UserController> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<User>>>> GetAllUsers()
    {
        try
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(ApiResponse<List<User>>.SuccessResponse(users, "Users retrieved successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving users");
            return StatusCode(500, ApiResponse<List<User>>.ErrorResponse("An error occurred while retrieving users"));
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<User>>> GetUserById(Guid id)
    {
        try
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound(ApiResponse<User>.ErrorResponse("User not found"));
            }

            return Ok(ApiResponse<User>.SuccessResponse(user, "User retrieved successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user {UserId}", id);
            return StatusCode(500, ApiResponse<User>.ErrorResponse("An error occurred while retrieving user"));
        }
    }

    [HttpGet("email/{email}")]
    public async Task<ActionResult<ApiResponse<User>>> GetUserByEmail(string email)
    {
        try
        {
            var user = await _userService.GetUserByEmailAsync(email);
            if (user == null)
            {
                return NotFound(ApiResponse<User>.ErrorResponse("User not found"));
            }

            return Ok(ApiResponse<User>.SuccessResponse(user, "User retrieved successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user by email {Email}", email);
            return StatusCode(500, ApiResponse<User>.ErrorResponse("An error occurred while retrieving user"));
        }
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<User>>> CreateUser([FromBody] CreateUserDto dto)
    {
        try
        {
            var existingUser = await _userService.GetUserByEmailAsync(dto.Email);
            if (existingUser != null)
            {
                return BadRequest(ApiResponse<User>.ErrorResponse("User with this email already exists"));
            }

            var user = new User
            {
                Username = dto.Username,
                Email = dto.Email,
                FirstName = dto.FirstName,
                LastName = dto.LastName
            };

            var createdUser = await _userService.CreateUserAsync(user);
            return CreatedAtAction(nameof(GetUserById), new { id = createdUser.Id },
                ApiResponse<User>.SuccessResponse(createdUser, "User created successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating user");
            return StatusCode(500, ApiResponse<User>.ErrorResponse("An error occurred while creating user"));
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<User>>> UpdateUser(Guid id, [FromBody] UpdateUserDto dto)
    {
        try
        {
            var existingUser = await _userService.GetUserByIdAsync(id);
            if (existingUser == null)
            {
                return NotFound(ApiResponse<User>.ErrorResponse("User not found"));
            }

            if (dto.Username != null) existingUser.Username = dto.Username;
            if (dto.Email != null) existingUser.Email = dto.Email;
            if (dto.FirstName != null) existingUser.FirstName = dto.FirstName;
            if (dto.LastName != null) existingUser.LastName = dto.LastName;

            var updatedUser = await _userService.UpdateUserAsync(id, existingUser);
            if (updatedUser == null)
            {
                return NotFound(ApiResponse<User>.ErrorResponse("User not found"));
            }

            return Ok(ApiResponse<User>.SuccessResponse(updatedUser, "User updated successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user {UserId}", id);
            return StatusCode(500, ApiResponse<User>.ErrorResponse("An error occurred while updating user"));
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteUser(Guid id)
    {
        try
        {
            var deleted = await _userService.DeleteUserAsync(id);
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
}

