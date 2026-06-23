using Opus.Mobile.Shared.Lookup;

namespace Opus.Mobile.API.Services.Lookup;

public interface ILookupService
{
    Task<IEnumerable<RoleLookupItem>> GetRoles(string? searchText, int? userId);

    Task<IEnumerable<TaskTypeLookupItem>> GetTaskTypes(string? searchText);

    Task<IEnumerable<NotificationTypeLookupItem>> GetNotificationTypes(string? searchText);

    Task<IEnumerable<UnscheduledTaskLookupItem>> GetUnscheduledTasks(string? searchText);

    Task<IEnumerable<TaskFieldComboLookupItem>> GetTaskFieldCombos(int taskFieldId);

    Task<IEnumerable<ComponentLookupItem>> GetComponents(int? userId, int? componentId, int? parentComponentId);
}
