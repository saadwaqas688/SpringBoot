using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Gateway.DTOs;
using Gateway.Services;
using Shared.Common;

namespace Gateway.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserAccountGatewayService _userAccountGatewayService;
    private readonly ICoursesGatewayService _coursesGatewayService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        IUserAccountGatewayService userAccountGatewayService,
        ICoursesGatewayService coursesGatewayService,
        ILogger<AuthController> logger)
    {
        _userAccountGatewayService = userAccountGatewayService;
        _coursesGatewayService = coursesGatewayService;
        _logger = logger;
    }

    [HttpPost("signup")]
    public async Task<ActionResult<ApiResponse<AuthResponseDto>>> SignUp([FromBody] SignUpDto dto)
    {
        var response = await _userAccountGatewayService.SignUpAsync(dto);
        return StatusCode(response.Success ? 200 : 400, response);
    }

    [HttpPost("signin")]
    public async Task<ActionResult<ApiResponse<AuthResponseDto>>> SignIn([FromBody] SignInDto dto)
    {
        try
        {
            var response = await _userAccountGatewayService.SignInAsync(dto);
            return StatusCode(response.Success ? 200 : 401, response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during signin");
            return StatusCode(500, ApiResponse<AuthResponseDto>.ErrorResponse(
                ex.Message.Contains("RabbitMQ") || ex.Message.Contains("BrokerUnreachable")
                    ? "Service temporarily unavailable. RabbitMQ connection failed. Please check if RabbitMQ is running."
                    : "An unexpected error occurred during signin"));
        }
    }

    [Authorize]
    [HttpPut("profile")]
    public async Task<ActionResult<ApiResponse<UserInfoDto>>> UpdateProfile([FromBody] UpdateProfileDto dto)
    {
        var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        if (string.IsNullOrEmpty(token))
        {
            return Unauthorized(ApiResponse<UserInfoDto>.ErrorResponse("Authorization token is required"));
        }

        var response = await _userAccountGatewayService.UpdateProfileAsync(token, dto);
        return StatusCode(response.Success ? 200 : 400, response);
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<ActionResult<ApiResponse<UserInfoDto>>> GetCurrentUser()
    {
        var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        if (string.IsNullOrEmpty(token))
        {
            return Unauthorized(ApiResponse<UserInfoDto>.ErrorResponse("Authorization token is required"));
        }

        var response = await _userAccountGatewayService.GetCurrentUserAsync(token);
        return StatusCode(response.Success ? 200 : 401, response);
    }

    [HttpPost("refresh-token")]
    public async Task<ActionResult<ApiResponse<AuthResponseDto>>> RefreshToken([FromBody] RefreshTokenDto dto)
    {
        var response = await _userAccountGatewayService.RefreshTokenAsync(dto);
        return StatusCode(response.Success ? 200 : 401, response);
    }

    [HttpPost("forgot-password")]
    public async Task<ActionResult<ApiResponse<string>>> ForgotPassword([FromBody] ForgotPasswordDto dto)
    {
        var response = await _userAccountGatewayService.ForgotPasswordAsync(dto);
        return StatusCode(response.Success ? 200 : 400, response);
    }

    [HttpPost("reset-password")]
    public async Task<ActionResult<ApiResponse<string>>> ResetPassword([FromBody] ResetPasswordDto dto)
    {
        var response = await _userAccountGatewayService.ResetPasswordAsync(dto);
        return StatusCode(response.Success ? 200 : 400, response);
    }

    [Authorize]
    [HttpGet("users")]
    public async Task<ActionResult<ApiResponse<PagedResponse<UserInfoDto>>>> GetAllUsers(
        [FromQuery] int page = 1, 
        [FromQuery] int pageSize = 10, 
        [FromQuery] string? searchTerm = null)
    {
        var response = await _userAccountGatewayService.GetAllUsersAsync(page, pageSize, searchTerm);
        return StatusCode(response.Success ? 200 : 500, response);
    }

    [Authorize]
    [HttpPost("users")]
    public async Task<ActionResult<ApiResponse<UserInfoDto>>> CreateUser([FromBody] CreateUserDto dto)
    {
        var response = await _userAccountGatewayService.CreateUserAsync(dto);
        return StatusCode(response.Success ? 200 : 400, response);
    }

    [Authorize]
    [HttpPut("users/{id}")]
    public async Task<ActionResult<ApiResponse<UserInfoDto>>> UpdateUser(string id, [FromBody] UpdateUserDto dto)
    {
        var response = await _userAccountGatewayService.UpdateUserAsync(id, dto);
        return StatusCode(response.Success ? 200 : 400, response);
    }

    [Authorize]
    [HttpDelete("users/{id}")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteUser(string id)
    {
        var response = await _userAccountGatewayService.DeleteUserAsync(id);
        return StatusCode(response.Success ? 200 : 400, response);
    }

    [Authorize]
    [HttpPut("users/{id}/status")]
    public async Task<ActionResult<ApiResponse<bool>>> UpdateUserStatus(string id, [FromBody] UpdateUserStatusDto dto)
    {
        var response = await _userAccountGatewayService.UpdateUserStatusAsync(id, dto.Status);
        return StatusCode(response.Success ? 200 : 400, response);
    }
}

public class UpdateUserStatusDto
{
    public string Status { get; set; } = string.Empty;
}

