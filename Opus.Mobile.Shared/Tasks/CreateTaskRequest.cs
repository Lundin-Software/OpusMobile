namespace Opus.Mobile.Shared.Tasks;

public class CreateTaskRequest
{
    public int? TaskId { get; set; }

    public int? ComponentId { get; set; }

    public int? TaskTypeId { get; set; }

    public int? ResponsibleRoleId { get; set; }

    public DateOnly DeadlineDate { get; set; }

    public string? Title { get; set; }

    public string? Desc { get; set; }

    public bool Priority { get; set; }

    public bool OnHold { get; set; }

    public bool Warranty { get; set; }
}
