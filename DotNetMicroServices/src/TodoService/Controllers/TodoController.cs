using Microsoft.AspNetCore.Mvc;
using TodoService.DTOs;
using TodoService.Models;
using TodoService.Services;
using Shared.Common;

namespace TodoService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TodoController : ControllerBase
{
    private readonly ITodoService _todoService;
    private readonly ILogger<TodoController> _logger;

    public TodoController(ITodoService todoService, ILogger<TodoController> logger)
    {
        _todoService = todoService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<Todo>>>> GetAllTodos([FromQuery] Guid? userId)
    {
        try
        {
            List<Todo> todos;
            if (userId.HasValue)
            {
                todos = await _todoService.GetTodosByUserIdAsync(userId.Value);
            }
            else
            {
                todos = await _todoService.GetAllTodosAsync();
            }

            return Ok(ApiResponse<List<Todo>>.SuccessResponse(todos, "Todos retrieved successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving todos");
            return StatusCode(500, ApiResponse<List<Todo>>.ErrorResponse("An error occurred while retrieving todos"));
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<Todo>>> GetTodoById(Guid id)
    {
        try
        {
            var todo = await _todoService.GetTodoByIdAsync(id);
            if (todo == null)
            {
                return NotFound(ApiResponse<Todo>.ErrorResponse("Todo not found"));
            }

            return Ok(ApiResponse<Todo>.SuccessResponse(todo, "Todo retrieved successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving todo {TodoId}", id);
            return StatusCode(500, ApiResponse<Todo>.ErrorResponse("An error occurred while retrieving todo"));
        }
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<Todo>>> CreateTodo([FromBody] CreateTodoDto dto)
    {
        try
        {
            var todo = new Todo
            {
                Title = dto.Title,
                Description = dto.Description,
                UserId = dto.UserId,
                IsCompleted = false
            };

            var createdTodo = await _todoService.CreateTodoAsync(todo);
            return CreatedAtAction(nameof(GetTodoById), new { id = createdTodo.Id },
                ApiResponse<Todo>.SuccessResponse(createdTodo, "Todo created successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating todo");
            return StatusCode(500, ApiResponse<Todo>.ErrorResponse("An error occurred while creating todo"));
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<Todo>>> UpdateTodo(Guid id, [FromBody] UpdateTodoDto dto)
    {
        try
        {
            var existingTodo = await _todoService.GetTodoByIdAsync(id);
            if (existingTodo == null)
            {
                return NotFound(ApiResponse<Todo>.ErrorResponse("Todo not found"));
            }

            if (dto.Title != null) existingTodo.Title = dto.Title;
            if (dto.Description != null) existingTodo.Description = dto.Description;
            if (dto.IsCompleted.HasValue) existingTodo.IsCompleted = dto.IsCompleted.Value;

            var updatedTodo = await _todoService.UpdateTodoAsync(id, existingTodo);
            if (updatedTodo == null)
            {
                return NotFound(ApiResponse<Todo>.ErrorResponse("Todo not found"));
            }

            return Ok(ApiResponse<Todo>.SuccessResponse(updatedTodo, "Todo updated successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating todo {TodoId}", id);
            return StatusCode(500, ApiResponse<Todo>.ErrorResponse("An error occurred while updating todo"));
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteTodo(Guid id)
    {
        try
        {
            var deleted = await _todoService.DeleteTodoAsync(id);
            if (!deleted)
            {
                return NotFound(ApiResponse<bool>.ErrorResponse("Todo not found"));
            }

            return Ok(ApiResponse<bool>.SuccessResponse(true, "Todo deleted successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting todo {TodoId}", id);
            return StatusCode(500, ApiResponse<bool>.ErrorResponse("An error occurred while deleting todo"));
        }
    }
}

