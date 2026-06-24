using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Opus.Mobile.API.Services.Purchases;
using Opus.Mobile.Shared.API;
using Opus.Mobile.Shared.Articles;
using System.Security.Claims;

namespace Opus.Mobile.API.Controllers;

[ApiController]
[Route("[controller]")]
public class PurchasesController(IPurchaseService purchaseService) : ControllerBase
{
    [Authorize]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(APIResponse<PurchaseItem>))]
    public async Task<ActionResult> SavePurchase([FromBody] SavePurchaseRequest request)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userIdClaim is null || !int.TryParse(userIdClaim, out var userId))
            return BadRequest("Could not get userId from JWT Token");

        var purchase = await purchaseService.SavePurchase(userId, request);

        return Ok(new APIResponse<PurchaseItem>(purchase));
    }
}
