using Opus.Mobile.Shared.Permissions;

namespace Opus.Mobile.API.Services.Permissions;

public interface IPermissionService
{
    Task<PermissionCheckItem> CheckForPermission(int userId, string action);
}
