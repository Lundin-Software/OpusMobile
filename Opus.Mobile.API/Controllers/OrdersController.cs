using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Opus.Mobile.API.Services.Orders;
using Opus.Mobile.Shared.API;
using Opus.Mobile.Shared.Orders;
using System.Security.Claims;

namespace Opus.Mobile.API.Controllers;

[ApiController]
[Route("[controller]")]
public class OrdersController(IOrderService orderService) : ControllerBase
{
    [Authorize]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(APIResponse<IEnumerable<OrderItem>>))]
    public async Task<ActionResult> GetOrders()
    {
        var orders = await orderService.GetOrders();

        return Ok(new APIResponse<IEnumerable<OrderItem>>(orders));
    }

    [Authorize]
    [HttpPost("{orderId:int}/Location")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(APIResponse<bool>))]
    public async Task<ActionResult> SetOrderLocation(
        [FromRoute] int orderId,
        [FromBody] SetOrderLocationRequest request)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userIdClaim is null || !int.TryParse(userIdClaim, out var userId))
            return BadRequest("Could not get userId from JWT Token");

        var result = await orderService.SetOrderLocation(userId, orderId, request);

        if (result != "OK")
            return Problem(result);

        return Ok(new APIResponse<bool>(true));
    }

    [Authorize]
    [HttpPost("{orderId:int}/Location/Label")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(APIResponse<string>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> PrintOrderLocationLabel([FromRoute] int orderId)
    {
        if (orderId <= 0)
            return BadRequest("OrderId is required.");

        await orderService.RequestOrderLocationLabelPrint(orderId);

        return Ok(new APIResponse<string>("OK\n\nRequest Sent"));
    }
}
