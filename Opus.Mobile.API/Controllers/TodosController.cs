using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Opus.Mobile.API.Services.Todos;
using Opus.Mobile.Shared.API;
using Opus.Mobile.Shared.Todos;
using System.Security.Claims;

namespace Opus.Mobile.API.Controllers;

[ApiController]
[Route("[controller]")]
public class TodosController(ITodoService todoService) : ControllerBase
{
    [Authorize]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(APIResponse<IEnumerable<TodoItem>>))]
    public async Task<ActionResult> GetTodos([FromBody] TodoSearchRequest request)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userIdClaim is null || !int.TryParse(userIdClaim, out var userId))
            return BadRequest("Could not get userId from JWT Token");

        var todos = await todoService.GetTodos(userId, request);

        return Ok(new APIResponse<IEnumerable<TodoItem>>(todos));
    }
}
