using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Opus.Mobile.API.Services.Documents;
using Opus.Mobile.Shared.API;
using Opus.Mobile.Shared.Documents;

namespace Opus.Mobile.API.Controllers;

[ApiController]
[Route("[controller]")]
public class DocumentsController(IDocumentService documentService) : ControllerBase
{
    [Authorize]
    [HttpPost("Search")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(APIResponse<IEnumerable<DocumentItem>>))]
    public async Task<ActionResult> SearchDocuments([FromBody] DocumentSearchRequest request)
    {
        var documents = await documentService.SearchDocuments(request);

        return Ok(new APIResponse<IEnumerable<DocumentItem>>(documents));
    }

    [Authorize]
    [HttpGet("{documentId:int}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(byte[]))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DownloadDocument([FromRoute] int documentId)
    {
        var document = await documentService.DownloadDocument(documentId);

        if (document is null)
            return NotFound();

        return Ok(document);
    }
}
