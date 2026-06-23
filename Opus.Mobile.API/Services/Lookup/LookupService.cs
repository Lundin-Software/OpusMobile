using Microsoft.EntityFrameworkCore;
using Opus.Mobile.Data.Context;
using Opus.Mobile.Shared.Lookup;

namespace Opus.Mobile.API.Services.Lookup;

public class LookupService(OpusDBContext ctx) : ILookupService
{
    public async Task<IEnumerable<RoleLookupItem>> GetRoles(string? searchText, int? userId)
    {
        List<RoleLookupItem> roles =
        [
            new RoleLookupItem
            {
                RoleId = -1,
                ShortName = "-ALL-",
                Name = "-ALL-"
            }
        ];

        var employee = await ctx.Employees
            .AsNoTracking()
            .FirstOrDefaultAsync(user => user.Id == userId);

        if (userId != null && (employee?.SuperUser ?? false) == false)
        {
            var userRoles = await (
                from employeeRole in ctx.EmployeesRoles.AsNoTracking()
                join role in ctx.Roles.AsNoTracking() on employeeRole.RoleId equals role.Id
                where employeeRole.EmployeeId == userId
                orderby role.ShortName
                select new RoleLookupItem
                {
                    RoleId = role.Id,
                    ShortName = role.ShortName,
                    Name = role.Name
                }).ToListAsync();

            roles.AddRange(userRoles);
        }
        else
        {
            var filter = Normalize(searchText);

            var allRoles = await ctx.Roles.AsNoTracking()
                .Where(role =>
                    (role.ShortName != null && role.ShortName.ToUpper().Contains(filter)) ||
                    (role.Name != null && role.Name.ToUpper().Contains(filter)))
                .OrderBy(role => role.ShortName)
                .Select(role => new RoleLookupItem
                {
                    RoleId = role.Id,
                    ShortName = role.ShortName,
                    Name = role.Name
                })
                .ToListAsync();

            roles.AddRange(allRoles);
        }

        return roles;
    }

    public async Task<IEnumerable<TaskTypeLookupItem>> GetTaskTypes(string? searchText)
    {
        List<TaskTypeLookupItem> taskTypes =
        [
            new TaskTypeLookupItem
            {
                TaskTypeId = -1,
                ShortName = "-ALL-",
                Name = "-ALL-"
            }
        ];

        var filter = Normalize(searchText);

        var dbTaskTypes = await ctx.TaskTypes
            .AsNoTracking()
            .Where(taskType =>
                (taskType.ShortName != null && taskType.ShortName.ToUpper().Contains(filter)) ||
                (taskType.Name != null && taskType.Name.ToUpper().Contains(filter)))
            .OrderBy(taskType => taskType.ShortName)
            .Select(taskType => new TaskTypeLookupItem
            {
                TaskTypeId = taskType.Id,
                ShortName = taskType.ShortName,
                Name = taskType.Name
            })
            .ToListAsync();

        taskTypes.AddRange(dbTaskTypes);

        return taskTypes;
    }

    public async Task<IEnumerable<NotificationTypeLookupItem>> GetNotificationTypes(string? searchText)
    {
        List<NotificationTypeLookupItem> notificationTypes =
        [
            new NotificationTypeLookupItem
            {
                NotificationTypeId = -1,
                ShortName = "-ALL-",
                Name = "-ALL-"
            }
        ];

        var filter = Normalize(searchText);

        var dbNotificationTypes = await ctx.NotificationTypes
            .AsNoTracking()
            .Where(notificationType =>
                (notificationType.ShortName != null && notificationType.ShortName.ToUpper().Contains(filter)) ||
                (notificationType.Name != null && notificationType.Name.ToUpper().Contains(filter)))
            .OrderBy(notificationType => notificationType.ShortName)
            .Select(notificationType => new NotificationTypeLookupItem
            {
                NotificationTypeId = notificationType.Id,
                ShortName = notificationType.ShortName,
                Name = notificationType.Name
            })
            .ToListAsync();

        notificationTypes.AddRange(dbNotificationTypes);

        return notificationTypes;
    }

    public async Task<IEnumerable<UnscheduledTaskLookupItem>> GetUnscheduledTasks(string? searchText)
    {
        List<UnscheduledTaskLookupItem> unscheduledTasks =
        [
            new UnscheduledTaskLookupItem
            {
                TaskId = -1,
                Title = "-ALL-",
                Desc = "-ALL-",
                IssueDate = DateTime.Now
            }
        ];

        var filter = Normalize(searchText);

        var dbUnscheduledTasks = await ctx.UnscheduledTasks
            .AsNoTracking()
            .Where(task =>
                task.Date >= DateTime.Now.AddYears(-2) &&
                task.Completed != true &&
                (
                    (task.Title != null && task.Title.ToUpper().Contains(filter)) ||
                    (task.Description != null && task.Description.ToUpper().Contains(filter))
                ))
            .OrderBy(task => task.Title)
            .Select(task => new UnscheduledTaskLookupItem
            {
                TaskId = task.Id,
                Title = task.Title,
                Desc = task.Description,
                IssueDate = task.Date
            })
            .ToListAsync();

        unscheduledTasks.AddRange(dbUnscheduledTasks);

        return unscheduledTasks;
    }

    public async Task<IEnumerable<TaskFieldComboLookupItem>> GetTaskFieldCombos(int taskFieldId)
    {
        List<TaskFieldComboLookupItem> combos =
        [
            new TaskFieldComboLookupItem
            {
                TaskFieldComboId = -1,
                ComboValue = "-NONE-"
            }
        ];

        var dbCombos = await ctx.TaskFieldComboBoxValues
            .AsNoTracking()
            .Where(combo => combo.TaskFieldId == taskFieldId)
            .OrderBy(combo => combo.ComboValue)
            .Select(combo => new TaskFieldComboLookupItem
            {
                TaskFieldComboId = combo.Id,
                TaskFieldId = combo.TaskFieldId,
                ComboValue = combo.ComboValue
            })
            .ToListAsync();

        combos.AddRange(dbCombos);

        return combos;
    }

    public async Task<IEnumerable<ComponentLookupItem>> GetComponents(int? userId, int? componentId, int? parentComponentId)
    {
        var lookups = await ctx.Procedures.SpXama_LookupComponentsAsync(
            userId,
            componentId,
            parentComponentId);

        var itemIds = lookups
            .Where(component => component.ID.HasValue)
            .Select(component => component.ID!.Value)
            .ToList();

        var parentIds = await ctx.Components
            .AsNoTracking()
            .Where(component =>
                component.ParentId != null &&
                itemIds.Contains(component.ParentId.Value))
            .Select(component => component.ParentId!.Value)
            .Distinct()
            .ToHashSetAsync();

        return lookups.Select(component => new ComponentLookupItem
        {
            ComponentId = component.ID,
            Name1 = component.ComponentName,
            ParentId = component.ParentId,
            IsHeader = component.IsHeader,
            IconImage = component.IconImage,
            HasChildren = component.ID.HasValue && parentIds.Contains(component.ID.Value)
        });
    }

    private static string Normalize(string? value) =>
        string.IsNullOrWhiteSpace(value)
            ? ""
            : value.Trim().ToUpperInvariant();
}
