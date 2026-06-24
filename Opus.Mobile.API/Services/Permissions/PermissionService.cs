using Microsoft.EntityFrameworkCore;
using Opus.Mobile.Data.Context;
using Opus.Mobile.Shared.Permissions;

namespace Opus.Mobile.API.Services.Permissions;

public class PermissionService(OpusDBContext ctx) : IPermissionService
{
    public async Task<PermissionCheckItem> CheckForPermission(int userId, string action)
    {
        var permission = await (
            from menu in ctx.MenuTable.AsNoTracking()
            join employeeMenu in ctx.EmployeesMenu.AsNoTracking()
                on menu.Id equals employeeMenu.MenuTableId
            where menu.Description == action &&
                  employeeMenu.EmployeeId == userId
            select employeeMenu.Visible
        ).FirstOrDefaultAsync();

        return new PermissionCheckItem
        {
            Action = action,
            IsAllowed = permission ?? false
        };
    }
}
