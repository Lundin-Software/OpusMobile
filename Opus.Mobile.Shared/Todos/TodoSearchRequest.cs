namespace Opus.Mobile.Shared.Todos;

public class TodoSearchRequest
{
    public int? ResponsibleRoleId { get; set; }

    public int? TaskTypeId { get; set; }

    public bool AssignedToMe { get; set; }
}
