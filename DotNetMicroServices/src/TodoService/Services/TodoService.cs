using TodoService.Models;
using Shared.Utils;

namespace TodoService.Services;

public class TodoService : ITodoService
{
    private static readonly List<Todo> _todos = new();
    private static readonly object _lock = new();

    public Task<List<Todo>> GetAllTodosAsync()
    {
        lock (_lock)
        {
            return Task.FromResult(new List<Todo>(_todos));
        }
    }

    public Task<List<Todo>> GetTodosByUserIdAsync(Guid userId)
    {
        lock (_lock)
        {
            var todos = _todos.Where(t => t.UserId == userId).ToList();
            return Task.FromResult(todos);
        }
    }

    public Task<Todo?> GetTodoByIdAsync(Guid id)
    {
        lock (_lock)
        {
            var todo = _todos.FirstOrDefault(t => t.Id == id);
            return Task.FromResult(todo);
        }
    }

    public Task<Todo> CreateTodoAsync(Todo todo)
    {
        lock (_lock)
        {
            todo.Id = Guid.NewGuid();
            todo.CreatedAt = DateTimeHelper.GetUtcNow();
            todo.UpdatedAt = DateTimeHelper.GetUtcNow();
            _todos.Add(todo);
            return Task.FromResult(todo);
        }
    }

    public Task<Todo?> UpdateTodoAsync(Guid id, Todo updatedTodo)
    {
        lock (_lock)
        {
            var todo = _todos.FirstOrDefault(t => t.Id == id);
            if (todo == null)
                return Task.FromResult<Todo?>(null);

            todo.Title = updatedTodo.Title;
            todo.Description = updatedTodo.Description;
            todo.IsCompleted = updatedTodo.IsCompleted;
            todo.UpdatedAt = DateTimeHelper.GetUtcNow();

            return Task.FromResult<Todo?>(todo);
        }
    }

    public Task<bool> DeleteTodoAsync(Guid id)
    {
        lock (_lock)
        {
            var todo = _todos.FirstOrDefault(t => t.Id == id);
            if (todo == null)
                return Task.FromResult(false);

            _todos.Remove(todo);
            return Task.FromResult(true);
        }
    }
}

