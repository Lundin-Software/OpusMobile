using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Opus.Mobile.API.Services.Lookup;
using Opus.Mobile.Shared.API;
using Opus.Mobile.Shared.Lookup;
using System.Security.Claims;

namespace Opus.Mobile.API.Controllers;

[ApiController]
[Route("[controller]")]
public class LookupController(ILookupService lookupService) : ControllerBase
{
    [Authorize]
    [HttpGet("Roles")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(APIResponse<IEnumerable<RoleLookupItem>>))]
    public async Task<ActionResult> GetRoles([FromQuery] string? searchText)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userIdClaim is null || !int.TryParse(userIdClaim, out var userId))
            return BadRequest("Could not get userId from JWT Token");

        var roles = await lookupService.GetRoles(searchText, userId);

        return Ok(new APIResponse<IEnumerable<RoleLookupItem>>(roles));
    }

    [Authorize]
    [HttpGet("TaskTypes")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(APIResponse<IEnumerable<TaskTypeLookupItem>>))]
    public async Task<ActionResult> GetTaskTypes([FromQuery] string? searchText)
    {
        var taskTypes = await lookupService.GetTaskTypes(searchText);

        return Ok(new APIResponse<IEnumerable<TaskTypeLookupItem>>(taskTypes));
    }

    [Authorize]
    [HttpGet("NotificationTypes")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(APIResponse<IEnumerable<NotificationTypeLookupItem>>))]
    public async Task<ActionResult> GetNotificationTypes([FromQuery] string? searchText)
    {
        var notificationTypes = await lookupService.GetNotificationTypes(searchText);

        return Ok(new APIResponse<IEnumerable<NotificationTypeLookupItem>>(notificationTypes));
    }

    [Authorize]
    [HttpGet("UnscheduledTasks")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(APIResponse<IEnumerable<UnscheduledTaskLookupItem>>))]
    public async Task<ActionResult> GetUnscheduledTasks([FromQuery] string? searchText)
    {
        var unscheduledTasks = await lookupService.GetUnscheduledTasks(searchText);

        return Ok(new APIResponse<IEnumerable<UnscheduledTaskLookupItem>>(unscheduledTasks));
    }

    [Authorize]
    [HttpGet("TaskFieldCombos/{taskFieldId:int}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(APIResponse<IEnumerable<TaskFieldComboLookupItem>>))]
    public async Task<ActionResult> GetTaskFieldCombos([FromRoute] int taskFieldId)
    {
        var combos = await lookupService.GetTaskFieldCombos(taskFieldId);

        return Ok(new APIResponse<IEnumerable<TaskFieldComboLookupItem>>(combos));
    }

    [Authorize]
    [HttpGet("Components")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(APIResponse<IEnumerable<ComponentLookupItem>>))]
    public async Task<ActionResult> GetComponents(
        [FromQuery] int? componentId,
        [FromQuery] int? parentComponentId)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userIdClaim is null || !int.TryParse(userIdClaim, out var userId))
            return BadRequest("Could not get userId from JWT Token");

        var components = await lookupService.GetComponents(
            userId,
            componentId,
            parentComponentId);

        return Ok(new APIResponse<IEnumerable<ComponentLookupItem>>(components));
    }
}
