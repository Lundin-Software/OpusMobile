namespace Opus.Mobile.Shared.TaskLogs;

public class TaskLogHistoryItem
{
    public int Id { get; set; }

    public int TaskId { get; set; }

    public string? AccomplishedBy { get; set; }

    public DateTime? AccomplishedDate { get; set; }

    public double? RunningHours { get; set; }

    public string? Remark { get; set; }

    public List<TaskFieldValueItem> TaskFields { get; set; } = [];
}
