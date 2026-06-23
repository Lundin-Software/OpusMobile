using Microsoft.AspNetCore.Mvc;
using Opus.Mobile.API.Services.Authentication;
using Opus.Mobile.Shared.API;
using Opus.Mobile.Shared.Authentication;

namespace Opus.Mobile.API.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthenticationController(IAuthenticationService authenticationService) : ControllerBase
{
    [HttpPost("Login")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(APIResponse<LoginResponse>))]
    public async Task<ActionResult> Login(LoginDTO login)
    {
        var result = await authenticationService.Login(login);

        return Ok(new APIResponse<LoginResponse>(result));
    }
}
