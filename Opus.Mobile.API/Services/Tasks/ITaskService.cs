using Opus.Mobile.Shared.Documents;
using Opus.Mobile.Shared.TaskLogs;
using Opus.Mobile.Shared.Tasks;

namespace Opus.Mobile.API.Services.Tasks;

public interface ITaskService
{
    Task<IEnumerable<DocumentItem>> GetTaskDocuments(int taskId);

    Task<CreatedTaskItem> CreateTask(int userId, CreateTaskRequest request);

    Task<string> SaveTaskPhoto(int userId, int taskId, SaveTaskPhotoRequest request);

    Task<IEnumerable<TaskArticleItem>> GetTaskArticles(int taskId);

    Task<TaskPhotoItem?> TakePhoto(int userId, int taskId, TakeTaskPhotoRequest request);

    Task<TaskDocumentItem?> UploadTaskDocument(int userId, int taskId, UploadTaskDocumentRequest request);

    Task<string> EditPhoto(int taskId, EditTaskPhotoRequest request);
}
