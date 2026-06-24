using Microsoft.EntityFrameworkCore;
using Opus.Mobile.Data.Context;
using Opus.Mobile.Shared.TaskLogs;

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

    public async Task<AccomplishedTaskLogItem> AccomplishTaskLog(
    int userId,
    int taskLogId,
    AccomplishTaskLogRequest request)
    {
        var fromComp = request.FromComp ?? 0;
        var unsched = request.Unsched ?? 0;
        var automatic = request.Automatic ?? 0;
        var purchaseId = request.PurchaseId ?? 0;

        var hours = request.Hours ?? 0;

        if (unsched == 0)
        {
            var counterNr = await ctx.Components
                .AsNoTracking()
                .Where(component => component.Id == request.ComponentId)
                .Select(component => component.CounterId ?? 0)
                .FirstOrDefaultAsync();

            var company = await ctx.Company.FirstOrDefaultAsync();

            if (company is not null && (company.CountersEditable ?? false) && counterNr > 0)
            {
                if (request.ComponentHours is null or 0)
                {
                    request.ComponentHours = ctx.FComponentHours(request.ComponentId)
                        .Max(component => component.CompHours);
                }

                hours = request.ComponentHours ?? 0;
            }
            else
            {
                var taskInterval = await ctx.TaskIntervals
                    .FirstOrDefaultAsync(interval => interval.Id == request.TaskIntervalId);

                if (taskInterval is not null &&
                    taskInterval.IntervalTypeId == 0 &&
                    unsched == 0 &&
                    request.Hours is null)
                {
                    hours = ctx.FComponentHours(request.ComponentId)
                        .Max(component => component.CompHours) ?? 0;
                }
            }
        }

        await ctx.Procedures.SpXama_CreateCosumingArticlesAndPurchaseAtAccomplishTaskAsync(
            taskLogId,
            userId);

        var updatedTaskLog = await ctx.Procedures.UpdateTaskLogsAsync(
            fromComp,
            unsched,
            taskLogId,
            userId,
            hours,
            request.Remark,
            request.TaskId,
            request.TaskIntervalId,
            request.TaskTypeId,
            DateTime.Now,
            request.ManHours,
            automatic,
            request.NextDate,
            purchaseId);

        var updatedTaskLogId = updatedTaskLog.FirstOrDefault()?.TaskLogID;

        if (updatedTaskLogId.HasValue)
            await ctx.Procedures.UPDATETaskLogsSetFieldsAsync(updatedTaskLogId);

        return new AccomplishedTaskLogItem
        {
            TaskLogId = updatedTaskLogId
        };
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
