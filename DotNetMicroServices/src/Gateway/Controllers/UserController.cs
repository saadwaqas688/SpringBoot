using Microsoft.AspNetCore.Mvc;
using Gateway.Services;
using Shared.DTOs;
using Shared.Common;
using System.Text.Json;

namespace Gateway.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserGatewayService _userGatewayService;
    private readonly ILogger<UserController> _logger;

    public UserController(IUserGatewayService userGatewayService, ILogger<UserController> logger)
    {
        _userGatewayService = userGatewayService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<Shared.Models.User>>>> GetAllUsers()
    {
                Console.WriteLine("get called with payload update  abc");

        var response = await _userGatewayService.GetAllUsersAsync();
        return StatusCode(response.Success ? 200 : 500, response);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<Shared.Models.User>>> GetUserById(Guid id)
    {
        var response = await _userGatewayService.GetUserByIdAsync(id);
        if (!response.Success && response.Message.Contains("not found"))
            return NotFound(response);
        return StatusCode(response.Success ? 200 : 500, response);
    }

    [HttpGet("email/{email}")]
    public async Task<ActionResult<ApiResponse<Shared.Models.User>>> GetUserByEmail(string email)
    {
        var response = await _userGatewayService.GetUserByEmailAsync(email);
        if (!response.Success && response.Message.Contains("not found"))
            return NotFound(response);
        return StatusCode(response.Success ? 200 : 500, response);
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<Shared.Models.User>>> CreateUser([FromBody] CreateUserDto dto)
    {
        Console.WriteLine($"CreateUser called with payload update  abc: {JsonSerializer.Serialize(dto)}");

        var response = await _userGatewayService.CreateUserAsync(dto);
        if (!response.Success && response.Message.Contains("already exists"))
            return BadRequest(response);
        return StatusCode(response.Success ? 201 : 500, response);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<Shared.Models.User>>> UpdateUser(Guid id, [FromBody] UpdateUserDto dto)
    {
        var response = await _userGatewayService.UpdateUserAsync(id, dto);
        if (!response.Success && response.Message.Contains("not found"))
            return NotFound(response);
        return StatusCode(response.Success ? 200 : 500, response);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteUser(Guid id)
    {
        var response = await _userGatewayService.DeleteUserAsync(id);
        if (!response.Success && response.Message.Contains("not found"))
            return NotFound(response);
        return StatusCode(response.Success ? 200 : 500, response);
    }
}

