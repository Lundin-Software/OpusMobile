using Opus.Mobile.Shared.TaskLogs;

namespace Opus.Mobile.API.Services.TaskLogs;

public interface ITaskLogService
{
    Task<TaskLogOpenItem> GetOrCreateOpenTaskLog(int userId, OpenTaskLogRequest request);

    Task<AccomplishedTaskLogItem> AccomplishTaskLog(int userId, int taskLogId, AccomplishTaskLogRequest request);
}
