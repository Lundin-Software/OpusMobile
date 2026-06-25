using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Opus.Mobile.API.Services.Components;
using Opus.Mobile.Shared.API;
using Opus.Mobile.Shared.Components;
using Opus.Mobile.Shared.Documents;
using System.Security.Claims;

namespace Opus.Mobile.API.Controllers;

[ApiController]
[Route("[controller]")]
public class ComponentsController(IComponentService componentService) : ControllerBase
{
    [Authorize]
    [HttpGet("Tree")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(APIResponse<IEnumerable<ComponentTreeItem>>))]
    public async Task<ActionResult> GetTree()
    {
        var components = await componentService.GetComponentTree();

        return Ok(new APIResponse<IEnumerable<ComponentTreeItem>>(components));
    }

    [Authorize]
    [HttpGet("{componentId:int}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(APIResponse<ComponentLookupDetails>))]
    public async Task<ActionResult> LoadLookupComponent([FromRoute] int componentId)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userIdClaim is null || !int.TryParse(userIdClaim, out var userId))
            return BadRequest("Could not get userId from JWT Token");

        var component = await componentService.LoadLookupComponent(userId, componentId);

        return Ok(new APIResponse<ComponentLookupDetails>(component));
    }

    [Authorize]
    [HttpGet("{componentId:int}/Documents")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(APIResponse<IEnumerable<ComponentDocumentItem>>))]
    public async Task<ActionResult> GetComponentDocuments([FromRoute] int componentId)
    {
        var documents = await componentService.GetComponentDocuments(componentId);

        return Ok(new APIResponse<IEnumerable<ComponentDocumentItem>>(documents));
    }

    [Authorize]
    [HttpGet("{componentId:int}/FileDocuments")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(APIResponse<IEnumerable<DocumentItem>>))]
    public async Task<ActionResult> GetComponentFileDocuments([FromRoute] int componentId)
    {
        var documents = await componentService.GetComponentFileDocuments(componentId);

        return Ok(new APIResponse<IEnumerable<DocumentItem>>(documents));
    }

    [Authorize]
    [HttpPost("{componentId:int}/Documents")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(APIResponse<ComponentDocumentItem>))]
    public async Task<ActionResult> SaveComponentDocument(
        [FromRoute] int componentId,
        [FromBody] SaveComponentDocumentRequest request)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userIdClaim is null || !int.TryParse(userIdClaim, out var userId))
            return BadRequest("Could not get userId from JWT Token");

        var document = await componentService.SaveComponentDocument(
            userId,
            componentId,
            request);

        return Ok(new APIResponse<ComponentDocumentItem>(document));
    }

    [Authorize]
    [HttpDelete("{componentId:int}/Documents/{componentDocumentId:int}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(APIResponse<bool>))]
    public async Task<ActionResult> DeleteComponentDocument(
        [FromRoute] int componentId,
        [FromRoute] int componentDocumentId)
    {
        await componentService.DeleteComponentDocument(componentId, componentDocumentId);

        return Ok(new APIResponse<bool>(true));
    }

    [Authorize]
    [HttpGet("{componentId:int}/Details")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(APIResponse<ComponentDetailsItem>))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> GetComponentDetails([FromRoute] int componentId)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userIdClaim is null || !int.TryParse(userIdClaim, out var userId))
            return BadRequest("Could not get userId from JWT Token");

        var details = await componentService.GetComponentDetails(userId, componentId);

        if (details is null)
            return NotFound();

        return Ok(new APIResponse<ComponentDetailsItem>(details));
    }

    [Authorize]
    [HttpGet("{componentId:int}/Tasks")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(APIResponse<IEnumerable<ComponentTaskItem>>))]
    public async Task<ActionResult> GetComponentTasks([FromRoute] int componentId)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userIdClaim is null || !int.TryParse(userIdClaim, out var userId))
            return BadRequest("Could not get userId from JWT Token");

        var tasks = await componentService.GetComponentTasks(userId, componentId);

        return Ok(new APIResponse<IEnumerable<ComponentTaskItem>>(tasks));
    }

    [Authorize]
    [HttpGet("{componentId:int}/ClassFields")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(APIResponse<IEnumerable<ComponentClassFieldItem>>))]
    public async Task<ActionResult> GetComponentClassFields([FromRoute] int componentId)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userIdClaim is null || !int.TryParse(userIdClaim, out var userId))
            return BadRequest("Could not get userId from JWT Token");

        var fields = await componentService.GetComponentClassFields(userId, componentId);

        return Ok(new APIResponse<IEnumerable<ComponentClassFieldItem>>(fields));
    }
}
