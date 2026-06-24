using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Opus.Mobile.API.Services.Articles;
using Opus.Mobile.Shared.API;
using Opus.Mobile.Shared.Articles;
using System.Security.Claims;

namespace Opus.Mobile.API.Controllers;

[ApiController]
[Route("[controller]")]
public class ArticlesController(IArticleService articleService) : ControllerBase
{
    [Authorize]
    [HttpGet("Barcode/{barcode}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(APIResponse<ArticleItem>))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> GetArticleByBarcode([FromRoute] string barcode)
    {
        var article = await articleService.GetArticleByBarcode(barcode);

        if (article is null)
            return NotFound();

        return Ok(new APIResponse<ArticleItem>(article));
    }

    [Authorize]
    [HttpPost("{articleId:int}/Location")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(APIResponse<bool>))]
    public async Task<ActionResult> SetArticleLocation(
        [FromRoute] int articleId,
        [FromBody] SetArticleLocationRequest request)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userIdClaim is null || !int.TryParse(userIdClaim, out var userId))
            return BadRequest("Could not get userId from JWT Token");

        var result = await articleService.SetArticleLocation(
            userId,
            articleId,
            request);

        return Ok(new APIResponse<bool>(result));
    }

    [Authorize]
    [HttpPost("{articleId:int}/Images")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(APIResponse<SaveArticleImageResponse>))]
    public async Task<ActionResult> SaveArticleImage(
        [FromRoute] int articleId,
        [FromBody] SaveArticleImageRequest request)
    {
        var result = await articleService.SaveArticleImage(articleId, request);

        return Ok(new APIResponse<SaveArticleImageResponse>(result));
    }
}
