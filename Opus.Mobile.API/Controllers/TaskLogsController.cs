using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Opus.Mobile.API.Services.TaskLogs;
using Opus.Mobile.Shared.API;
using Opus.Mobile.Shared.Tasks;
using System.Security.Claims;

namespace Opus.Mobile.API.Controllers;

[ApiController]
[Route("[controller]")]
public class TaskLogsController(ITaskLogService taskLogService) : ControllerBase
{
    [Authorize]
    [HttpPost("Open")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(APIResponse<TaskLogOpenItem>))]
    public async Task<ActionResult> OpenTaskLog([FromBody] OpenTaskLogRequest request)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userIdClaim is null || !int.TryParse(userIdClaim, out var userId))
            return BadRequest("Could not get userId from JWT Token");

        var taskLog = await taskLogService.GetOrCreateOpenTaskLog(userId, request);

        return Ok(new APIResponse<TaskLogOpenItem>(taskLog));
    }
}
