using TodoService.Models;
using TodoService.Services;
using Shared.Common;
using Shared.Constants;
using Shared.DTOs;
using System.Text.Json;

namespace TodoService.Services;

public class TodoMessageHandler
{
    private readonly ITodoService _todoService;
    private readonly ILogger<TodoMessageHandler> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    public TodoMessageHandler(ITodoService todoService, ILogger<TodoMessageHandler> logger)
    {
        _todoService = todoService;
        _logger = logger;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    public async Task<object?> HandleMessage(string messageJson, string routingKey)
    {
        try
        {
            _logger.LogInformation("Handling message with routing key: {RoutingKey}", routingKey);

            return routingKey switch
            {
                RabbitMQConstants.Todo.HealthCheck => HandleHealthCheck(),
                RabbitMQConstants.Todo.GetAll => await HandleGetAll(messageJson),
                RabbitMQConstants.Todo.GetById => await HandleGetById(messageJson),
                RabbitMQConstants.Todo.Create => await HandleCreate(messageJson),
                RabbitMQConstants.Todo.Update => await HandleUpdate(messageJson),
                RabbitMQConstants.Todo.Delete => await HandleDelete(messageJson),
                _ => ApiResponse<object>.ErrorResponse($"Unknown routing key: {routingKey}")
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling message with routing key: {RoutingKey}", routingKey);
            return ApiResponse<object>.ErrorResponse($"Error processing message: {ex.Message}");
        }
    }

    private object HandleHealthCheck()
    {
        return ApiResponse<string>.SuccessResponse("TodoService is healthy", "Health check successful");
    }

    private async Task<object> HandleGetAll(string messageJson)
    {
        try
        {
            var request = JsonSerializer.Deserialize<GetAllTodosRequest>(messageJson, _jsonOptions);
            List<Todo> todos;

            if (request?.UserId != null)
            {
                todos = await _todoService.GetTodosByUserIdAsync(request.UserId.Value);
            }
            else
            {
                todos = await _todoService.GetAllTodosAsync();
            }

            // Convert to Shared.Models.Todo
            var sharedTodos = todos.Select(t => new Shared.Models.Todo
            {
                Id = t.Id,
                Title = t.Title,
                Description = t.Description,
                IsCompleted = t.IsCompleted,
                UserId = t.UserId,
                CreatedAt = t.CreatedAt,
                UpdatedAt = t.UpdatedAt
            }).ToList();

            return ApiResponse<List<Shared.Models.Todo>>.SuccessResponse(sharedTodos, "Todos retrieved successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in HandleGetAll");
            return ApiResponse<List<Shared.Models.Todo>>.ErrorResponse($"Error retrieving todos: {ex.Message}");
        }
    }

    private async Task<object> HandleGetById(string messageJson)
    {
        try
        {
            var request = JsonSerializer.Deserialize<GetByIdRequest>(messageJson, _jsonOptions);
            
            if (request?.Id == null || !Guid.TryParse(request.Id.ToString(), out var id))
            {
                return ApiResponse<Shared.Models.Todo>.ErrorResponse("Invalid ID provided");
            }

            var todo = await _todoService.GetTodoByIdAsync(id);
            
            if (todo == null)
            {
                return ApiResponse<Shared.Models.Todo>.ErrorResponse("Todo not found");
            }

            var sharedTodo = new Shared.Models.Todo
            {
                Id = todo.Id,
                Title = todo.Title,
                Description = todo.Description,
                IsCompleted = todo.IsCompleted,
                UserId = todo.UserId,
                CreatedAt = todo.CreatedAt,
                UpdatedAt = todo.UpdatedAt
            };

            return ApiResponse<Shared.Models.Todo>.SuccessResponse(sharedTodo, "Todo retrieved successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in HandleGetById");
            return ApiResponse<Shared.Models.Todo>.ErrorResponse($"Error retrieving todo: {ex.Message}");
        }
    }

    private async Task<object> HandleCreate(string messageJson)
    {
        try
        {
            var dto = JsonSerializer.Deserialize<CreateTodoDto>(messageJson, _jsonOptions);
            
            if (dto == null)
            {
                return ApiResponse<Shared.Models.Todo>.ErrorResponse("Invalid create todo data");
            }

            var todo = new Todo
            {
                Title = dto.Title,
                Description = dto.Description,
                UserId = dto.UserId,
                IsCompleted = false
            };

            var createdTodo = await _todoService.CreateTodoAsync(todo);

            var sharedTodo = new Shared.Models.Todo
            {
                Id = createdTodo.Id,
                Title = createdTodo.Title,
                Description = createdTodo.Description,
                IsCompleted = createdTodo.IsCompleted,
                UserId = createdTodo.UserId,
                CreatedAt = createdTodo.CreatedAt,
                UpdatedAt = createdTodo.UpdatedAt
            };

            return ApiResponse<Shared.Models.Todo>.SuccessResponse(sharedTodo, "Todo created successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in HandleCreate");
            return ApiResponse<Shared.Models.Todo>.ErrorResponse($"Error creating todo: {ex.Message}");
        }
    }

    private async Task<object> HandleUpdate(string messageJson)
    {
        try
        {
            var request = JsonSerializer.Deserialize<UpdateTodoRequest>(messageJson, _jsonOptions);
            
            if (request?.Id == null || !Guid.TryParse(request.Id.ToString(), out var id))
            {
                return ApiResponse<Shared.Models.Todo>.ErrorResponse("Invalid ID provided");
            }

            var existingTodo = await _todoService.GetTodoByIdAsync(id);
            if (existingTodo == null)
            {
                return ApiResponse<Shared.Models.Todo>.ErrorResponse("Todo not found");
            }

            // Update fields from DTO
            if (request.Dto != null)
            {
                if (request.Dto.Title != null) existingTodo.Title = request.Dto.Title;
                if (request.Dto.Description != null) existingTodo.Description = request.Dto.Description;
                if (request.Dto.IsCompleted.HasValue) existingTodo.IsCompleted = request.Dto.IsCompleted.Value;
            }

            var updatedTodo = await _todoService.UpdateTodoAsync(id, existingTodo);
            
            if (updatedTodo == null)
            {
                return ApiResponse<Shared.Models.Todo>.ErrorResponse("Failed to update todo");
            }

            var sharedTodo = new Shared.Models.Todo
            {
                Id = updatedTodo.Id,
                Title = updatedTodo.Title,
                Description = updatedTodo.Description,
                IsCompleted = updatedTodo.IsCompleted,
                UserId = updatedTodo.UserId,
                CreatedAt = updatedTodo.CreatedAt,
                UpdatedAt = updatedTodo.UpdatedAt
            };

            return ApiResponse<Shared.Models.Todo>.SuccessResponse(sharedTodo, "Todo updated successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in HandleUpdate");
            return ApiResponse<Shared.Models.Todo>.ErrorResponse($"Error updating todo: {ex.Message}");
        }
    }

    private async Task<object> HandleDelete(string messageJson)
    {
        try
        {
            var request = JsonSerializer.Deserialize<GetByIdRequest>(messageJson, _jsonOptions);
            
            if (request?.Id == null || !Guid.TryParse(request.Id.ToString(), out var id))
            {
                return ApiResponse<bool>.ErrorResponse("Invalid ID provided");
            }

            var deleted = await _todoService.DeleteTodoAsync(id);
            
            if (!deleted)
            {
                return ApiResponse<bool>.ErrorResponse("Todo not found");
            }

            return ApiResponse<bool>.SuccessResponse(true, "Todo deleted successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in HandleDelete");
            return ApiResponse<bool>.ErrorResponse($"Error deleting todo: {ex.Message}");
        }
    }

    // Helper classes for deserializing requests
    private class GetAllTodosRequest
    {
        public Guid? UserId { get; set; }
    }

    private class GetByIdRequest
    {
        public object? Id { get; set; }
    }

    private class UpdateTodoRequest
    {
        public object? Id { get; set; }
        public UpdateTodoDto? Dto { get; set; }
    }
}
