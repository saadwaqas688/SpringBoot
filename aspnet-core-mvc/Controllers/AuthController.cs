using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CourseManagementAPI.DTOs;
using CourseManagementAPI.Services;
using CourseManagementAPI.Repositories;

namespace CourseManagementAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IUserRepository _userRepository;
    private readonly IJwtService _jwtService;

    public AuthController(IAuthService authService, IUserRepository userRepository, IJwtService jwtService)
    {
        _authService = authService;
        _userRepository = userRepository;
        _jwtService = jwtService;
    }

    [HttpPost("signup")]
    public async Task<ActionResult<AuthResponse>> Signup([FromBody] SignupRequest request)
    {
        try
        {
            var user = await _authService.SignupAsync(
                request.Email,
                request.Password,
                request.FirstName,
                request.LastName,
                "USER"
            );

            var token = _jwtService.GenerateToken(user.Id!, user.Email, user.Role);

            return Ok(new AuthResponse
            {
                Token = token,
                UserId = user.Id!,
                Email = user.Email,
                Role = user.Role
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("signin")]
    public async Task<ActionResult<AuthResponse>> Signin([FromBody] SigninRequest request)
    {
        try
        {
            var token = await _authService.SigninAsync(request.Email, request.Password);
            var user = await _userRepository.GetByEmailAsync(request.Email);

            if (user == null)
            {
                return Unauthorized();
            }

            return Ok(new AuthResponse
            {
                Token = token,
                UserId = user.Id!,
                Email = user.Email,
                Role = user.Role
            });
        }
        catch (Exception ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
    }

    [HttpPost("signup-admin")]
    [Authorize(Roles = "ADMIN")]
    public async Task<ActionResult<AuthResponse>> SignupAdmin([FromBody] SignupRequest request)
    {
        try
        {
            var user = await _authService.SignupAsync(
                request.Email,
                request.Password,
                request.FirstName,
                request.LastName,
                "ADMIN"
            );

            var token = _jwtService.GenerateToken(user.Id!, user.Email, user.Role);

            return Ok(new AuthResponse
            {
                Token = token,
                UserId = user.Id!,
                Email = user.Email,
                Role = user.Role
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}



