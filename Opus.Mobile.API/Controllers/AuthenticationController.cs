using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Opus.Mobile.API.Services.Authentication;
using Opus.Mobile.Shared.API;
using Opus.Mobile.Shared.Authentication;
using System.Security.Claims;

namespace Opus.Mobile.API.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthenticationController(IAuthenticationService authenticationService) : ControllerBase
{
    [HttpPost("Login")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(APIResponse<LoginItem>))]
    public async Task<ActionResult> Login(LoginRequest login)
    {
        var result = await authenticationService.Login(login);

        return Ok(new APIResponse<LoginItem>(result));
    }

    [Authorize]
    [HttpGet("User")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(APIResponse<UserDetailsItem>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> GetUserDetails()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userIdClaim is null || !int.TryParse(userIdClaim, out var userId))
            return BadRequest("Could not get userId from JWT Token");

        var result = await authenticationService.GetUserDetails(userId);

        if (result is null)
            return NotFound();

        return Ok(new APIResponse<UserDetailsItem>(result));
    }
}
