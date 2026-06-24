using Opus.Mobile.Shared.Documents;
using Opus.Mobile.Shared.Tasks;

namespace Opus.Mobile.API.Services.Tasks;

public interface ITaskService
{
    Task<IEnumerable<DocumentItem>> GetTaskDocuments(int taskId);

    Task<CreatedTaskItem> CreateTask(int userId, CreateTaskRequest request);
}
