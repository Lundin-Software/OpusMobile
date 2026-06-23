using Microsoft.EntityFrameworkCore;
using Opus.Mobile.Data.Context;
using Opus.Mobile.Shared.Tasks;

namespace Opus.Mobile.API.Services.TaskLogs;

public class TaskLogService(OpusDBContext ctx) : ITaskLogService
{
    public async Task<TaskLogOpenItem> GetOrCreateOpenTaskLog(
        int userId,
        OpenTaskLogRequest request)
    {
        if (request.TaskId is null || request.TaskIntervalId is null)
            throw new ArgumentException("TaskId and TaskIntervalId are required.");

        var taskLog = await ctx.TaskLogs
            .FirstOrDefaultAsync(log =>
                log.TaskId == request.TaskId.Value &&
                log.TaskIntervalsId == request.TaskIntervalId.Value &&
                !log.CompletedTime.HasValue);

        if (taskLog is null)
        {
            taskLog = new Data.Models.TaskLogs
            {
                TaskId = request.TaskId.Value,
                TaskIntervalsId = request.TaskIntervalId,
                UserId = userId,
                TaskTypeId = request.TaskTypeId == -1 ? null : request.TaskTypeId,
                NextDate = request.NextDate,
                AddedDate = DateTime.Now
            };

            await ctx.TaskLogs.AddAsync(taskLog);
            await ctx.SaveChangesAsync();
        }

        var task = await ctx.Tasks
            .Include(task => task.Component)
            .FirstOrDefaultAsync(task => task.Id == taskLog.TaskId);

        var result = new TaskLogOpenItem
        {
            TaskLogId = taskLog.Id,
            TaskId = taskLog.TaskId,
            TaskIntervalId = taskLog.TaskIntervalsId
        };

        if (task is not null)
        {
            result.Title = task.Name;
            result.Desc = task.Description;
            result.TaskClassId = task.TaskClassId;

            if (task.ComponentId is not null && task.Component is not null)
            {
                result.ComponentId = task.ComponentId;
                result.ComponentTree = task.Component.Name1;
            }

            result.TaskLogs = await GetTaskLogs(task.Id);
        }

        return result;
    }

    private async Task<List<TaskLogHistoryItem>> GetTaskLogs(int taskId)
    {
        return await ctx.TaskLogs
            .AsNoTracking()
            .Where(log => log.TaskId == taskId)
            .OrderByDescending(log => log.CompletedTime)
            .Select(log => new TaskLogHistoryItem
            {
                Id = log.Id,
                TaskId = log.TaskId,
                AccomplishedBy = log.User == null ? string.Empty : log.User.ShortName,
                AccomplishedDate = log.CompletedTime,
                RunningHours = log.RunningHours,
                Remark = log.Remark,
                TaskFields = log.TaskFieldData
                    .Select(fieldData => new TaskFieldValueItem
                    {
                        TaskFieldId = fieldData.TaskFieldId ?? 0,
                        FieldPrompt = fieldData.TaskField.FieldPrompt,
                        FieldUnit = fieldData.TaskField.FieldUnit,
                        FieldData = fieldData.FieldData
                    })
                    .ToList()
            })
            .ToListAsync();
    }
}
