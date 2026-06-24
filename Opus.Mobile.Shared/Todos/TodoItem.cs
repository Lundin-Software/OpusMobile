namespace Opus.Mobile.Shared.Todos;

public class TodoItem
{
    public int? TodoId { get; set; }
    public int? TaskLogId { get; set; }
    public int? TaskId { get; set; }
    public int? TaskIntervalId { get; set; }
    public int? TaskClassId { get; set; }

    public string? Title { get; set; }
    public string? Desc { get; set; }

    public double? LeftTime { get; set; }
    public string? LeftTimeChar { get; set; }
    public string? IntervalChar { get; set; }

    public DateTime? CompletedTime { get; set; }

    public int? ComponentId { get; set; }
    public string? ComponentTree { get; set; }
    public double? ComponentHours { get; set; }

    public int? TaskTypeId { get; set; }
    public string? TaskTypeName { get; set; }

    public int? UserId { get; set; }
    public string? UserShortName { get; set; }

    public int? Unscheduled { get; set; }
    public string? ResponsibleRoleName { get; set; }
}
