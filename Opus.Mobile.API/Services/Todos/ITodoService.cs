using Opus.Mobile.Shared.Todos;

namespace Opus.Mobile.API.Services.Todos;

public interface ITodoService
{
    Task<IEnumerable<TodoItem>> GetTodos(int userId, TodoSearchRequest request);
}
