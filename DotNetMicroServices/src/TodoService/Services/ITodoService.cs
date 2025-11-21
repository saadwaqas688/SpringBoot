using TodoService.Models;

namespace TodoService.Services;

public interface ITodoService
{
    Task<List<Todo>> GetAllTodosAsync();
    Task<List<Todo>> GetTodosByUserIdAsync(Guid userId);
    Task<Todo?> GetTodoByIdAsync(Guid id);
    Task<Todo> CreateTodoAsync(Todo todo);
    Task<Todo?> UpdateTodoAsync(Guid id, Todo todo);
    Task<bool> DeleteTodoAsync(Guid id);
}

