using Opus.Mobile.Services.Requests.RequestProcessors;
using Opus.Mobile.Shared.Models.Services;
using Opus.Mobile.Shared.Todos;

namespace Opus.Mobile.Services.Modules.Todos;

public class TodosService(IRequestProcessor requestProcessor) : ITodosService
{
    public Task<ServiceResponse<IEnumerable<TodoItem>>> GetTodos(
        TodoSearchRequest request,
        CancellationToken cancellationToken = default)
    {
        return requestProcessor.Post<IEnumerable<TodoItem>>(TodosAPIEndpoints.Todos, request, cancellationToken);
    }
}
