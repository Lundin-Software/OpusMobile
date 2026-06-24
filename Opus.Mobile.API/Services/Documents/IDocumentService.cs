using Opus.Mobile.Shared.Documents;

namespace Opus.Mobile.API.Services.Documents;

public interface IDocumentService
{
    Task<IEnumerable<DocumentItem>> SearchDocuments(DocumentSearchRequest request);

    Task<byte[]?> DownloadDocument(int documentId);
}
