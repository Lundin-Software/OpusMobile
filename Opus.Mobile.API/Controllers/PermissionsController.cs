using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Opus.Mobile.API.Services.Permissions;
using Opus.Mobile.Shared.API;
using Opus.Mobile.Shared.Permissions;
using System.Security.Claims;

namespace Opus.Mobile.API.Controllers;

[ApiController]
[Route("[controller]")]
public class PermissionsController(IPermissionService permissionService) : ControllerBase
{
    [Authorize]
    [HttpGet("{action}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(APIResponse<PermissionCheckItem>))]
    public async Task<ActionResult> CheckForPermission([FromRoute] string action)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userIdClaim is null || !int.TryParse(userIdClaim, out var userId))
            return BadRequest("Could not get userId from JWT Token");

        var permission = await permissionService.CheckForPermission(userId, action);

        return Ok(new APIResponse<PermissionCheckItem>(permission));
    }
}
