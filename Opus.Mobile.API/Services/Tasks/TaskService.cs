using Microsoft.EntityFrameworkCore;
using Opus.Mobile.Data.Context;
using Opus.Mobile.Shared.Documents;
using Opus.Mobile.Shared.Tasks;
using System.Xml.XPath;

namespace Opus.Mobile.API.Services.Tasks;

public class TaskService(OpusDBContext ctx) : ITaskService
{
    public async Task<IEnumerable<DocumentItem>> GetTaskDocuments(int taskId)
    {
        return await ctx.TaskDocuments
            .AsNoTracking()
            .Where(taskDocument =>
                taskDocument.TaskId == taskId &&
                taskDocument.Document.Extension != null &&
                DocumentExtensions.Contains(taskDocument.Document.Extension.ToLower()))
            .Select(taskDocument => new DocumentItem
            {
                Id = taskDocument.Document.Id,
                Description = taskDocument.Document.Description,
                Extension = taskDocument.Document.Extension
            })
            .ToListAsync();
    }

    public async Task<CreatedTaskItem> CreateTask(int userId, CreateTaskRequest request)
    {
        var nr = 1;

        if (await ctx.UnscheduledTasks.AnyAsync())
        {
            var lastNr = await ctx.UnscheduledTasks
                .Where(task => task.Nr != null)
                .OrderBy(task => task.Nr)
                .Select(task => task.Nr)
                .LastOrDefaultAsync();

            if (lastNr.HasValue)
                nr = lastNr.Value + 1;
        }

        var result = await ctx.Procedures.SpXama_CreateUnscheduledTaskAsync(
            nr,
            request.ComponentId,
            request.DeadlineDate,
            request.Title,
            request.Desc,
            request.ResponsibleRoleId is > 0 ? request.ResponsibleRoleId : null,
            "",
            request.Priority,
            request.OnHold,
            request.Warranty,
            request.TaskTypeId is > 0 ? request.TaskTypeId : null,
            "",
            userId,
            request.TaskId);

        var newTaskIdText = result.FirstOrDefault()?.Result;

        if (!int.TryParse(newTaskIdText, out var newTaskId))
        {
            return new CreatedTaskItem
            {
                TaskId = null
            };
        }

        var taskExists = await ctx.UnscheduledTasks
            .AsNoTracking()
            .AnyAsync(task => task.Id == newTaskId);

        return new CreatedTaskItem
        {
            TaskId = taskExists ? newTaskId : null
        };
    }

    private static readonly string[] DocumentExtensions =
    [
        ".pdf",
        ".xlsx",
        ".xls",
        ".doc",
        ".docx"
    ];
}
