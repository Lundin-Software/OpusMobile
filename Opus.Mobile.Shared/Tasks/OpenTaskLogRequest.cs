namespace Opus.Mobile.Shared.Tasks;

public class OpenTaskLogRequest
{
    public int? TaskId { get; set; }

    public int? TaskIntervalId { get; set; }

    public int? TaskTypeId { get; set; }

    public DateTime? NextDate { get; set; }
}
