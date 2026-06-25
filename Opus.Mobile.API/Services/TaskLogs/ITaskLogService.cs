using Opus.Mobile.Shared.TaskLogs;

namespace Opus.Mobile.API.Services.TaskLogs;

public interface ITaskLogService
{
    Task<TaskLogOpenItem> GetOrCreateOpenTaskLog(int userId, OpenTaskLogRequest request);

    Task<AccomplishedTaskLogItem> AccomplishTaskLog(int userId, int taskLogId, AccomplishTaskLogRequest request);

    Task<bool> SaveAccomplishData(int userId, int taskLogId, SaveAccomplishDataRequest request);

    Task<IEnumerable<TaskFieldItem>> GetTaskFields(int taskLogId, int? taskClassId);

    Task<IEnumerable<TaskLogHistoryItem>> GetTaskLogs(int taskId);
}
