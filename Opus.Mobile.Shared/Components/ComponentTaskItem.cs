namespace Opus.Mobile.Shared.Components;

public class ComponentTaskItem
{
    public int TaskId { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public double? LeftTime { get; set; }

    public DateTime? NextDate { get; set; }

    public string? IntervalChar { get; set; }

    public int? TaskClassId { get; set; }

    public string? TaskClassName { get; set; }

    public int? IntervalTypeId { get; set; }

    public int? TaskTypeId { get; set; }

    public int? TaskIntervalId { get; set; }
}
