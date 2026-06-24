using Opus.Mobile.Data.Context;
using Opus.Mobile.Data.Models;
using Opus.Mobile.Shared.Todos;

namespace Opus.Mobile.API.Services.Todos;

public class TodoService(OpusDBContext ctx) : ITodoService
{
    public async Task<IEnumerable<TodoItem>> GetTodos(int userId, TodoSearchRequest request)
    {
        var roleId = request.ResponsibleRoleId;
        var taskTypeIds = request.TaskTypeId is > 0
            ? request.TaskTypeId.Value.ToString()
            : "";

        var userFilter = request.AssignedToMe
            ? userId.ToString()
            : "";

        roleId ??= request.AssignedToMe ? -2 : -1;

        var todos = await ctx.Procedures.SelectScheduledUnscheduledTasksAsync(
            roleId,
            "",
            false,
            null,
            null,
            userId,
            -1,
            -1,
            -1,
            userFilter,
            taskTypeIds,
            -1,
            false,
            0,
            0,
            "",
            "");

        return todos
            .Where(todo =>
                !request.AssignedToMe ||
                (todo.AssignedToUserID.HasValue && todo.AssignedToUserID == userId))
            .Select(MapTodo);
    }

    private static TodoItem MapTodo(SelectScheduledUnscheduledTasksResult todo)
    {
        return new TodoItem
        {
            TodoId = todo.ID,
            TaskLogId = todo.ID,
            TaskId = todo.TASKID,
            TaskIntervalId = todo.TaskIntervalsID,
            TaskClassId = todo.TaskClassID,

            Title = todo.TaskName,
            Desc = todo.TaskDescription,

            LeftTime = todo.LeftTime,
            LeftTimeChar = todo.LeftTimeChar,
            IntervalChar = todo.IntervalChar,
            CompletedTime = todo.CompletedTime,

            ComponentId = todo.ComponentID,
            ComponentTree = todo.ComponentTree,
            ComponentHours = todo.CompHours,

            TaskTypeId = todo.TaskTypeID,
            TaskTypeName = todo.TaskTypeName,

            UserId = todo.UserID,
            UserShortName = todo.UserShName,

            Unscheduled = todo.Unscheduled,
            ResponsibleRoleName = todo.ResponsibleRole
        };
    }
}
