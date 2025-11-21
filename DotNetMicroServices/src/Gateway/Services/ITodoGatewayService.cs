using Shared.Models;
using Shared.DTOs;
using Shared.Common;

namespace Gateway.Services;

public interface ITodoGatewayService
{
    Task<ApiResponse<List<Todo>>> GetAllTodosAsync(Guid? userId = null);
    Task<ApiResponse<Todo>> GetTodoByIdAsync(Guid id);
    Task<ApiResponse<Todo>> CreateTodoAsync(CreateTodoDto dto);
    Task<ApiResponse<Todo>> UpdateTodoAsync(Guid id, UpdateTodoDto dto);
    Task<ApiResponse<bool>> DeleteTodoAsync(Guid id);
}

