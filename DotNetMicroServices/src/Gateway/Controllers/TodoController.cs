using Microsoft.AspNetCore.Mvc;
using Gateway.Services;
using Shared.DTOs;
using Shared.Common;

namespace Gateway.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TodoController : ControllerBase
{
    private readonly ITodoGatewayService _todoGatewayService;
    private readonly ILogger<TodoController> _logger;

    public TodoController(ITodoGatewayService todoGatewayService, ILogger<TodoController> logger)
    {
        _todoGatewayService = todoGatewayService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<Shared.Models.Todo>>>> GetAllTodos([FromQuery] Guid? userId)
    {
        var response = await _todoGatewayService.GetAllTodosAsync(userId);
        return StatusCode(response.Success ? 200 : 500, response);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<Shared.Models.Todo>>> GetTodoById(Guid id)
    {
        var response = await _todoGatewayService.GetTodoByIdAsync(id);
        if (!response.Success && response.Message.Contains("not found"))
            return NotFound(response);
        return StatusCode(response.Success ? 200 : 500, response);
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<Shared.Models.Todo>>> CreateTodo([FromBody] CreateTodoDto dto)
    {
        var response = await _todoGatewayService.CreateTodoAsync(dto);
        return StatusCode(response.Success ? 201 : 500, response);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<Shared.Models.Todo>>> UpdateTodo(Guid id, [FromBody] UpdateTodoDto dto)
    {
        var response = await _todoGatewayService.UpdateTodoAsync(id, dto);
        if (!response.Success && response.Message.Contains("not found"))
            return NotFound(response);
        return StatusCode(response.Success ? 200 : 500, response);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteTodo(Guid id)
    {
        var response = await _todoGatewayService.DeleteTodoAsync(id);
        if (!response.Success && response.Message.Contains("not found"))
            return NotFound(response);
        return StatusCode(response.Success ? 200 : 500, response);
    }
}

