using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Opus.Mobile.API.Services.Tasks;
using Opus.Mobile.Shared.API;
using Opus.Mobile.Shared.Documents;
using Opus.Mobile.Shared.Tasks;
using System.Security.Claims;

namespace Opus.Mobile.API.Controllers;

[ApiController]
[Route("[controller]")]
public class TasksController(ITaskService taskService) : ControllerBase
{
    [Authorize]
    [HttpGet("{taskId:int}/Documents")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(APIResponse<IEnumerable<DocumentItem>>))]
    public async Task<ActionResult> GetTaskDocuments([FromRoute] int taskId)
    {
        var documents = await taskService.GetTaskDocuments(taskId);

        return Ok(new APIResponse<IEnumerable<DocumentItem>>(documents));
    }

    [Authorize]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(APIResponse<CreatedTaskItem>))]
    public async Task<ActionResult> CreateTask([FromBody] CreateTaskRequest request)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userIdClaim is null || !int.TryParse(userIdClaim, out var userId))
            return BadRequest("Could not get userId from JWT Token");

        var task = await taskService.CreateTask(userId, request);

        return Ok(new APIResponse<CreatedTaskItem>(task));
    }
}
