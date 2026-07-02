using Opus.Mobile.Shared.Models.Services;
using Opus.Mobile.Shared.Todos;

namespace Opus.Mobile.Services.Modules.Todos;

public interface ITodosService
{
    Task<ServiceResponse<IEnumerable<TodoItem>>> GetTodos(TodoSearchRequest request, CancellationToken cancellationToken = default);
}
