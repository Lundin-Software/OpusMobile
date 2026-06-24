namespace Opus.Mobile.Shared.Notifications;

public class CreateNotificationRequest
{
    public int? ComponentId { get; set; }

    public int? NotificationTypeId { get; set; }

    public int? CreatedByRoleId { get; set; }

    public int? ResponsibleRoleId { get; set; }

    public string? Title { get; set; }

    public string? Description { get; set; }

    public bool Priority { get; set; }

    public bool Completed { get; set; }
}
