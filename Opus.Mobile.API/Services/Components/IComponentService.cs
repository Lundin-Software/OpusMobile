using Opus.Mobile.Shared.Components;

namespace Opus.Mobile.API.Services.Components;

public interface IComponentService
{
    Task<IEnumerable<ComponentTreeItem>> GetComponentTree();

    Task<ComponentLookupDetails> LoadLookupComponent(int employeeId, int componentId);

    Task<IEnumerable<ComponentDocumentItem>> GetComponentDocuments(int componentId);

    Task<ComponentDocumentItem> SaveComponentDocument(int userId, int componentId, SaveComponentDocumentRequest request);

    Task DeleteComponentDocument(int componentId, int componentDocumentId);
}
