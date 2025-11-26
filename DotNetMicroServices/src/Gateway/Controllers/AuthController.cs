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
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        IUserAccountGatewayService userAccountGatewayService,
        ILogger<AuthController> logger)
    {
        _userAccountGatewayService = userAccountGatewayService;
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
        var response = await _userAccountGatewayService.SignInAsync(dto);
        return StatusCode(response.Success ? 200 : 401, response);
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
}

