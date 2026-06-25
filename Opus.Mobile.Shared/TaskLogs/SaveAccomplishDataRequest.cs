namespace Opus.Mobile.Shared.TaskLogs;

public class SaveAccomplishDataRequest
{
    public int? TaskId { get; set; }

    public List<SaveTaskFieldDataItem> TaskFields { get; set; } = [];

    public List<SaveTaskArticleDataItem> TaskArticles { get; set; } = [];
}
