namespace Opus.Mobile.Shared.TaskLogs;

public class AccomplishTaskLogRequest
{
    public int? FromComp { get; set; }

    public int? Unsched { get; set; }

    public int? ComponentId { get; set; }

    public double? ComponentHours { get; set; }

    public int? TaskId { get; set; }

    public int? TaskIntervalId { get; set; }

    public int? TaskTypeId { get; set; }

    public double? Hours { get; set; }

    public string? Remark { get; set; }

    public int? ManHours { get; set; }

    public int? Automatic { get; set; }

    public DateTime? NextDate { get; set; }

    public int? PurchaseId { get; set; }
}
