using Microsoft.EntityFrameworkCore;
using Opus.Mobile.Data.Context;
using Opus.Mobile.Shared.Documents;

namespace Opus.Mobile.API.Services.Documents;

public class DocumentService(OpusDBContext ctx) : IDocumentService
{
    public async Task<IEnumerable<DocumentItem>> SearchDocuments(DocumentSearchRequest request)
    {
        var query = ctx.Documents.AsNoTracking().AsQueryable();

        if (request.ComponentId != 0)
        {
            query = query.Where(document =>
                document.ComponentDocuments.Any(componentDocument =>
                    componentDocument.ComponentId == request.ComponentId));
        }

        if (!string.IsNullOrWhiteSpace(request.SearchString))
        {
            query = query.Where(document =>
                document.Description != null &&
                document.Description.Contains(request.SearchString));
        }

        return await query
            .Where(document =>
                document.Id > request.LastId &&
                document.Extension != null &&
                DocumentExtensions.Contains(document.Extension.ToLower()))
            .OrderBy(document => document.Id)
            .Take(100)
            .Select(document => new DocumentItem
            {
                Id = document.Id,
                Description = document.Description,
                Extension = document.Extension
            })
            .ToListAsync();
    }

    public async Task<byte[]?> DownloadDocument(int documentId)
    {
        return await ctx.Documents
            .AsNoTracking()
            .Where(document => document.Id == documentId)
            .Select(document => document.Doc)
            .FirstOrDefaultAsync();
    }

    private static readonly string[] DocumentExtensions =
    [
        ".pdf",
        ".xlsx",
        ".xls",
        ".doc",
        ".docx"
    ];
}
