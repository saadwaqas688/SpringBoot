using Shared.Models;
using Shared.DTOs;
using Shared.Common;
using Shared.Constants;
using Shared.Services;

namespace Gateway.Services;

public class TodoGatewayService : ITodoGatewayService
{
    private readonly IRabbitMQService _rabbitMQService;
    private readonly ILogger<TodoGatewayService> _logger;

    public TodoGatewayService(
        IRabbitMQService rabbitMQService,
        ILogger<TodoGatewayService> logger)
    {
        _rabbitMQService = rabbitMQService;
        _logger = logger;
    }

    public async Task<ApiResponse<List<Todo>>> GetAllTodosAsync(Guid? userId = null)
    {
        try
        {
            var message = new { UserId = userId };
            var response = await _rabbitMQService.SendMessageAsync<ApiResponse<List<Todo>>>(
                RabbitMQConstants.TodoServiceQueue,
                RabbitMQConstants.Todo.GetAll,
                message);
            return response ?? ApiResponse<List<Todo>>.ErrorResponse("Failed to retrieve todos");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling TodoService");
            return ApiResponse<List<Todo>>.ErrorResponse("An error occurred while retrieving todos");
        }
    }

    public async Task<ApiResponse<Todo>> GetTodoByIdAsync(Guid id)
    {
        try
        {
            var message = new { Id = id };
            var response = await _rabbitMQService.SendMessageAsync<ApiResponse<Todo>>(
                RabbitMQConstants.TodoServiceQueue,
                RabbitMQConstants.Todo.GetById,
                message);
            return response ?? ApiResponse<Todo>.ErrorResponse("Failed to retrieve todo");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling TodoService for todo {TodoId}", id);
            return ApiResponse<Todo>.ErrorResponse("An error occurred while retrieving todo");
        }
    }

    public async Task<ApiResponse<Todo>> CreateTodoAsync(CreateTodoDto dto)
    {
        try
        {
            var response = await _rabbitMQService.SendMessageAsync<ApiResponse<Todo>>(
                RabbitMQConstants.TodoServiceQueue,
                RabbitMQConstants.Todo.Create,
                dto);
            return response ?? ApiResponse<Todo>.ErrorResponse("Failed to create todo");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling TodoService to create todo");
            return ApiResponse<Todo>.ErrorResponse("An error occurred while creating todo");
        }
    }

    public async Task<ApiResponse<Todo>> UpdateTodoAsync(Guid id, UpdateTodoDto dto)
    {
        try
        {
            var message = new { Id = id, Dto = dto };
            var response = await _rabbitMQService.SendMessageAsync<ApiResponse<Todo>>(
                RabbitMQConstants.TodoServiceQueue,
                RabbitMQConstants.Todo.Update,
                message);
            return response ?? ApiResponse<Todo>.ErrorResponse("Failed to update todo");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling TodoService to update todo {TodoId}", id);
            return ApiResponse<Todo>.ErrorResponse("An error occurred while updating todo");
        }
    }

    public async Task<ApiResponse<bool>> DeleteTodoAsync(Guid id)
    {
        try
        {
            var message = new { Id = id };
            var response = await _rabbitMQService.SendMessageAsync<ApiResponse<bool>>(
                RabbitMQConstants.TodoServiceQueue,
                RabbitMQConstants.Todo.Delete,
                message);
            return response ?? ApiResponse<bool>.ErrorResponse("Failed to delete todo");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling TodoService to delete todo {TodoId}", id);
            return ApiResponse<bool>.ErrorResponse("An error occurred while deleting todo");
        }
    }
}

