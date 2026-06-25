using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Opus.Mobile.API.Services.TaskLogs;
using Opus.Mobile.API.Services.Tasks;
using Opus.Mobile.Shared.API;
using Opus.Mobile.Shared.Documents;
using Opus.Mobile.Shared.TaskLogs;
using Opus.Mobile.Shared.Tasks;
using System.Security.Claims;

namespace Opus.Mobile.API.Controllers;

[ApiController]
[Route("[controller]")]
public class TasksController(ITaskService taskService, ITaskLogService taskLogService) : ControllerBase
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

    [Authorize]
    [HttpPost("{taskId:int}/Photos")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(APIResponse<bool>))]
    public async Task<ActionResult> SaveTaskPhoto(
        [FromRoute] int taskId,
        [FromBody] SaveTaskPhotoRequest request)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userIdClaim is null || !int.TryParse(userIdClaim, out var userId))
            return BadRequest("Could not get userId from JWT Token");

        var result = await taskService.SaveTaskPhoto(userId, taskId, request);

        if (result != "OK")
            return BadRequest(result);

        return Ok(new APIResponse<bool>(true));
    }

    [Authorize]
    [HttpGet("{taskId:int}/Articles")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(APIResponse<IEnumerable<TaskArticleItem>>))]
    public async Task<ActionResult> GetTaskArticles([FromRoute] int taskId)
    {
        var articles = await taskService.GetTaskArticles(taskId);

        return Ok(new APIResponse<IEnumerable<TaskArticleItem>>(articles));
    }

    [Authorize]
    [HttpPost("{taskId:int}/Photo")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(APIResponse<TaskPhotoItem>))]
    public async Task<ActionResult> TakePhoto(
        [FromRoute] int taskId,
        [FromBody] TakeTaskPhotoRequest request)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userIdClaim is null || !int.TryParse(userIdClaim, out var userId))
            return BadRequest("Could not get userId from JWT Token");

        var photo = await taskService.TakePhoto(userId, taskId, request);

        if (photo is null)
            return NotFound();

        return Ok(new APIResponse<TaskPhotoItem>(photo));
    }

    [Authorize]
    [HttpPost("{taskId:int}/Documents")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(APIResponse<TaskDocumentItem>))]
    public async Task<ActionResult> UploadTaskDocument(
        [FromRoute] int taskId,
        [FromBody] UploadTaskDocumentRequest request)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userIdClaim is null || !int.TryParse(userIdClaim, out var userId))
            return BadRequest("Could not get userId from JWT Token");

        var document = await taskService.UploadTaskDocument(userId, taskId, request);

        if (document is null)
            return BadRequest("Could not upload task document.");

        return Ok(new APIResponse<TaskDocumentItem>(document));
    }

    [Authorize]
    [HttpPut("{taskId:int}/Photo")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(APIResponse<bool>))]
    public async Task<ActionResult> EditPhoto(
        [FromRoute] int taskId,
        [FromBody] EditTaskPhotoRequest request)
    {
        var result = await taskService.EditPhoto(taskId, request);

        if (result == "ok")
            return Ok(new APIResponse<bool>(true));

        return BadRequest(result);
    }

    [Authorize]
    [HttpGet("{taskId:int}/Logs")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(APIResponse<IEnumerable<TaskLogHistoryItem>>))]
    public async Task<ActionResult> GetTaskLogs([FromRoute] int taskId)
    {
        var logs = await taskLogService.GetTaskLogs(taskId);

        return Ok(new APIResponse<IEnumerable<TaskLogHistoryItem>>(logs));
    }
}
