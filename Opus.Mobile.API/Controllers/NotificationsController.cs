using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Opus.Mobile.API.Services.Notifications;
using Opus.Mobile.Shared.API;
using Opus.Mobile.Shared.Notifications;
using System.Security.Claims;

namespace Opus.Mobile.API.Controllers;

[ApiController]
[Route("[controller]")]
public class NotificationsController(INotificationService notificationService) : ControllerBase
{
    [Authorize]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(APIResponse<NotificationItem>))]
    public async Task<ActionResult> CreateNotification([FromBody] CreateNotificationRequest request)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userIdClaim is null || !int.TryParse(userIdClaim, out var userId))
            return BadRequest("Could not get userId from JWT Token");

        var notification = await notificationService.CreateNotification(userId, request);

        return Ok(new APIResponse<NotificationItem>(notification));
    }

    [Authorize]
    [HttpPost("{notificationId:int}/Documents")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(APIResponse<NotificationDocumentItem>))]
    public async Task<ActionResult> UploadNotificationDocument(
        [FromRoute] int notificationId,
        [FromBody] UploadNotificationDocumentRequest request)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userIdClaim is null || !int.TryParse(userIdClaim, out var userId))
            return BadRequest("Could not get userId from JWT Token");

        var document = await notificationService.UploadNotificationDocument(
            userId,
            notificationId,
            request);

        if (document is null)
            return BadRequest("Could not upload notification document.");

        return Ok(new APIResponse<NotificationDocumentItem>(document));
    }
}
