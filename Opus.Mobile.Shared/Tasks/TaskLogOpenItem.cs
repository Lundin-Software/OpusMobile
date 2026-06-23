namespace Opus.Mobile.Shared.Tasks;

public class TaskLogOpenItem
{
    public int? TaskLogId { get; set; }

    public int? TaskId { get; set; }

    public int? TaskIntervalId { get; set; }

    public int? TaskClassId { get; set; }

    public string? Title { get; set; }

    public string? Desc { get; set; }

    public int? ComponentId { get; set; }

    public string? ComponentTree { get; set; }

    public List<TaskLogHistoryItem> TaskLogs { get; set; } = [];
}
