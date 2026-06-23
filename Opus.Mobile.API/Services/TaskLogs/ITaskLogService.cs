using Opus.Mobile.Shared.Tasks;

namespace Opus.Mobile.API.Services.TaskLogs;

public interface ITaskLogService
{
    Task<TaskLogOpenItem> GetOrCreateOpenTaskLog(int userId, OpenTaskLogRequest request);
}
